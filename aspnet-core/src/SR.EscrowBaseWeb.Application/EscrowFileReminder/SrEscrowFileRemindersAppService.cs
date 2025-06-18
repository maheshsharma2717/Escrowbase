using SR.EscrowBaseWeb.EscrowFileMaster;
using SR.EscrowBaseWeb.Authorization.Users;

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using SR.EscrowBaseWeb.EscrowFileReminder.Dtos;
using SR.EscrowBaseWeb.Dto;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using SR.EscrowBaseWeb.Storage;
using SR.EscrowBaseWeb.EscrowDetails;
using Microsoft.Extensions.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using System.IO;
using SR.EscrowBaseWeb.Configuration;
using SR.EscrowBaseWeb.SrAssignedFilesDetails;
using System.Net.Mail;
using SR.EscrowBaseWeb.SREnterprise;
using SR.EscrowBaseWeb.SREscrowFileHistory.Dtos;
using SR.EscrowBaseWeb.SREscrowFileHistory;
using GetAllForLookupTableInput = SR.EscrowBaseWeb.EscrowFileReminder.Dtos.GetAllForLookupTableInput;
using Abp.Notifications;
using SR.EscrowBaseWeb.Notifications;
using Abp;
using Abp.Domain.Uow;
using Abp.Localization;

namespace SR.EscrowBaseWeb.EscrowFileReminder
{

    public class SrEscrowFileRemindersAppService : EscrowBaseWebAppServiceBase, ISrEscrowFileRemindersAppService
    {
        private readonly IRepository<SrEscrowFileReminder, long> _srEscrowFileReminderRepository;
        private readonly IRepository<SREscrowFileMaster, long> _lookup_srEscrowFileMasterRepository;
        private readonly IRepository<User, long> _lookup_userRepository;
        private readonly IRepository<EscrowDetail, long> _escrowDetailRepository;
        private readonly IConfigurationRoot _config;
        private readonly IRepository<SrAssignedFilesDetail, long> _srAssignedFilesDetailRepository;
        private readonly IRepository<Enterprise> _enterpriseRepository;
        private readonly IRepository<SREscrowFileMaster, long> _srEscrowFileMasterRepository;
        private readonly IEscrowFileHistoriesAppService _escrowFileHistoriesAppService;
        
        private readonly INotificationPublisher _notificationPublisher;
        private readonly INotificationSubscriptionManager _notificationSubscriptionManager;
        private readonly IAppNotifier _appNotifier;

        //static IConfiguration config = (new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build());




        public SrEscrowFileRemindersAppService(IRepository<SrEscrowFileReminder, long> srEscrowFileReminderRepository,
            IRepository<SREscrowFileMaster, long> lookup_srEscrowFileMasterRepository, IRepository<User,
                long> lookup_userRepository,
            IRepository<EscrowDetail, long> escrowDetailRepository,
                IAppConfigurationAccessor appConfigurationAccessor,
                IRepository<SrAssignedFilesDetail, long> srAssignedFilesDetailRepository,
                IRepository<Enterprise> enterpriseRepository,
                IEscrowFileHistoriesAppService escrowFileHistoriesAppService,
                IRepository<SREscrowFileMaster, long> srEscrowFileMasterRepository,
                INotificationPublisher notificationPublisher,
                INotificationSubscriptionManager notificationSubscriptionManager,
                IAppNotifier appNotifier
            )
        {
            _srEscrowFileReminderRepository = srEscrowFileReminderRepository;
            _lookup_srEscrowFileMasterRepository = lookup_srEscrowFileMasterRepository;
            _lookup_userRepository = lookup_userRepository;
            _escrowDetailRepository = escrowDetailRepository;
            _config = appConfigurationAccessor.Configuration;
            _srAssignedFilesDetailRepository = srAssignedFilesDetailRepository;
            _enterpriseRepository = enterpriseRepository;
            _escrowFileHistoriesAppService = escrowFileHistoriesAppService;
            _srEscrowFileMasterRepository = srEscrowFileMasterRepository;
            _notificationPublisher = notificationPublisher;
            _notificationSubscriptionManager = notificationSubscriptionManager;
            _appNotifier = appNotifier;
        }

        public async Task<PagedResultDto<GetSrEscrowFileReminderForViewDto>> GetAll(GetAllSrEscrowFileRemindersInput input)
        {

            var filteredSrEscrowFileReminders = _srEscrowFileReminderRepository.GetAll()
                        .Include(e => e.SREscrowFileMasterFk)
                        .Include(e => e.CreatedByFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.ReminderType.Contains(input.Filter) || e.SentTo.Contains(input.Filter) || e.ReminderText.Contains(input.Filter) || e.SentFrom.Contains(input.Filter) || e.SentToUserType.Contains(input.Filter) || e.SentFromUserType.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.SentFromUserTypeFilter), e => e.SentFromUserType == input.SentFromUserTypeFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.SREscrowFileMasterFileFullNameFilter), e => e.SREscrowFileMasterFk != null && e.SREscrowFileMasterFk.FileFullName == input.SREscrowFileMasterFileFullNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.CreatedByFk != null && e.CreatedByFk.Name == input.UserNameFilter);

            var pagedAndFilteredSrEscrowFileReminders = filteredSrEscrowFileReminders
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var srEscrowFileReminders = from o in pagedAndFilteredSrEscrowFileReminders
                                        join o1 in _lookup_srEscrowFileMasterRepository.GetAll() on o.SREscrowFileMasterId equals o1.Id into j1
                                        from s1 in j1.DefaultIfEmpty()

                                        join o2 in _lookup_userRepository.GetAll() on o.CreatedBy equals o2.Id into j2
                                        from s2 in j2.DefaultIfEmpty()

                                        select new
                                        {

                                            o.ReminderType,
                                            o.SentTo,
                                            o.ReminderText,
                                            o.SentFrom,
                                            o.CreatedAt,
                                            o.ReminderStatus,
                                            o.SentToUserType,
                                            o.SentFromUserType,
                                            Id = o.Id,
                                            SREscrowFileMasterFileFullName = s1 == null || s1.FileFullName == null ? "" : s1.FileFullName.ToString(),
                                            UserName = s2 == null || s2.Name == null ? "" : s2.Name.ToString()
                                        };

            var totalCount = await filteredSrEscrowFileReminders.CountAsync();

            var dbList = await srEscrowFileReminders.ToListAsync();
            var results = new List<GetSrEscrowFileReminderForViewDto>();

            foreach (var o in dbList)
            {
                var res = new GetSrEscrowFileReminderForViewDto()
                {
                    SrEscrowFileReminder = new SrEscrowFileReminderDto
                    {

                        ReminderType = o.ReminderType,
                        SentTo = o.SentTo,
                        ReminderText = o.ReminderText,
                        SentFrom = o.SentFrom,
                        CreatedAt = o.CreatedAt,
                        ReminderStatus = o.ReminderStatus,
                        SentToUserType = o.SentToUserType,
                        SentFromUserType = o.SentFromUserType,
                        Id = o.Id,
                    },
                    SREscrowFileMasterFileFullName = o.SREscrowFileMasterFileFullName,
                    UserName = o.UserName
                };

                results.Add(res);
            }

            return new PagedResultDto<GetSrEscrowFileReminderForViewDto>(
                totalCount,
                results
            );

        }

        public async Task<GetSrEscrowFileReminderForViewDto> GetSrEscrowFileReminderForView(long id)
        {
            var srEscrowFileReminder = await _srEscrowFileReminderRepository.GetAsync(id);

            var output = new GetSrEscrowFileReminderForViewDto { SrEscrowFileReminder = ObjectMapper.Map<SrEscrowFileReminderDto>(srEscrowFileReminder) };

            if (output.SrEscrowFileReminder.SREscrowFileMasterId != null)
            {
                var _lookupSREscrowFileMaster = await _lookup_srEscrowFileMasterRepository.FirstOrDefaultAsync((long)output.SrEscrowFileReminder.SREscrowFileMasterId);
                output.SREscrowFileMasterFileFullName = _lookupSREscrowFileMaster?.FileFullName?.ToString();
            }

            if (output.SrEscrowFileReminder.CreatedBy != null)
            {
                var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.SrEscrowFileReminder.CreatedBy);
                output.UserName = _lookupUser?.Name?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_SrEscrowFileReminders_Edit)]
        public async Task<GetSrEscrowFileReminderForEditOutput> GetSrEscrowFileReminderForEdit(EntityDto<long> input)
        {
            var srEscrowFileReminder = await _srEscrowFileReminderRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetSrEscrowFileReminderForEditOutput { SrEscrowFileReminder = ObjectMapper.Map<CreateOrEditSrEscrowFileReminderDto>(srEscrowFileReminder) };

            if (output.SrEscrowFileReminder.SREscrowFileMasterId != null)
            {
                var _lookupSREscrowFileMaster = await _lookup_srEscrowFileMasterRepository.FirstOrDefaultAsync((long)output.SrEscrowFileReminder.SREscrowFileMasterId);
                output.SREscrowFileMasterFileFullName = _lookupSREscrowFileMaster?.FileFullName?.ToString();
            }

            if (output.SrEscrowFileReminder.CreatedBy != null)
            {
                var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.SrEscrowFileReminder.CreatedBy);
                output.UserName = _lookupUser?.Name?.ToString();
            }

            return output;
        }

        public async Task<List<AssignedFileUser>> CreateOrEdit(CreateOrEditSrEscrowFileReminderDto input)
        {
            if (input.Id == null)
            {
                var dbSrFileMapping = _srEscrowFileMasterRepository.Get(input.SREscrowFileMasterId);
                foreach (var item in input.ReminderType)
                {

                    string messageStatus = "Not Sent";
                    switch (item.ReminderType)
                    {

                        case "Message":
                            foreach (var dbUser in input.assignedFileUser)
                            {
                                CreateOrEditEscrowFileHistoryDto createOrEditEscrowFileHistoryDto = new CreateOrEditEscrowFileHistoryDto();
                                createOrEditEscrowFileHistoryDto.FileFullPath = dbSrFileMapping.FileFullName;
                                createOrEditEscrowFileHistoryDto.UserId = AbpSession.UserId;
                                createOrEditEscrowFileHistoryDto.SrEscrowFileMasterId = input.SREscrowFileMasterId;

                                if (dbUser.IsChecked == false)
                                {
                                    dbUser.MessageStatus = "";
                                    continue;
                                }


                                if (!string.IsNullOrWhiteSpace(dbUser.PhoneNo) && !dbUser.PhoneNo.Contains("Registered"))
                                {

                                    var response = await SendReminderMessage(dbUser, input);
                                    messageStatus = response ? "Message Sent" : "Message not Sent";
                                    input.ReminderStatus = response;
                                    input.SentTo = dbUser.Email;
                                    input.SentToUserType = dbUser.UserType;
                                    await Create(input, item.ReminderType);
                                    dbUser.MessageStatus = messageStatus;
                                    // insert into file reminder

                                    //createOrEditEscrowFileHistoryDto.
                                    createOrEditEscrowFileHistoryDto.Message = response ? $"Message Sent to {dbUser.Email}" : "Message not Sent";
                                    createOrEditEscrowFileHistoryDto.ActionType = "";
                                    await AddReminderTOFileHistory(createOrEditEscrowFileHistoryDto);
                                }
                                else
                                {
                                    messageStatus = "Mobile number is not registered";
                                    input.ReminderStatus = false;
                                    input.SentTo = dbUser.Email;
                                    input.SentToUserType = dbUser.UserType;
                                    await Create(input, item.ReminderType);
                                    dbUser.MessageStatus = messageStatus;
                                    createOrEditEscrowFileHistoryDto.Message = $"Message not Sent to {dbUser.Email} due to mobile number is not registered";
                                    createOrEditEscrowFileHistoryDto.ActionType = "";
                                    await AddReminderTOFileHistory(createOrEditEscrowFileHistoryDto);


                                }
                            }
                            break;

                        case "Email":
                            foreach (var dbUser in input.assignedFileUser)
                            {
                                if (dbUser.IsChecked == false)
                                {
                                    dbUser.MessageStatus = "";
                                    continue;
                                }

                                CreateOrEditEscrowFileHistoryDto createOrEditEscrowFileHistoryDto = new CreateOrEditEscrowFileHistoryDto();
                                createOrEditEscrowFileHistoryDto.FileFullPath = dbSrFileMapping.FileFullName;
                                createOrEditEscrowFileHistoryDto.UserId = AbpSession.UserId;
                                createOrEditEscrowFileHistoryDto.SrEscrowFileMasterId = input.SREscrowFileMasterId;


                                var mailResponse = await SendReminderEmail(dbUser, input);

                                messageStatus = mailResponse ? "Mail Sent" : "Mail not Sent";
                                input.ReminderStatus = mailResponse;
                                input.SentTo = dbUser.Email;

                                input.SentToUserType = dbUser.UserType;
                                await Create(input, item.ReminderType);
                                dbUser.EmailStatus = messageStatus;

                                createOrEditEscrowFileHistoryDto.Message = mailResponse ? $"Mail Sent to {dbUser.Email}" : $"Mail not Sent to {dbUser.Email}";
                                createOrEditEscrowFileHistoryDto.ActionType = "File reminder";
                                await AddReminderTOFileHistory(createOrEditEscrowFileHistoryDto);

                            }


                            break;

                        case "DirectMessage":
                            foreach (var dbUser in input.assignedFileUser)
                            {
                                if (dbUser.IsChecked == true)
                                {

                                    string notificationName = AppNotificationNames.NewUserRegistered;





                                    string userid = dbUser.UserId.ToString();
                                    var ghh = UserIdentifier.Parse(userid);

                                    try
                                    {
                                        CreateOrEditEscrowFileHistoryDto createOrEditEscrowFileHistoryDto = new CreateOrEditEscrowFileHistoryDto();
                                        createOrEditEscrowFileHistoryDto.FileFullPath = dbSrFileMapping.FileFullName;
                                        createOrEditEscrowFileHistoryDto.UserId = AbpSession.UserId;
                                        createOrEditEscrowFileHistoryDto.SrEscrowFileMasterId = input.SREscrowFileMasterId;

                                        await _appNotifier.SendMessageAsync(
                ghh,
                 input.ReminderText,
                  NotificationSeverity.Success
                 );

                                        messageStatus = "Direct message sent";
                                        input.ReminderStatus = true;
                                        input.SentTo = dbUser.Email;

                                        input.SentToUserType = dbUser.UserType;
                                        await Create(input, item.ReminderType);
                                        dbUser.DirectMessageStatus = messageStatus;

                                        createOrEditEscrowFileHistoryDto.Message =   $"Direct message sent to {dbUser.Email}";
                                        createOrEditEscrowFileHistoryDto.ActionType = "File reminder";
                                        await AddReminderTOFileHistory(createOrEditEscrowFileHistoryDto);





                                    }
                                    catch (Exception ex)
                                    {

                                    }

                                }
                                else
                                {
                                    dbUser.DirectMessageStatus = "";
                                }
                            }
                            break;
                    }

                }
            }
            else
            {

                await Update(input);
            }
            return input.assignedFileUser;
        }
        [UnitOfWork]
        private async Task<UserIdentifier[]> GetHostUserIds()
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                var hostUsers = await _lookup_userRepository.GetAll().Where(e => e.TenantId == null).ToListAsync();

                var hostUserIds = new UserIdentifier[hostUsers.Count];

                for (var i = 0; i < hostUsers.Count; i++)
                {
                    hostUserIds[i] = hostUsers[i].ToUserIdentifier();
                }

                return hostUserIds;
            }
        }
        protected virtual async Task<bool> SendReminderMessage(AssignedFileUser user, CreateOrEditSrEscrowFileReminderDto input)
        {
            string AccountSid = _config["Twilio:AccountSid"].ToString();
            string AuthToken = _config["Twilio:AuthToken"].ToString();
            string MessagingServiceSid = _config["Twilio:MessagingServiceSid"].ToString();
            //string SenderNumber = _config["Twilio:SenderNumber"].ToString();

            var cc = _config["CountryCode"];
            TwilioClient.Init(AccountSid, AuthToken);

            if (!user.PhoneNo.Contains("+1"))
            {
                user.PhoneNo = "+1" + user.PhoneNo;
            }

            var messageOptions = new CreateMessageOptions(
                new PhoneNumber(user.PhoneNo));

            messageOptions.Body = $"Hi You have reminder message from  your Escrow company {input.EscrowNumber} and sent by Escrow officer  {user.FullName}   {input.ReminderText}";

            //  messageOptions.Body = "You have reminder on the secure web portal regarding your escrow " + input.EscrowNumber+ " that needs your attention. Please log in to review, this reminder was sent by escrowbaseweb powered by software reality. File Name is " + "'" + fileName + "'";

            //messageOptions.From = SenderNumber;
            messageOptions.MessagingServiceSid = MessagingServiceSid;

            var message = MessageResource.Create(messageOptions);
            var res = message.Status.ToString();
            if (res == "accepted")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected virtual async Task<bool> SendReminderEmail(AssignedFileUser user, CreateOrEditSrEscrowFileReminderDto input)
        {
            bool response = false;
            try
            {

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("Noreply@EscrowBasePortal.com");
                mail.To.Add(user.Email);
                mail.Subject = "File Reminder";
                string referer = _config["App:ClientRootAddress"].ToString();

                var escrow = _escrowDetailRepository.GetAll().Where(x => x.EscrowId == input.EscrowNumber).FirstOrDefault();
                var enterprises = _enterpriseRepository.GetAll().Where(x => x.EnterpriseName == escrow.Company).FirstOrDefault();
                var Company = enterprises.EnterpriseName;
                var userName = user.FullName;
                var logo = enterprises.Logo;
                if (logo == "" || logo == null) { logo = "https://ayushkamiya.com/Escrow-logo.png"; }
                //var Message = "You have reminder message regarding your escrow " + input.EscrowNumber + " that needs your attention.  File Name is " + "'" + fileName + "'" + "'\n\n" + " " + input.ReminderText + "'\n\n";
                var Title = "You have a reminder message regarding the file assigned to your escrow  " + input.EscrowNumber +
              " that needs your attention.";

                var Message = input.ReminderText;
                string texts = "";
                using (StreamReader reader = System.IO.File.OpenText("wwwroot\\ReminderTemplate.html")) // Path to your Email format
                {
                    texts = reader.ReadToEnd();
                    texts = texts.Replace("$$Company$$", Company).Replace("$$Logo$$", logo).Replace("$$Title$$", Title).Replace("$$Message$$", Message).Replace("$$userName$$", userName);
                }


                Random rnd = new Random();

                mail.IsBodyHtml = true;
                mail.Body = texts;
                SmtpClient SmtpServer = new SmtpClient();
                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("office@mandavconsultancy.com", "aouownmhogfobzbc");
                SmtpServer.Host = "smtp.gmail.com";
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);


                response = true;
            }
            catch (Exception exception)
            {
                response = false;
            }
            return response;
        }
        protected virtual void SendReminderDirectMessage(string toNumber, string message)
        {

        }

        //[AbpAuthorize(AppPermissions.Pages_SrEscrowFileReminders_Create)]
        protected virtual async Task Create(CreateOrEditSrEscrowFileReminderDto input, string reminderType)
        {

            SrEscrowFileReminder srEscrowFileReminder = new SrEscrowFileReminder();
            srEscrowFileReminder.CreatedAt = DateTime.Now;
            srEscrowFileReminder.CreatedBy = input.CreatedBy;
            srEscrowFileReminder.ReminderStatus = input.ReminderStatus;
            srEscrowFileReminder.ReminderText = input.ReminderText;
            srEscrowFileReminder.ReminderType = reminderType;
            srEscrowFileReminder.SentFrom = input.SentFrom;
            srEscrowFileReminder.SentFromUserType = input.SentFromUserType;
            srEscrowFileReminder.SentTo = input.SentTo;
            srEscrowFileReminder.SentToUserType = input.SentToUserType;
            srEscrowFileReminder.SREscrowFileMasterId = input.SREscrowFileMasterId;
            await _srEscrowFileReminderRepository.InsertAsync(srEscrowFileReminder);
        }

        //  [AbpAuthorize(AppPermissions.Pages_SrEscrowFileReminders_Edit)]
        protected virtual async Task Update(CreateOrEditSrEscrowFileReminderDto input)
        {
            var srEscrowFileReminder = await _srEscrowFileReminderRepository.FirstOrDefaultAsync((long)input.Id);
            ObjectMapper.Map(input, srEscrowFileReminder);

        }

        //[AbpAuthorize(AppPermissions.Pages_SrEscrowFileReminders_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            await _srEscrowFileReminderRepository.DeleteAsync(input.Id);
        }

        [AbpAuthorize(AppPermissions.Pages_SrEscrowFileReminders)]
        public async Task<PagedResultDto<SrEscrowFileReminderSREscrowFileMasterLookupTableDto>> GetAllSREscrowFileMasterForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_srEscrowFileMasterRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.FileFullName != null && e.FileFullName.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var srEscrowFileMasterList = await query
                .PageBy(input)
                .ToListAsync(); 

            var lookupTableDtoList = new List<SrEscrowFileReminderSREscrowFileMasterLookupTableDto>();
            foreach (var srEscrowFileMaster in srEscrowFileMasterList)
            {
                lookupTableDtoList.Add(new SrEscrowFileReminderSREscrowFileMasterLookupTableDto
                {
                    Id = srEscrowFileMaster.Id,
                    DisplayName = srEscrowFileMaster.FileFullName?.ToString()
                });
            }

            return new PagedResultDto<SrEscrowFileReminderSREscrowFileMasterLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_SrEscrowFileReminders)]
        public async Task<PagedResultDto<SrEscrowFileReminderUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_userRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Name != null && e.Name.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var userList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<SrEscrowFileReminderUserLookupTableDto>();
            foreach (var user in userList)
            {
                lookupTableDtoList.Add(new SrEscrowFileReminderUserLookupTableDto
                {
                    Id = user.Id,
                    DisplayName = user.Name?.ToString()
                });
            }

            return new PagedResultDto<SrEscrowFileReminderUserLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        private async Task<CreateOrEditEscrowFileHistoryDto> AddReminderTOFileHistory(CreateOrEditEscrowFileHistoryDto escrowFileHistory)
        {
            //CreateOrEditEscrowFileHistoryDto escrowFileHistory = new CreateOrEditEscrowFileHistoryDto();
            escrowFileHistory.SrEscrowFileMasterId = escrowFileHistory.SrEscrowFileMasterId;
            escrowFileHistory.FileFullPath = escrowFileHistory.FileFullPath;
            escrowFileHistory.UserId = AbpSession.UserId;
            escrowFileHistory.Message = escrowFileHistory.Message;
            escrowFileHistory.ActionType = escrowFileHistory.ActionType;
            await _escrowFileHistoriesAppService.CreateOrEdit(escrowFileHistory);
            return escrowFileHistory;
        }

    }

}