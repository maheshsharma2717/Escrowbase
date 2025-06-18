using SR.EscrowBaseWeb.Authorization.Users;
using SR.EscrowBaseWeb.Authorization.Users;

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using SR.EscrowBaseWeb.EscrowDirectMessage.Dtos;
using SR.EscrowBaseWeb.Dto;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using SR.EscrowBaseWeb.Storage;
using Abp;
using Abp.Notifications;
using SR.EscrowBaseWeb.Notifications;
using System.Net.Mail;
using System.IO;
using Microsoft.Extensions.Configuration;
using SR.EscrowBaseWeb.SREnterprise;
using SR.EscrowBaseWeb.EscrowFileMaster;
using SR.EscrowBaseWeb.Configuration;
using SR.EscrowBaseWeb.EscrowDetails;
using Twilio;
using Twilio.Types;
using Twilio.Rest.Api.V2010.Account;

namespace SR.EscrowBaseWeb.EscrowDirectMessage
{
    // [AbpAuthorize(AppPermissions.Pages_EscrowDirectMessageDetailses)]
    public class EscrowDirectMessageDetailsesAppService : EscrowBaseWebAppServiceBase, IEscrowDirectMessageDetailsesAppService
    {
        private readonly IRepository<EscrowDirectMessageDetails, long> _escrowDirectMessageDetailsRepository;
        private readonly IRepository<User, long> _lookup_userRepository;
        private readonly IAppNotifier _appNotifier;
        private readonly IConfigurationRoot _config;
        private readonly IRepository<Enterprise> _enterpriseRepository;
        private readonly IRepository<EscrowDetail, long> _escrowDetailRepository;

        public EscrowDirectMessageDetailsesAppService(IRepository<EscrowDirectMessageDetails, long> escrowDirectMessageDetailsRepository,
            IRepository<User, long> lookup_userRepository,
             IAppNotifier appNotifier,
               IAppConfigurationAccessor appConfigurationAccessor,
                IRepository<Enterprise> enterpriseRepository,
                IRepository<EscrowDetail, long> escrowDetailRepository
            )
        {
            _escrowDirectMessageDetailsRepository = escrowDirectMessageDetailsRepository;
            _lookup_userRepository = lookup_userRepository;
            _appNotifier = appNotifier;
            _config = appConfigurationAccessor.Configuration;
            _escrowDetailRepository = escrowDetailRepository;
            _enterpriseRepository = enterpriseRepository;

        }

        public async Task<PagedResultDto<GetEscrowDirectMessageDetailsForViewDto>> GetAll(GetAllEscrowDirectMessageDetailsesInput input)
        {

            var filteredEscrowDirectMessageDetailses = _escrowDirectMessageDetailsRepository.GetAll()
                        .Include(e => e.EscrowUserFk)
                        .Include(e => e.SenderUserFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Message.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.EscrowUserFk != null && e.EscrowUserFk.Name == input.UserNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserName2Filter), e => e.SenderUserFk != null && e.SenderUserFk.Id == long.Parse(input.UserName2Filter));

            var pagedAndFilteredEscrowDirectMessageDetailses = filteredEscrowDirectMessageDetailses
                .OrderBy(input.Sorting ?? "id asc")
                .OrderByDescending(x => x.CreatedDate)
                .PageBy(input);

            var escrowDirectMessageDetailses = from o in pagedAndFilteredEscrowDirectMessageDetailses
                                               join o1 in _lookup_userRepository.GetAll() on o.EscrowUserId equals o1.Id into j1
                                               from s1 in j1.DefaultIfEmpty()

                                               join o2 in _lookup_userRepository.GetAll() on o.SenderUserId equals o2.Id into j2
                                               from s2 in j2.DefaultIfEmpty()

                                               select new
                                               {

                                                   Id = o.Id,
                                                   UserName = s1 == null || s1.Name == null ? "" : s1.Name.ToString(),
                                                   UserName2 = s2 == null || s2.Name == null ? "" : s2.Name.ToString(),
                                                   Message = o.Message,
                                                   CreatedDate = o.CreatedDate,
                                                   Status = o.Status,
                                                   MessageType = o.MessageType
                                               };

            var totalCount = await filteredEscrowDirectMessageDetailses.CountAsync();

            var dbList = await escrowDirectMessageDetailses.ToListAsync();
            var results = new List<GetEscrowDirectMessageDetailsForViewDto>();

            foreach (var o in dbList)
            {
                var res = new GetEscrowDirectMessageDetailsForViewDto()
                {
                    EscrowDirectMessageDetails = new EscrowDirectMessageDetailsDto
                    {

                        Id = o.Id,
                        Message = o.Message,
                        Status = o.Status ? "Sent Sucessfully" : "Sent Failed",
                        CreatedDate = o.CreatedDate,
                        MessageType = o.MessageType
                    },
                    UserName = o.UserName,
                    UserName2 = o.UserName2,


                };

                results.Add(res);
            }

            return new PagedResultDto<GetEscrowDirectMessageDetailsForViewDto>(
                totalCount,
                results.OrderByDescending(x => x.EscrowDirectMessageDetails.CreatedDate).ToList()
            );

        }

        public async Task<GetEscrowDirectMessageDetailsForViewDto> GetEscrowDirectMessageDetailsForView(long id)
        {
            var escrowDirectMessageDetails = await _escrowDirectMessageDetailsRepository.GetAsync(id);

            var output = new GetEscrowDirectMessageDetailsForViewDto { EscrowDirectMessageDetails = ObjectMapper.Map<EscrowDirectMessageDetailsDto>(escrowDirectMessageDetails) };

            if (output.EscrowDirectMessageDetails.EscrowUserId != null)
            {
                var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.EscrowDirectMessageDetails.EscrowUserId);
                output.UserName = _lookupUser?.Name?.ToString();
            }

            if (output.EscrowDirectMessageDetails.SenderUserId != null)
            {
                var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.EscrowDirectMessageDetails.SenderUserId);
                output.UserName2 = _lookupUser?.Name?.ToString();
            }

            return output;
        }

        // [AbpAuthorize(AppPermissions.Pages_EscrowDirectMessageDetailses_Edit)]
        public async Task<GetEscrowDirectMessageDetailsForEditOutput> GetEscrowDirectMessageDetailsForEdit(EntityDto<long> input)
        {
            var escrowDirectMessageDetails = await _escrowDirectMessageDetailsRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetEscrowDirectMessageDetailsForEditOutput { EscrowDirectMessageDetails = ObjectMapper.Map<CreateOrEditEscrowDirectMessageDetailsDto>(escrowDirectMessageDetails) };

            if (output.EscrowDirectMessageDetails.EscrowUserId != null)
            {
                var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.EscrowDirectMessageDetails.EscrowUserId);
                output.UserName = _lookupUser?.Name?.ToString();
            }

            if (output.EscrowDirectMessageDetails.SenderUserId != null)
            {
                var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.EscrowDirectMessageDetails.SenderUserId);
                output.UserName2 = _lookupUser?.Name?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditEscrowDirectMessageDetailsDto input)
        {
            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        // [AbpAuthorize(AppPermissions.Pages_EscrowDirectMessageDetailses_Create)]
        protected virtual async Task Create(CreateOrEditEscrowDirectMessageDetailsDto input)
        {
            var userIds = UserIdentifier.Parse(input.EscrowUserId.ToString());
            input.CreatedDate = DateTime.UtcNow;
            try
            {
                foreach (var item in input.ReminderType)
                {
                    switch (item.ReminderType)
                    {
                        case "Message":
                            var Message = $"This is the  message from your Escrow:  { input.EscrowNumber}    {input.Message}  Sent by  {_lookup_userRepository.Get(long.Parse(input.SenderUserId.ToString())).EmailAddress} userType ({input.UserType})";
                            var dbuser = _lookup_userRepository.Get(long.Parse(input.EscrowUserId.ToString()));
                            var messageResponse = await SendReminderEmail(dbuser, Message, input);
                            input.Status = messageResponse ? true : false;
                            var escrowDirectMessageDetailsMessage = ObjectMapper.Map<EscrowDirectMessageDetails>(input);
                            escrowDirectMessageDetailsMessage.MessageType = "Message";
                            await _escrowDirectMessageDetailsRepository.InsertAsync(escrowDirectMessageDetailsMessage);
                            break;
                        case "Email":
                            var emailMessage = $"This is the email from your Escrow:  { input.EscrowNumber}    {input.Message}  Sent by  {_lookup_userRepository.Get(long.Parse(input.SenderUserId.ToString())).EmailAddress} userType ({input.UserType})";
                            var user = _lookup_userRepository.Get(long.Parse(input.EscrowUserId.ToString()));
                            var mailResponse = await SendReminderEmail(user, emailMessage, input);
                            input.Status = mailResponse ? true : false;
                            var escrowDirectMessageDetails = ObjectMapper.Map<EscrowDirectMessageDetails>(input);
                            escrowDirectMessageDetails.MessageType = "Email";
                            await _escrowDirectMessageDetailsRepository.InsertAsync(escrowDirectMessageDetails);
                            break;
                        case "DirectMessage":
                            var directMessage = $"This is the direct message from your Escrow:  { input.EscrowNumber}    {input.Message}  Sent by  {_lookup_userRepository.Get(long.Parse(input.SenderUserId.ToString())).EmailAddress} userType ({input.UserType})";
                            await _appNotifier.SendMessageAsync(userIds, directMessage, NotificationSeverity.Success);
                            input.Status = true;
                            var escrowDirectMessageDetailsDirect = ObjectMapper.Map<EscrowDirectMessageDetails>(input);
                            escrowDirectMessageDetailsDirect.MessageType = "Direct Message";
                            await _escrowDirectMessageDetailsRepository.InsertAsync(escrowDirectMessageDetailsDirect);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                input.Status = false;
                var escrowDirectMessageDetails = ObjectMapper.Map<EscrowDirectMessageDetails>(input);
                await _escrowDirectMessageDetailsRepository.InsertAsync(escrowDirectMessageDetails);
            }
        }


        protected virtual async Task<bool> SendReminderEmail(User user, string message, CreateOrEditEscrowDirectMessageDetailsDto input)
        {
            bool response = false;
            try
            {

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("Noreply@EscrowBasePortal.com");
                mail.To.Add(user.EmailAddress);
                mail.Subject = "File Reminder";
                string referer = _config["App:ClientRootAddress"].ToString();

                var escrow = _escrowDetailRepository.GetAll().Where(x => x.EscrowId == input.EscrowNumber).FirstOrDefault();
                var enterprises = _enterpriseRepository.GetAll().Where(x => x.EnterpriseName == escrow.Company).FirstOrDefault();
                var Company = enterprises.EnterpriseName;
                var userName = user.FullName;
                var logo = enterprises.Logo;
                if (logo == "" || logo == null) { logo = "https://ayushkamiya.com/Escrow-logo.png"; }
                //var Message = "You have reminder message regarding your escrow " + input.EscrowNumber + " that needs your attention.  File Name is " + "'" + fileName + "'" + "'\n\n" + " " + input.ReminderText + "'\n\n";



                string texts = "";
                using (StreamReader reader = System.IO.File.OpenText("wwwroot\\ReminderTemplate.html")) // Path to your Email format
                {
                    texts = reader.ReadToEnd();
                    texts = texts.Replace("$$Company$$", Company).Replace("$$Logo$$", logo).Replace("$$Title$$", "").Replace("$$Message$$", message).Replace("$$userName$$", userName);
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

        protected virtual async Task<bool> SendReminderMessage(User user, string message, CreateOrEditEscrowDirectMessageDetailsDto input)
        {
            string AccountSid = _config["Twilio:AccountSid"].ToString();
            string AuthToken = _config["Twilio:AuthToken"].ToString();
            string MessagingServiceSid = _config["Twilio:MessagingServiceSid"].ToString();
            //string SenderNumber = _config["Twilio:SenderNumber"].ToString();

            var cc = _config["CountryCode"];
            TwilioClient.Init(AccountSid, AuthToken);

            if (!user.PhoneNumber.Contains("+1"))
            {
                user.PhoneNumber = "+1" + user.PhoneNumber;
            }

            var messageOptions = new CreateMessageOptions(
                new PhoneNumber(user.PhoneNumber));

            //  messageOptions.Body = $"Hi You have reminder message from  your Escrow company {input.EscrowNumber} and sent by Escrow officer  {user.FullName}   {input.ReminderText}";
            messageOptions.Body = message;

            messageOptions.MessagingServiceSid = MessagingServiceSid;

            var messageRequest = MessageResource.Create(messageOptions);
            var res = messageRequest.Status.ToString();
            if (res == "accepted")
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        //  [AbpAuthorize(AppPermissions.Pages_EscrowDirectMessageDetailses_Edit)]
        protected virtual async Task Update(CreateOrEditEscrowDirectMessageDetailsDto input)
        {
            var escrowDirectMessageDetails = await _escrowDirectMessageDetailsRepository.FirstOrDefaultAsync((long)input.Id);
            ObjectMapper.Map(input, escrowDirectMessageDetails);

        }

        // [AbpAuthorize(AppPermissions.Pages_EscrowDirectMessageDetailses_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            await _escrowDirectMessageDetailsRepository.DeleteAsync(input.Id);
        }

        //  [AbpAuthorize(AppPermissions.Pages_EscrowDirectMessageDetailses)]
        public async Task<PagedResultDto<EscrowDirectMessageDetailsUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_userRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Name != null && e.Name.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var userList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<EscrowDirectMessageDetailsUserLookupTableDto>();
            foreach (var user in userList)
            {
                lookupTableDtoList.Add(new EscrowDirectMessageDetailsUserLookupTableDto
                {
                    Id = user.Id,
                    DisplayName = user.Name?.ToString()
                });
            }

            return new PagedResultDto<EscrowDirectMessageDetailsUserLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

    }
}