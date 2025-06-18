using Abp;
using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Notifications;
using Aspose.Words;
using CsvHelper;
using foxit.addon.pageeditor;
//using foxit.common;
//using foxit.common.fxcrt;
using foxit.pdf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using MySqlConnector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using SR.EscrowBaseWeb.Authorization.Users;
using SR.EscrowBaseWeb.Authorization.Users.Dto;
using SR.EscrowBaseWeb.E_SignRecords;
using SR.EscrowBaseWeb.E_SignRecords.Dtos;
using SR.EscrowBaseWeb.EscrowDetails;
using SR.EscrowBaseWeb.EscrowDetails.Dtos;
using SR.EscrowBaseWeb.EscrowFileMaster;
using SR.EscrowBaseWeb.Friendships;
using SR.EscrowBaseWeb.Friendships.Dto;
using SR.EscrowBaseWeb.Invitee;
using SR.EscrowBaseWeb.Invitee.Dtos;
using SR.EscrowBaseWeb.SrAssignedFilesDetails;
using SR.EscrowBaseWeb.SREnterprise;
using SR.EscrowBaseWeb.SREscrowClient;
using SR.EscrowBaseWeb.SREscrowClient.Dtos;
using SR.EscrowBaseWeb.SREscrowFileHistory;
using SR.EscrowBaseWeb.SREscrowFileHistory.Dtos;
using SR.EscrowBaseWeb.SrEscrows;
using SR.EscrowBaseWeb.SrEscrows.Dtos;
using SR.EscrowBaseWeb.SrEscrowUserMapping;
using SR.EscrowBaseWeb.SrEscrowUserMapping.Dtos;
using SR.EscrowBaseWeb.SRFileMapping;
using SR.EscrowBaseWeb.SRFileMapping.Dtos;
using SR.EscrowBaseWeb.SrInvitationRecords;
using SR.EscrowBaseWeb.SrInvitationRecords.Dtos;
using SR.EscrowBaseWeb.SRSecurityQuestion;
using SR.EscrowBaseWeb.Web.Chat.SignalR;
using SR.EscrowBaseWeb.Web.Models;
using SR.EscrowBaseWeb.Web.Models.Ui;
//using System;
//using System.Collections.Generic;
using SR.EscrowBaseWeb.Web.Models.ZohoESign;
using SR.EscrowWeb.Enterprise.utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

//using GrapeCity.Documents.Pdf;
//using System.Text;

namespace SR.EscrowBaseWeb.Web.Controllers
{
    ///<Summary>
    /// Home controller
    ///</Summary>
    public class HomeController : EscrowBaseWebControllerBase
    {

        private readonly IRepository<Enterprise> _enterpriseRepository;
        private readonly IRepository<EscrowClient> _escrowClientRepository;
        private readonly UserAppService _IUserAppService;
        private readonly EscrowUserMappingsAppService _IEscrowUserMappingsAppService;
        private readonly SrFileMappingsAppService _ISrFileMappingsAppService;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<EscrowDetail, long> _escrowDetailRepository;
        private readonly E_SignRecordsAppService _e_SignRecordsAppService;
        private readonly IRepository<UserRole, long> _roleRepository;
        private readonly IRepository<SecurityQuestion> _securityRepository;
        private readonly IRepository<SrFileMapping> _srfilemapRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly TokenAuthController _tokenAuthControler;
        private readonly EscrowDetailsAppService _IescrowDetailsAppService;
        private readonly IRepository<SrEscrow> _ISrEscrowRepository;
        private readonly INotificationPublisher _notificationPublisher;
        private readonly IRepository<E_SignRecord, long> _esignRepository;
        private readonly IRepository<SrAssignedFilesDetail, long> _srAssignedFilesDetailRepository;
        private readonly SrAssignedFilesDetailsAppService _srAssignedFilesDetailsAppService;
        IRepository<SrInvitationRecord, long> _srInvitationRecordRepository;
        private readonly SrEscrowsAppService _srEscrowsAppService;
        private readonly SrInvitationRecordsAppService _ISrInvitationRecordsAppService;
        IWebHostEnvironment _hostingEnvironment;
        private readonly IHubContext<ChatHub> _hub;
        private IUnitOfWorkManager _unitOfWorkManager;
        private readonly IEscrowFileHistoriesAppService _escrowFileHistoriesAppService;
        private readonly IRepository<SREscrowFileMaster, long> _srEscrowFileMasterRepository;
        private readonly IFriendshipAppService _friendshipAppService;
        ///<Summary>
        /// Static string parameters
        ///</Summary>
        protected static string escrownumber = String.Empty, fileslist = String.Empty, sttrescro = String.Empty, sttrenterprise = String.Empty, newname = String.Empty, fulldestfile = String.Empty, pdfname = String.Empty, finalstring = String.Empty, passw = "", username = "", typestore = "";


        //protected static string esignPath = string.Empty;
        //protected static string esignKey = string.Empty;
        //protected static string esignUser = string.Empty;
        ///<Summary>
        /// Static int parameters
        ///</Summary>
        public static int Idno, getid = 0;
        Regex regexx = new Regex(@"\{.*?\}");
        static IConfiguration conf = (new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build());


        ///<Summary>
        /// Get all required data for home controller
        ///</Summary>
        public HomeController(IRepository<EscrowClient> escrowClientRepository,
            IRepository<Enterprise> enterpriseRepository,
            TokenAuthController tokenAuthControler,
            IRepository<E_SignRecord, long> esignRepository,
            UserAppService IUserAppService,
            EscrowUserMappingsAppService IEscrowUserMappingsAppService,
            SrFileMappingsAppService ISrFileMappingsAppService,
            IPasswordHasher<User> passwordHasher,
            IRepository<User, long> userRepository,
            IRepository<EscrowDetail, long> escrowDetailRepository,
            E_SignRecordsAppService e_SignRecordsAppService,
            IRepository<SrFileMapping> srfilemapRepository,
            IRepository<UserRole, long> roleRepository,
            IWebHostEnvironment hostingEnvironment,
            IRepository<SecurityQuestion> securityRepository,
            EscrowDetailsAppService IescrowDetailsAppService,
            IRepository<SRInvitee, int> srinviteeRepository,
            IRepository<SrInvitationRecord, long> srInvitationRecordRepository,
            IRepository<SrEscrow> ISrEscrowRepository,
            SrEscrowsAppService srEscrowsAppService,
            SrInvitationRecordsAppService ISrInvitationRecordsAppService,
            INotificationPublisher notificationPublisher,
            IRepository<SrAssignedFilesDetail, long> srAssignedFilesDetailRepository,
            SrAssignedFilesDetailsAppService srAssignedFilesDetailsAppService,
            IUnitOfWorkManager unitOfWorkManager,
            IHubContext<ChatHub> hub,
            IEscrowFileHistoriesAppService escrowFileHistoriesAppService,
            IRepository<SREscrowFileMaster, long> srEscrowFileMasterRepository,
            IFriendshipAppService friendshipAppService
            )
        {
            _escrowClientRepository = escrowClientRepository;
            _enterpriseRepository = enterpriseRepository;
            _e_SignRecordsAppService = e_SignRecordsAppService;
            _IUserAppService = IUserAppService;
            _IEscrowUserMappingsAppService = IEscrowUserMappingsAppService;
            _ISrFileMappingsAppService = ISrFileMappingsAppService;
            _userRepository = userRepository;
            _srfilemapRepository = srfilemapRepository;
            _passwordHasher = passwordHasher;
            _roleRepository = roleRepository;
            _securityRepository = securityRepository;
            _tokenAuthControler = tokenAuthControler;
            _hostingEnvironment = hostingEnvironment;
            _escrowDetailRepository = escrowDetailRepository;
            _IescrowDetailsAppService = IescrowDetailsAppService;
            _notificationPublisher = notificationPublisher;
            _srInvitationRecordRepository = srInvitationRecordRepository;
            _ISrEscrowRepository = ISrEscrowRepository;
            _ISrInvitationRecordsAppService = ISrInvitationRecordsAppService;
            _srEscrowsAppService = srEscrowsAppService;
            _esignRepository = esignRepository;
            _srAssignedFilesDetailRepository = srAssignedFilesDetailRepository;
            _srAssignedFilesDetailsAppService = srAssignedFilesDetailsAppService;
            _hub = hub;
            _unitOfWorkManager = unitOfWorkManager;
            _escrowFileHistoriesAppService = escrowFileHistoriesAppService;
            _srEscrowFileMasterRepository = srEscrowFileMasterRepository;
            _friendshipAppService = friendshipAppService;
        }

        ///<Summary>
        /// Index
        ///</Summary>
        [DisableAuditing]
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Ui");
        }

        ///<Summary>
        /// Upload create enterprise file
        ///</Summary>
        public responseData UploadCreateEnterpriseFile()
        {
            responseData temp = new responseData();
            try
            {

                var file = Request.Form.Files[0];

                return temp;
            }
            catch (Exception ex)
            {
                temp.message = "Upload Failed " + ex;
                temp.statusCode = 500;
                string logs = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");
                if (!System.IO.File.Exists(logs))
                {
                    FileStream fs1 = new FileStream(logs, FileMode.OpenOrCreate, FileAccess.Write);
                }
                StreamWriter writer = new StreamWriter(logs, true);
                writer.WriteLine("Error in UploadCreateEnterpriseFile method for -: error=" + ex.ToString() + DateTime.Now.ToString());
                writer.Close();
                return temp;
            }

        }

        ///<Summary>
        /// Upload CSV file
        ///</Summary>
        public responseData UploadCSV()
        {
            responseData res = new responseData();

            try
            {
                foreach (var file in Request.Form.Files)
                {
                    //var file = Request.Form.Files[0];
                    var folderName = Path.Combine(@"wwwroot\\Common\\CSVUpload");
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                    string destination = "", extension = "", imgfile = "";
                    string rootpath = conf["App:ServerRootAddress"].ToString();
                    if (Path.GetExtension(file.FileName) != ".csv" || Path.GetExtension(file.FileName) != ".txt")
                    {
                        destination = Path.Combine(_hostingEnvironment.WebRootPath + "\\Images\\");
                        extension = Path.GetExtension(file.FileName);
                        imgfile = Path.Combine(destination, file.FileName);
                        using (var stream = new FileStream(imgfile, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }
                    }
                    if (file.Length > 0)
                    {
                        string fileName = "Temp.csv";
                        var fullPath = Path.Combine(pathToSave, fileName);
                        var dbPath = Path.Combine(folderName, fileName);
                        if (!Directory.Exists(pathToSave))
                        {
                            Directory.CreateDirectory(pathToSave);
                        }
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }


                        var response = ConvertCSV(@"wwwroot\\Common\\CSVUpload\\" + fileName);
                        res.message = "Uploaded Successfully";
                        res = response;
                        res.statusCode = 200;

                        return res;

                    }
                    else
                    {
                        res.message = "Error while uploading";
                        res.statusCode = 500;
                        return res;
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                // res.CSVStatus = ex.Message;
                res.message = ex.Message;
                res.statusCode = 500;
                return res;
            }
        }

        ///<Summary>
        /// Create new escrow entry
        ///</Summary>
        [HttpPost]
        public IActionResult CreateEscrowThroughCommand(string CompanyName, string SubCompanyName, string Escrow, string PropertyAddress, string EscrowOfficerName, string EOEmail, string EOPhone, string EoPhoneExt, string EoPhoneCell, string logo)
        {
            CompanyName = ValidFileName(CompanyName);
            string message = String.Empty; bool record = true;
            try
            {

                CreateOrEditSrEscrowDto obj = new CreateOrEditSrEscrowDto();
                //GetAllSrEscrowsInput ob = new GetAllSrEscrowsInput();

                string destination = "", extension = "", imgfile = "";
                string rootpath = conf["App:ServerRootAddress"].ToString();
                var enterprises = _enterpriseRepository.GetAll().Where(x => x.EnterpriseName == CompanyName.Replace("\"", "").Trim() || x.Subcompany == SubCompanyName.Replace("\"", "").Trim()).FirstOrDefault();
                if (enterprises == null)
                {
                    return Json(new { Executed = record, message = CompanyName + " or " + SubCompanyName + " is not exists in the database" });
                }
                if (logo != "" || logo != null)
                {
                    destination = Path.Combine(_hostingEnvironment.WebRootPath + "\\Images\\");
                    if (Request.Form.Files.Count > 0)
                    {
                        var file = Request.Form.Files[0];
                        extension = Path.GetExtension(file.FileName);
                        //imgfile = Path.Combine(destination, SubCompanyName.Replace("\"", "").Replace(" ", "") + extension);
                        imgfile = Path.Combine(destination, SubCompanyName.Replace("\"", "") + extension);

                        if (!System.IO.File.Exists(imgfile))
                        {
                            using (var stream = new FileStream(imgfile, FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }
                        }
                        else
                        {
                            //using (var stream = new FileStream(imgfile, FileMode.Append))
                            //{
                            //    file.CopyTo(stream);
                            //}
                        }
                    }
                }

                //ob.EscrowNoFilter = Escrow.Replace("\"", "");
                var escrowdata = _ISrEscrowRepository.GetAll().Where(x => x.EscrowNo == Escrow.Replace("\"", "") && x.EnterpriseId == enterprises.Id && x.SubCompanyName == SubCompanyName.Replace("\"", "")).FirstOrDefault();
                if (enterprises != null)
                {
                    var white = SubCompanyName.Replace("\"", "").Trim();
                    if (escrowdata != null)
                    {
                        escrowdata.EnterpriseId = escrowdata.EnterpriseId;
                        obj.Id = escrowdata.Id;
                        escrowdata.SubCompanyName = escrowdata.SubCompanyName;
                        escrowdata.EOEmail = EOEmail.Replace("\"", "");
                        escrowdata.EOPhone = EOPhone.Replace("\"", "");
                        escrowdata.EoPhoneCell = EoPhoneCell.Replace("\"", "");
                        escrowdata.EoPhoneExt = EoPhoneExt.Replace("\"", "");
                        escrowdata.EscrowNo = escrowdata.EscrowNo;
                        escrowdata.EscrowOfficerName = EscrowOfficerName.Replace("\"", "");
                        escrowdata.PropertyAddress = PropertyAddress.Replace("\"", "");
                        //escrowdata.Logo = rootpath+"Images/"+ white.Replace(" ", "") + extension;
                        escrowdata.Logo = rootpath + "wwwroot/Images/" + white + extension;
                        var un = _ISrEscrowRepository.InsertOrUpdate(escrowdata);
                        var escrowdataemail = _ISrEscrowRepository.GetAll().Where(x => x.EOEmail == escrowdata.EOEmail).FirstOrDefault();
                        if (escrowdataemail != null)
                        {

                            MailMessage mail = new MailMessage();
                            mail.From = new MailAddress("Noreply@EscrowBasePortal.com");
                            mail.To.Add(escrowdataemail.EOEmail);
                            mail.Subject = "Escrow Email Already Exist";
                            string referer = conf["App:ClientRootAddress"].ToString();
                            mail.IsBodyHtml = true;
                            mail.Body = "Escrow Already Exist for Email Id : " + escrowdata.EOEmail;
                            SmtpClient SmtpServer = new SmtpClient();
                            SmtpServer.Port = 587;
                            SmtpServer.Credentials = new System.Net.NetworkCredential("office@mandavconsultancy.com", "aouownmhogfobzbc");
                            SmtpServer.Host = "smtp.gmail.com";
                            SmtpServer.EnableSsl = true;
                            SmtpServer.Send(mail);
                            message = "Escrow Already Exist for Email Id : " + escrowdata.EOEmail;
                            record = false;

                        }
                        else
                        {


                            record = true;
                            MailMessage mails = new MailMessage();
                            mails.From = new MailAddress("Noreply@EscrowBasePortal.com");
                            mails.To.Add(escrowdata.EOEmail);
                            mails.Subject = "Escrow Created";
                            string referers = conf["App:ClientRootAddress"].ToString();
                            mails.IsBodyHtml = true;
                            mails.Body = "Escrow Created successfully for Email Id : " + escrowdata.EOEmail + " Enterprise : " + escrowdata.SubCompanyName + " Escrow No " + escrowdata.EscrowNo;
                            SmtpClient SmtpServers = new SmtpClient();
                            SmtpServers.Port = 587;
                            SmtpServers.Credentials = new System.Net.NetworkCredential("office@mandavconsultancy.com", "aouownmhogfobzbc");
                            SmtpServers.Host = "smtp.gmail.com";
                            SmtpServers.EnableSsl = true;
                            SmtpServers.Send(mails);
                            string logs = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");
                            if (!System.IO.File.Exists(logs))
                            {
                                FileStream fs1 = new FileStream(logs, FileMode.OpenOrCreate, FileAccess.Write);
                            }
                            StreamWriter writer = new StreamWriter(logs, true);
                            writer.WriteLine("New Escrow Created for escrow no -: " + escrowdata.EscrowNo + " DateTime=" + DateTime.Now.ToString());
                            writer.Close();
                            message = "Escrow Created successfully for Email Id : " + escrowdata.EOEmail + " Enterprise : " + escrowdata.SubCompanyName + " Escrow No " + escrowdata.EscrowNo;
                        }
                    }
                    else
                    {
                        obj.EnterpriseId = enterprises.Id;
                        obj.SubCompanyName = SubCompanyName.Replace("\"", "").Trim();
                        obj.EOEmail = EOEmail.Replace("\"", "");
                        obj.EOPhone = EOPhone.Replace("\"", "");
                        obj.EoPhoneCell = EoPhoneCell.Replace("\"", "");
                        obj.EoPhoneExt = EoPhoneExt.Replace("\"", "");
                        obj.EscrowNo = Escrow.Replace("\"", "");
                        obj.EscrowOfficerName = EscrowOfficerName.Replace("\"", "");
                        obj.PropertyAddress = PropertyAddress.Replace("\"", "");
                        //obj.Logo = rootpath + "/Images/" + white.Replace(" ", "") + extension;
                        obj.Logo = rootpath + "/Images/" + white + extension;
                        var un = _srEscrowsAppService.CreateOrEdit(obj);
                        //string folderName = @"wwwroot\\Common\\Paperless\\" + enterprises.EnterpriseName.Replace("\"", "").Replace(" ", "") + "\\";
                        string folderName = @"wwwroot\\Common\\Paperless\\" + enterprises.EnterpriseName.Replace("\"", "") + "\\";

                        //string pathString = System.IO.Path.Combine(folderName, SubCompanyName.Replace("\"", "").Replace(" ","") + "\\" + Escrow.Replace("\"", "").Replace(" ", "")+"\\Other");
                        string pathString = System.IO.Path.Combine(folderName, SubCompanyName.Replace("\"", "") + "\\" + Escrow.Replace("\"", "") + "\\Other");


                        System.IO.Directory.CreateDirectory(pathString);
                        message = "Record inserted for " + CompanyName;
                        record = true;
                        var escrowdataemail = _ISrEscrowRepository.GetAll().Where(x => x.EOEmail == obj.EOEmail).FirstOrDefault();
                        if (escrowdataemail != null)
                        {

                            MailMessage mail = new MailMessage();
                            mail.From = new MailAddress("Noreply@EscrowBasePortal.com");
                            mail.To.Add(escrowdataemail.EOEmail);
                            mail.Subject = "Escrow Email Already Exist";
                            string referer = conf["App:ClientRootAddress"].ToString();
                            mail.IsBodyHtml = true;
                            mail.Body = "Escrow Already Exist for Email Id : " + obj.EOEmail;
                            SmtpClient SmtpServer = new SmtpClient();
                            SmtpServer.Port = 587;
                            SmtpServer.Credentials = new System.Net.NetworkCredential("office@mandavconsultancy.com", "aouownmhogfobzbc");
                            SmtpServer.Host = "smtp.gmail.com";
                            SmtpServer.EnableSsl = true;
                            SmtpServer.Send(mail);
                            message = "Escrow Already Exist for  Email Id : " + obj.EOEmail;

                        }

                        MailMessage mails = new MailMessage();
                        mails.From = new MailAddress("Noreply@EscrowBasePortal.com");
                        mails.To.Add(obj.EOEmail);
                        mails.Subject = "Escrow Created";
                        string referers = conf["App:ClientRootAddress"].ToString();
                        mails.IsBodyHtml = true;
                        mails.Body = "Escrow Created Sucessfuly for Email Id : " + obj.EOEmail + " Enterprise : " + obj.SubCompanyName + " Escrow No " + obj.EscrowNo;
                        SmtpClient SmtpServers = new SmtpClient();
                        SmtpServers.Port = 587;
                        SmtpServers.Credentials = new System.Net.NetworkCredential("office@mandavconsultancy.com", "aouownmhogfobzbc");
                        SmtpServers.Host = "smtp.gmail.com";
                        SmtpServers.EnableSsl = true;
                        SmtpServers.Send(mails);
                        string logs = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");
                        if (!System.IO.File.Exists(logs))
                        {
                            FileStream fs1 = new FileStream(logs, FileMode.OpenOrCreate, FileAccess.Write);
                        }
                        StreamWriter writer = new StreamWriter(logs, true);
                        writer.WriteLine("New Escrow Created for escrow no -: " + obj.EscrowNo + " DateTime=" + DateTime.Now.ToString());
                        writer.Close();
                        message = "Escrow Created Sucessfuly for   Email Id : " + obj.EOEmail + " Enterprise : " + obj.SubCompanyName + " Escrow No " + obj.EscrowNo;

                        //string seller = Path.Combine(pathString, @"\(0)0000aaaSeller Opening Documents.txt");
                        //string buyer = Path.Combine(pathString, @"\(0)0000aaaBuyer Opening Documents.txt");
                        //if (!System.IO.File.Exists(seller))
                        //{
                        //    FileStream fs1 = new FileStream(seller, FileMode.OpenOrCreate, FileAccess.Write);
                        //    FileStream fs2 = new FileStream(buyer, FileMode.OpenOrCreate, FileAccess.Write);
                        //} 
                        CreateOrEditSrFileMappingDto createOrEditSrFileMappingDto = new CreateOrEditSrFileMappingDto();

                        createOrEditSrFileMappingDto.EscrowiId = obj.EscrowNo;
                        createOrEditSrFileMappingDto.FileName = "Paperless\\" + enterprises.EnterpriseName + "\\" + obj.SubCompanyName + "\\" + obj.EscrowNo + "\\Other";
                        createOrEditSrFileMappingDto.IsActive = true;
                        createOrEditSrFileMappingDto.Action = "READ";
                        createOrEditSrFileMappingDto.UserId = 8;

                        var dbSrFileMapping = _srfilemapRepository.GetAll().Where(x => x.FileName == createOrEditSrFileMappingDto.FileName && x.Action == createOrEditSrFileMappingDto.Action).FirstOrDefault();
                        if (dbSrFileMapping == null)
                        {
                            _ISrFileMappingsAppService.CreateOrEdit(createOrEditSrFileMappingDto);
                        }

                        CreateNewEsrowmapping(enterprises, obj);
                    }
                    return Json(new { Executed = record, message = message });
                }
                return Json(new { Executed = record, message = "Enterprise doesn't exists" });
            }
            catch (Exception ex)
            {
                string logs = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");
                if (!System.IO.File.Exists(logs))
                {
                    FileStream fs1 = new FileStream(logs, FileMode.OpenOrCreate, FileAccess.Write);
                }
                StreamWriter writer = new StreamWriter(logs, true);
                writer.WriteLine("Error in CreateOrUpdate method for -: error=" + ex.ToString() + DateTime.Now.ToString());
                writer.Close();
                return Json(new { Executed = record, message = ex.Message.ToString() });
            }
        }

        #region
        ///<Summary>
        /// Convert CSV
        ///</Summary>
        ///
        private CreateOrEditSrFileMappingDto CreateNewEsrowmapping(Enterprise enterprises, CreateOrEditSrEscrowDto escrow)
        {
            var userAction = new string[] { "SRX","BRX","BR1","BR2","BR3","BR4","BR5","BR6","BR7","BR8","BR9","BR10",
"SR1","SR2","SR3","SR14","SR5","SR6","SR7","SR8","SR9","SR10",
"RAL","RBL","RAS","RBS","RAO","RBO","LR1","LR2","LR3","LP1","LP2",
"LP3","TCX","TCA","LBX","LBP","EO1","EA1","EOX","EAX","TC1","TC2",
"TC3","TC4","TC5","TC6","TC7","TC8","TC9","TC10","LTC","STC","OTC" };
            foreach (var action in userAction)
            {
                CreateOrEditSrFileMappingDto fileMapping = new CreateOrEditSrFileMappingDto();

                fileMapping.EscrowiId = escrow.EscrowNo;
                fileMapping.FileName = "Paperless\\" + enterprises.EnterpriseName + "\\" + escrow.SubCompanyName + "\\" + escrow.EscrowNo + "\\"; ;
                fileMapping.IsActive = true;
                fileMapping.Action = action;
                fileMapping.UserId = 8;
                var dbSrFileMappingTCX = _srfilemapRepository.GetAll().Where(x => x.FileName == fileMapping.FileName && x.Action == fileMapping.Action).FirstOrDefault();
                if (dbSrFileMappingTCX == null)
                {
                    _ISrFileMappingsAppService.CreateOrEdit(fileMapping);
                }
            }
            return null;
        }

        private responseData ConvertCSV(string fileName)
        {
            responseData res = new responseData();

            //string error = "";
            try
            {
                using (TextReader reader = System.IO.File.OpenText(fileName))
                {
                    CsvReader csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture);
                    csv.Configuration.Delimiter = ";";
                    csv.Configuration.MissingFieldFound = null;
                    csv.Configuration.HeaderValidated = null;
                    List<enterpriseCreate> listData = new List<enterpriseCreate>();
                    while (csv.Read())
                    {
                        enterpriseCreate lst = csv.GetRecord<enterpriseCreate>();
                        enterpriseCreate obj = new enterpriseCreate();

                        if (!string.IsNullOrWhiteSpace(lst.EnterpriseName))
                        {
                            obj.EnterpriseName = EncryptionUtility.decrypt(lst.EnterpriseName.Replace("%1%", "\""), "test");
                        }

                        if (!string.IsNullOrWhiteSpace(lst.EnterpriseExt))
                        {
                            obj.EnterpriseExt = EncryptionUtility.decrypt(lst.EnterpriseExt.Replace("%1%", "\""), "test");
                        }

                        if (!string.IsNullOrWhiteSpace(lst.EnterpriseExtFlag))
                        {
                            obj.EnterpriseExtFlag = EncryptionUtility.decrypt(lst.EnterpriseExtFlag.Replace("%1%", "\""), "test");
                        }

                        if (!string.IsNullOrWhiteSpace(lst.Phone))
                        {
                            obj.Phone = EncryptionUtility.decrypt(lst.Phone.Replace("%1%", "\""), "test");
                        }

                        if (!string.IsNullOrWhiteSpace(lst.PrimaryContact))
                        {
                            obj.PrimaryContact = EncryptionUtility.decrypt(lst.PrimaryContact.Replace("%1%", "\""), "test");
                        }

                        if (!string.IsNullOrWhiteSpace(lst.PrimaryContactCellNo))
                        {
                            obj.PrimaryContactCellNo = EncryptionUtility.decrypt(lst.PrimaryContactCellNo.Replace("%1%", "\""), "test");
                        }

                        if (!string.IsNullOrWhiteSpace(lst.AlternateEnterpriseName))
                        {
                            obj.AlternateEnterpriseName = EncryptionUtility.decrypt(lst.AlternateEnterpriseName.Replace("%1%", "\""), "test");
                        }

                        if (!string.IsNullOrWhiteSpace(lst.BrokerName))
                        {
                            obj.BrokerName = EncryptionUtility.decrypt(lst.BrokerName.Replace("%1%", "\""), "test");
                        }

                        if (!string.IsNullOrWhiteSpace(lst.CorporateName))
                        {
                            obj.CorporateName = EncryptionUtility.decrypt(lst.CorporateName.Replace("%1%", "\""), "test");
                        }

                        if (!string.IsNullOrWhiteSpace(lst.OfficePhone))
                        {
                            obj.OfficePhone = EncryptionUtility.decrypt(lst.OfficePhone.Replace("%1%", "\""), "test");
                        }

                        if (!string.IsNullOrWhiteSpace(lst.OfficeFax))
                        {
                            obj.OfficeFax = EncryptionUtility.decrypt(lst.OfficeFax.Replace("%1%", "\""), "test");
                        }

                        if (!string.IsNullOrWhiteSpace(lst.SecondaryEnterpriseEmail))
                        {
                            obj.SecondaryEnterpriseEmail = EncryptionUtility.decrypt(lst.SecondaryEnterpriseEmail.Replace("%1%", "\""), "test");
                        }

                        if (!string.IsNullOrWhiteSpace(lst.DisclosureVerbage))
                        {
                            obj.DisclosureVerbage = EncryptionUtility.decrypt(lst.DisclosureVerbage.Replace("%1%", "\""), "test");
                        }

                        if (!string.IsNullOrWhiteSpace(lst.LicenseVerbiage))
                        {
                            obj.LicenseVerbiage = EncryptionUtility.decrypt(lst.LicenseVerbiage.Replace("%1%", "\""), "test");
                        }
                        if (!string.IsNullOrWhiteSpace(lst.DefaultRealtor))
                        {
                            obj.DefaultRealtor = EncryptionUtility.decrypt(lst.DefaultRealtor.Replace("%1%", "\""), "test");
                        }

                        if (!string.IsNullOrWhiteSpace(lst.DefaultMbroker))
                        {
                            obj.DefaultMbroker = EncryptionUtility.decrypt(lst.DefaultMbroker.Replace("%1%", "\""), "test");
                        }

                        if (!string.IsNullOrWhiteSpace(lst.DefaultTitle))
                        {
                            obj.DefaultTitle = EncryptionUtility.decrypt(lst.DefaultTitle.Replace("%1%", "\""), "test");
                        }

                        if (!string.IsNullOrWhiteSpace(lst.DefaultRefi))
                        {
                            obj.DefaultRefi = EncryptionUtility.decrypt(lst.DefaultRefi.Replace("%1%", "\""), "test");
                        }

                        if (!string.IsNullOrWhiteSpace(lst.DefaultTitle))
                        {
                            obj.TaxPayerID = EncryptionUtility.decrypt(lst.TaxPayerID.Replace("%1%", "\""), "test");
                        }

                        if (!string.IsNullOrWhiteSpace(lst.LicenesNo))
                        {
                            obj.LicenesNo = EncryptionUtility.decrypt(lst.LicenesNo.Replace("%1%", "\""), "test");
                        }

                        if (!string.IsNullOrWhiteSpace(lst.Subcompany))
                        {
                            obj.Subcompany = EncryptionUtility.decrypt(lst.Subcompany.Replace("%1%", "\""), "test");
                        }
                        listData.Add(obj);
                    }
                    res = createOrUpdate(listData);
                }
                System.IO.File.Delete(fileName);
            }
            catch (Exception ex)
            {
                System.IO.File.Delete(fileName);
                res.message = ex.Message;
            }
            return res;
        }

        ///<Summary>
        /// Upload invites users with thier files
        ///</Summary>`
        public responseData createOrUpdate(List<enterpriseCreate> item)
        {
            responseData res = new responseData();
            foreach (var Record in item)
            {
                char[] spearator = { '/', ' ' };
                Int32 count = 2;
                var parent = "";
                string enterpriseId = "";


                string[] strlist = Record.EnterpriseName.Split(spearator,
                       count, StringSplitOptions.None);
                if (strlist.Length == 2)
                {
                    string enterprise = strlist[0];
                    string child = strlist[1];
                    var getParent = _enterpriseRepository.GetAll().Where(x => x.EnterpriseName == enterprise).FirstOrDefault();
                    if (getParent != null)
                    {
                        parent = getParent.Id.ToString();
                        enterpriseId = getParent.Subcompany;
                        Record.EnterpriseName = child;
                    }

                }



                var find = _enterpriseRepository.FirstOrDefault(e => e.EnterpriseName == Record.EnterpriseName);
                if (find != null)
                {
                    try
                    {
                        res.message += Record.EnterpriseName + "Already Exist" + "\n";
                        res.updatedRecord++;
                        res.totalRecord++;
                        continue;

                    }
                    catch (Exception ex)
                    {
                        string logs = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");
                        if (!System.IO.File.Exists(logs))
                        {
                            FileStream fs1 = new FileStream(logs, FileMode.OpenOrCreate, FileAccess.Write);
                        }
                        StreamWriter writer = new StreamWriter(logs, true);
                        writer.WriteLine("Error in CreateOrUpdate method for -: error=" + ex.ToString() + DateTime.Now.ToString());
                        writer.Close();

                    }

                }
                else
                {
                    try
                    {
                        Enterprise obj = new Enterprise();
                        if (string.IsNullOrWhiteSpace(Record.EnterpriseName))
                        {
                            res.message += "Enterprise Name Required" + "\n";
                            res.totalRecord++;
                            continue;
                        }
                        obj.EnterpriseName = Record.EnterpriseName;
                        if (string.IsNullOrWhiteSpace(Record.EnterpriseExt))
                        {
                            res.message += "EnterpriseExt Name Required" + "\n";
                            res.totalRecord++;
                            continue;
                        }
                        obj.EnterpriseExt = Record.EnterpriseExt;
                        obj.EnterpriseExtFlag = Record.EnterpriseExtFlag;
                        if (string.IsNullOrWhiteSpace(Record.Phone))
                        {
                            res.message += "Phone Number Required" + "\n";
                            res.totalRecord++;
                            continue;
                        }
                        obj.PrimaryContact = Record.Phone;
                        obj.PrimaryContactCellNo = Record.PrimaryContactCellNo;
                        obj.AlternateEnterpriseName = Record.AlternateEnterpriseName;
                        obj.BrokerName = Record.BrokerName;
                        obj.CorporateName = Record.CorporateName;
                        obj.OfficePhone = Record.OfficePhone;
                        obj.OfficeFax = Record.OfficeFax;
                        obj.SecondaryEnterpriseEmail = Record.SecondaryEnterpriseEmail;
                        obj.DisclosureVerbage = Record.DisclosureVerbage;
                        obj.LicenseVerbiage = Record.LicenseVerbiage;
                        obj.DefaultRealtor = Record.DefaultRealtor;
                        obj.DefaultMbroker = Record.DefaultMbroker;
                        obj.DefaultTitle = Record.DefaultTitle;
                        obj.DefaultRefi = Record.DefaultRefi;
                        obj.TaxPayerID = Record.TaxPayerID;
                        obj.LicenesNo = Record.LicenesNo;
                        obj.Subcompany = Record.Subcompany;
                        obj.Logo = Record.Logo;
                        if (parent != "")
                        {
                            obj.ParentId = int.Parse(parent);
                        }
                        var lastId = _enterpriseRepository.GetAll().ToList();
                        if (lastId.Count > 0)
                        {
                            obj.EnterpriseId = (int.Parse(lastId.LastOrDefault().EnterpriseId) + 1).ToString();
                            if (obj.EnterpriseId.Length == 1)
                            {
                                obj.EnterpriseId = "000" + obj.EnterpriseId;
                            }
                            if (obj.EnterpriseId.Length == 2)
                            {
                                obj.EnterpriseId = "00" + obj.EnterpriseId;
                            }
                            if (obj.EnterpriseId.Length == 3)
                            {
                                obj.EnterpriseId = "0" + obj.EnterpriseId;
                            }
                        }
                        else
                        {
                            obj.EnterpriseId = "0001";
                        }

                        _enterpriseRepository.InsertAndGetId(obj);
                        res.message += Record.EnterpriseName + " Created Successfully" + "\n";
                        res.addedRecord++;
                        res.totalRecord++;
                        //Main(obj.EnterpriseName, enterpriseId);
                        Main(obj.EnterpriseName, obj.EnterpriseName);
                        Main(obj.EnterpriseName, obj.Subcompany);
                    }
                    catch (Exception ex)

                    {
                        res.message += ex.Message + "\n";
                        res.faildRecord++;
                        res.totalRecord++;
                    }
                }
            }
            return res;
        }

        ///<Summary>
        /// Create enterprise folders
        ///</Summary>
        static void Main(string name, string child)
        {
            if (child != null && child != "")
            {

                name = ValidFileName(name);
                //Create Folder Name
                string folderName = @"wwwroot\\Common\\Paperless\\" + name;//.Replace(" ","") + "\\";
                // folderName = ValidFileName(folderName);
                //folderName = ValidFileName(name);
                child = ValidFileName(child);
                //string pathString = System.IO.Path.Combine(folderName, child.Replace(" ", ""));
                string pathString = System.IO.Path.Combine(folderName, child);
                //pathString = ValidFileName(child);

                System.IO.Directory.CreateDirectory(pathString);
            }
            else
            {
                string folderName = @"wwwroot\\Common\\Paperless\\";

                string pathString = System.IO.Path.Combine(folderName, name);//.Replace(" ", ""));


                System.IO.Directory.CreateDirectory(pathString);


            }

        }

        public static string ValidFileName(string Vfn)
        {
            // First replace invalid Charcter symbols for file names with normal ascii set with similar acceptable charcters
            Vfn = Strings.Replace(Vfn, "<", "(");
            Vfn = Strings.Replace(Vfn, ">", ")");
            Vfn = Strings.Replace(Vfn, ":", ";");
            Vfn = Strings.Replace(Vfn, "*", "'");
            Vfn = Strings.Replace(Vfn, "/", "-");
            Vfn = Strings.Replace(Vfn, @"\", "=");
            Vfn = Strings.Replace(Vfn, "|", "_");
            Vfn = Strings.Replace(Vfn, "?", "+");
            Vfn = Strings.Replace(Vfn, "*", ".");
            // Vfn = Strings.Replace(Vfn, ".", "DOT");
            if (Vfn != null)
            {
                char str = Vfn[Vfn.Length - 1];
                if (str.ToString() == ".")
                {
                    Vfn = Vfn.Substring(0, Vfn.LastIndexOf('.'));
                }
            }

            //int length = Vfn.Length - Vfn.IndexOf("1") - 1;

            //string sub = Vfn.Substring(0, length);

            return Vfn;
        }


        ///<Summary>
        /// Get Images
        ///</Summary>
        public IEnumerable<FilePath> GetImages()
        {
            List<FilePath> files = new List<FilePath>();
            DirectoryInfo dirInfo = new DirectoryInfo(@"wwwroot\\Common\\Paperless\\0001\\0002\\");

            foreach (FileInfo fInfo in dirInfo.GetFiles())
            {
                files.Add(new FilePath() { path = fInfo.Name });
            }

            return files.ToList();
        }

        ///<Summary>
        /// Transfer file one directory to another
        ///</Summary>
        public async Task<string> UploadFileFolderEnterprise(string enterpriseFolder)
        {

            var folderName = "";
            string status = "";
            string newstr = String.Empty;
            string stresoff = String.Empty;
            string Filenamest = "";
            string textfile = "";
            List<string> allLinesText;
            List<string> set = new List<string>();
            string Timestamp = DateTime.Now.ToString("MM-dd-yyyy-HH-mm-ss-FFFFF");
            folderName = Path.Combine(@"wwwroot\Common\Paperless");
            try
            {
                responseData res = new responseData();
                foreach (var file in Request.Form.Files.OrderBy(c => c.FileName.Contains(".txt") ? 0 : 1))
                {
                    string source = Convert.ToString(file.FileName);
                    string fileName = System.IO.Path.GetFileName(file.FileName);
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                    string fileEx = System.IO.Path.GetExtension(file.FileName);
                    string filewithoutEx = System.IO.Path.GetFileNameWithoutExtension(file.FileName);
                    Filenamest = source.Substring(source.LastIndexOf("\\") + 1);
                    string destpath = String.Empty;

                    if (fileEx == ".txt")
                    {
                        textfile = Path.Combine(folderName, filewithoutEx + Timestamp.ToString() + ".txt");
                        if (Directory.Exists(textfile))
                        {
                            System.IO.File.Delete(textfile);
                        }
                        using (var stream = new FileStream(textfile, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        int count = 0;
                        string linex;
                        var filx = new System.IO.StreamReader(textfile).ReadToEnd();
                        var lines = filx.Split(new char[] { '\n' });
                        int counting = lines.Length;

                        System.IO.StreamReader filex = new System.IO.StreamReader(textfile);

                        while ((linex = filex.ReadLine()) != null)
                        {
                            System.Console.WriteLine(linex);
                            allLinesText = System.IO.File.ReadAllLines(textfile).ToList();
                            if (!linex.Contains("Email, User Name, Company, Invitee, Esc#, Access Types"))
                            {
                                string get = "";
                                var reg = new Regex("(\\\")(.*?)(\\\")", RegexOptions.Compiled);
                                List<string> list = new List<string>();
                                foreach (System.Text.RegularExpressions.Match match in reg.Matches(linex))
                                { list.Add(match.Value.Replace("\"", "")); }
                                if (list.Count > 2)
                                {
                                    for (int l = 0; l < list.Count; l++)
                                    {
                                        if (l == 0)
                                        {

                                            get = list[0];
                                        }
                                        if (l == 1)
                                        {
                                            //two = list[1];
                                        }
                                        if (l == 2)
                                        {
                                            {
                                                get += ";" + list[2];
                                            }
                                            sttrenterprise = ValidFileName(list[2]);
                                        }
                                        if (l == 3)
                                        {
                                            stresoff = list[3];
                                        }
                                        if (l == 4)
                                        {
                                            get += ";" + list[4];
                                            sttrescro = list[4];
                                        }
                                        if (l == 5)
                                        {
                                            var Ent = _enterpriseRepository.GetAll();//.Where(x => x.EnterpriseId == sttrenterprise || ValidFileName(x.EnterpriseName) == sttrenterprise || x.Subcompany == sttrenterprise).FirstOrDefault();

                                            foreach (var item in Ent)
                                            {
                                                if (item.EnterpriseId == sttrenterprise || ValidFileName(item.EnterpriseName) == sttrenterprise || item.Subcompany == sttrenterprise)
                                                {
                                                    get += ";" + item.EnterpriseName;
                                                }
                                            }


                                        }
                                        if (l == 6)
                                        {
                                        }
                                    }

                                    set.Add(get);
                                }
                                else
                                {
                                    status += "Warning : skipped the empty row found in text file";
                                }
                                count++;
                            }


                        }
                    }
                    #region

                    string nameofenterprise = String.Empty;
                    string part1 = sttrescro.Trim();
                    string part2 = sttrenterprise.Trim();
                    var enterprises = _enterpriseRepository.GetAll(); //.Where(x => x.EnterpriseId == part2 || ValidFileName(x.EnterpriseName == part2 || x.Subcompany == part2).FirstOrDefault();
                    Enterprise eterprise = null;
                    foreach (var ent in enterprises)
                    {
                        if (ent.EnterpriseId == part2 || ValidFileName(ent.EnterpriseName) == ValidFileName(part2) || ent.Subcompany == part2)
                        {
                            eterprise = ent;
                        }
                    }
                    if (eterprise == null)
                    {
                        status += "Enterprise Not found";
                        continue;
                    }
                    nameofenterprise = ValidFileName(eterprise.EnterpriseName); //nameofenterprise.Replace(" ", "");
                    string noofenterprise = eterprise.EnterpriseId;
                    string finalpartstring = "";
                    var despath = "";
                    if (set.Count > 0)
                    {
                        for (int i = 0; i < set.Count; i++)
                        {
                            var data = set[i].Split(new string[] { ";" }, StringSplitOptions.None);
                            //finalpartstring = ValidFileName(data[3].Replace(" ", "")) + "\\" + ValidFileName(data[1].Replace(" ", "")) + "\\" + data[2];
                            finalpartstring = ValidFileName(data[3]) + "\\" + ValidFileName(data[1]) + "\\" + data[2];

                            despath = Path.Combine(pathToSave);
                            if (destpath != "")
                            {
                                destpath += "," + Path.Combine("((" + data[0] + "))" + despath, finalpartstring);
                            }
                            else
                            {
                                destpath = Path.Combine("((" + data[0] + "))" + despath, finalpartstring);
                            }
                        }
                    }

                    char[] separator = { ',' };
                    string[] strlist = destpath.Split(separator, StringSplitOptions.None);
                    string tpath = Path.Combine(despath, noofenterprise);
                    for (int i = 0; i < strlist.Length; i++)
                    {
                        var trim = Regex.Replace(strlist[i], @"\(\([^()]*\)\)", string.Empty);
                        trim = trim.Replace("()", "");
                        if (Directory.Exists(tpath))
                        {
                            Directory.Move(tpath, nameofenterprise);
                            Directory.CreateDirectory(trim + "\\Other");
                        }
                        else
                        {
                            Directory.CreateDirectory(trim + "\\Other");
                        }
                    }

                    #endregion
                    if (file.Length > 0)
                    {
                        string escrowNumber = Path.GetFileName(Path.GetDirectoryName(file.FileName));

                        CreateOrEditEscrowClientDto obj = new CreateOrEditEscrowClientDto();
                        obj.EscrowNumber = escrowNumber;
                        escrownumber = escrowNumber;

                        string realfilename = String.Empty;
                        string fileExten = System.IO.Path.GetExtension(file.FileName);
                        char[] separ = { ',' };
                        string[] strlis = destpath.Split(separ, StringSplitOptions.None);
                        if (strlis.Length > 0)
                        {
                            for (int xc = 0; xc < strlis.Length; xc++)
                            {
                                var trim = Regex.Replace(strlis[xc], @"\(\([^()]*\)\)", string.Empty);
                                trim = trim.Replace("()", "");
                                realfilename = filewithoutEx + Timestamp.ToString() + "(" + stresoff + ")" + ".pdf";
                                if (fileExten == ".pdf" && !System.IO.File.Exists(Path.Combine(trim, realfilename)))
                                {
                                    string changedname = Path.Combine(trim, realfilename);
                                    using (var stream = new FileStream(changedname, FileMode.Create))
                                    {
                                        file.CopyTo(stream);
                                    }
                                }
                            }
                        }

                        if (fileExten == ".pdf")
                        {
                            char[] sepa = { ',' };
                            string[] strl = destpath.Split(sepa, StringSplitOptions.None);

                            for (int i = 0; i < strl.Length; i++)
                            {
                                fulldestfile = strl[i];
                                pdfname = file.FileName;
                                if (finalstring != "")
                                {
                                    string newstr1 = Path.Combine(fulldestfile, realfilename);
                                    finalstring += "%$%" + newstr1;
                                }
                                else
                                {
                                    finalstring = Path.Combine(fulldestfile, realfilename);
                                }
                            }
                        }
                        else
                        {
                            fulldestfile = destpath;
                            string fileExte = System.IO.Path.GetExtension(file.FileName);
                            if (fileExte == ".pdf")
                            {
                                fulldestfile = destpath;
                                pdfname = file.FileName;
                                if (finalstring != "")
                                {
                                    string newstr1 = Path.Combine(fulldestfile, realfilename);
                                    finalstring += "%$%" + newstr1;
                                }
                                else
                                {
                                    finalstring = Path.Combine(fulldestfile, realfilename);
                                }
                            }
                        }


                    }
                }
                if (folderName != "")
                {
                    if (Path.GetFileName(textfile) == "InviteUser" + Timestamp + ".txt")
                    {
                        var d = await ReadTextFile(textfile);
                        status += d;
                    }
                    else if (Path.GetFileName(textfile) != "InviteUser" + Timestamp + ".txt")
                    {
                        var err = "Error : File not found";
                        return err.ToString();
                    }
                }

            }
            catch (Exception ex)
            {
                string logs = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");
                if (!System.IO.File.Exists(logs))
                {
                    FileStream fs1 = new FileStream(logs, FileMode.OpenOrCreate, FileAccess.Write);
                }
                StreamWriter writer = new StreamWriter(logs, true);
                writer.WriteLine("Error in UploadFileFolderEnterprise method for -: error = " + ex.ToString() + DateTime.Now.ToString());
                writer.Close();
                return ex.Message;
            }


            return status;

        }
        ///<Summary>
        /// Verify phone number when first login
        /// 
        ///</Summary>
        public string verifyPhoneNumber(string uid, string n, string un, string phoneno, string code)
        {
            User user = new User();
            string res = "", pno = "";
            try
            {
                string itgr = "";

                if (!string.IsNullOrWhiteSpace(code))
                {
                    var usr = _userRepository.GetAll().Where(x => x.UserName == un).FirstOrDefault();
                    if (usr != null)
                    {
                        usr.IsPhoneNumberConfirmed = true;
                        _userRepository.InsertOrUpdate(usr);
                        return usr.EmailConfirmationCode;
                    }
                    return null;
                }
                else
                {
                    if (phoneno != "undefined" && phoneno != "" && phoneno != null)
                    {
                        Random rnd = new Random();
                        int _min = 111111;
                        int _max = 999999;
                        string AccountSid = conf["Twilio:AccountSid"].ToString();
                        string AuthToken = conf["Twilio:AuthToken"].ToString();
                        string MessagingServiceSid = conf["Twilio:MessagingServiceSid"].ToString();
                        string SenderNumber = conf["Twilio:SenderNumber"].ToString();
                        itgr = rnd.Next(_min, _max).ToString();
                        if (itgr != "")
                        {
                            var cc = ConfigurationManager.AppSettings["CountryCode"];
                            TwilioClient.Init(AccountSid, AuthToken);
                            var User = new UserEditDto();
                            CreateOrUpdateUserInput cuu = new CreateOrUpdateUserInput();
                            if (!phoneno.Contains("+1"))
                            {
                                pno = "+1" + phoneno.Trim();
                            }
                            var messageOptions = new CreateMessageOptions(
                                new PhoneNumber(pno));
                            //messageOptions.MessagingServiceSid = MessagingServiceSid;
                            messageOptions.Body = n + " Your verification code for EscrowBase SignUp is: " + itgr + " Email For This Account : " + un;
                            messageOptions.From = SenderNumber;
                            User.PhoneNumber = pno;
                            GetUsersInput input = new GetUsersInput();
                            input.Filter = un;
                            input.MaxResultCount = 1000;
                            var ch = _IUserAppService.GetUsersFilteredQuery(input).FirstOrDefault();
                            if (ch != null)
                            {
                                User.UserName = ch.UserName;
                                User.EmailAddress = ch.EmailAddress;
                                User.IsActive = ch.IsActive;
                                User.PasswordResetCode = ch.PasswordResetCode;
                                User.Name = ch.Name;
                                User.Surname = ch.Surname;
                                User.EmailConfirmationCode = itgr.ToString();
                                User.Id = ch.Id;
                                cuu.User = User;
                                var chh = _IUserAppService.CreateOrUpdateUser(cuu);
                            }
                            var message = MessageResource.Create(messageOptions);
                            res = message.Status.ToString();
                            return res;
                        }
                    }
                    return "phone no empty";
                }
            }
            catch (Exception ex)
            {
                res = ex.Message.ToString();
                return res;
            }
        }

        ///<Summary>
        /// Send change password verification code
        ///</Summary>
        public async Task<responseData> forgotPassword(string uid, string un, string phoneno, string code, string status)
        {
            User user = new User();
            string pno = "";
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            string IPAddress = GetIPAddress();
            responseData res = new responseData();
            try
            {
                string itgr = "";
                if (status == "Code")
                {
                    LogMessage("forgotPassword CodeOtp", "called  to genrate the otp");
                    var User = new UserEditDto();
                    CreateOrUpdateUserInput cuu = new CreateOrUpdateUserInput();

                    GetUsersInput input = new GetUsersInput();
                    input.Filter = un;
                    input.MaxResultCount = 100000;
                    var ch = _IUserAppService.GetUsersFilteredQuery(input).FirstOrDefault();
                    if (ch.BlockAttemptsTill != DateTime.MinValue && ch.BlockAttemptsTill != null)
                    {
                        if (DateTime.Now < ch.BlockAttemptsTill && ch.UserIP == IPAddress)
                        {
                            res.message = "You Exceeded Attempts limit please try after 24 hours";
                            return res;
                        }
                        else
                        {
                            User.BlockAttemptsTill = DateTime.MinValue;
                        }

                    }
                    if (ch.AttemptsCount == "4")
                    {
                        User.BlockAttemptsTill = DateTime.Now.AddHours(24);
                    }
                    if (ch.AttemptsCount == "5")
                    {
                        res.message = "You Exceeded Attempts limit please try after 24 hours";
                        return res;
                    }
                    LogMessage("forgotPassword CodeOtp", "calling Send Mail method");
                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress("Noreply@EscrowBasePortal.com");
                    mail.To.Add(un);
                    mail.Subject = "Verification code";
                    string referer = conf["App:ClientRootAddress"].ToString();
                    Random rnd = new Random();
                    int _min = 111111;
                    int _max = 999999;
                    itgr = rnd.Next(_min, _max).ToString();
                    if (itgr != "")
                    {
                        mail.IsBodyHtml = true;
                        mail.Body = "Your verification code is: " + itgr;
                        SmtpClient SmtpServer = new SmtpClient();
                        SmtpServer.Port = 587;
                        SmtpServer.Credentials = new System.Net.NetworkCredential("office@mandavconsultancy.com", "aouownmhogfobzbc");
                        SmtpServer.Host = "smtp.gmail.com";
                        SmtpServer.EnableSsl = true;
                        SmtpServer.Send(mail);
                        LogMessage("forgotPassword CodeOtp", "Send Mail Sent");

                        if (ch.IsPhoneNumberConfirmed)
                        {
                            LogMessage("forgotPassword CodeOtp", "Send OTP to Phone ");
                            var CC = ConfigurationManager.AppSettings["CountryCode"];
                            if (!string.IsNullOrWhiteSpace(phoneno))
                            {
                                if (!phoneno.Contains("+1"))
                                {
                                    pno = CC + phoneno.Trim();
                                }
                                else
                                {
                                    pno = "+" + phoneno.Trim();
                                }
                                string accountSid = conf["Twilio:AccountSid"].ToString();
                                string authToken = conf["Twilio:AuthToken"].ToString();
                                string MessagingServiceSid = conf["Twilio:MessagingServiceSid"].ToString();
                                TwilioClient.Init(accountSid, authToken);
                                var messageOptions = new CreateMessageOptions(
                                    new PhoneNumber(pno));
                                messageOptions.MessagingServiceSid = MessagingServiceSid;
                                messageOptions.Body = "Your verification code is: " + itgr;
                                var message = MessageResource.Create(messageOptions);
                                res.message = message.Status.ToString();
                                LogMessage("forgotPassword CodeOtp", "Send OTP to Phone Sent ");

                            }

                        }
                        User.UserIP = IPAddress;
                        if (Convert.ToInt32(ch.AttemptsCount) >= 1)
                        {
                            User.AttemptsCount = ch.AttemptsCount + 1;
                        }
                        else
                        {
                            User.AttemptsCount = "1";
                        }
                        User.PhoneNumber = ch.PhoneNumber;
                        User.UserName = ch.EmailAddress;
                        User.EmailAddress = ch.EmailAddress;
                        User.IsActive = true;
                        User.Name = ch.Name;
                        User.Surname = ch.Surname;
                        User.EmailConfirmationCode = itgr.ToString();
                        User.Id = Convert.ToInt32(uid);
                        cuu.User = User;
                        await _IUserAppService.CreateOrUpdateUser(cuu);

                        LogMessage("forgotPassword CodeOtp", "Update user details");

                        return res;
                    }
                    else
                    {
                        res.message = "Something went wrong";
                        return res;
                    }
                }
                else if (status == "Reset")
                {
                    var User = new UserEditDto();
                    string Mailid = conf["Email:Mailid"].ToString();
                    string MailPwd = conf["Email:MailPwd"].ToString();
                    string MailHost = conf["Email:MailHost"].ToString();
                    int MailPort = Convert.ToInt32(conf["Email:MailPort"].ToString());
                    bool MailSSL = Convert.ToBoolean(conf["Email:MailSSL"].ToString());
                    CreateOrUpdateUserInput cuu = new CreateOrUpdateUserInput();
                    LogMessage("forgotPassword Reset", "get varification code from DB");
                    MailMessage mail = new MailMessage();
                    var vc = _userRepository.GetAll().Where(x => x.Id == Convert.ToInt32(uid)).FirstOrDefault();
                    string rres = vc.EmailConfirmationCode;
                    if (rres == code)
                    {

                        LogMessage("forgotPassword Reset", " varification code matched with DB");
                        mail.From = new MailAddress("Noreply@EscrowBasePortal.com");
                        mail.To.Add(un);
                        mail.Subject = "Verification code";
                        string referer = conf["App:ClientRootAddress"].ToString();

                        if (referer != null)
                        {

                            //var token = _tokenAuthControler.ExternalAuthenticate(un).Result;
                            int length = 10;

                            GetUsersInput input = new GetUsersInput();
                            input.Filter = un;
                            LogMessage("forgotPassword Reset", " GetUsersFilteredQuery  called");
                            var ch = _IUserAppService.GetUsersFilteredQuery(input).FirstOrDefault();

                            LogMessage("forgotPassword Reset", " GetUsersFilteredQuery  completed");
                            LogMessage("forgotPassword Reset", " Mail funtion started  ");
                            mail.IsBodyHtml = true;
                            byte[] a = System.Text.ASCIIEncoding.ASCII.GetBytes(Convert.ToString(ch.Id));
                            string encryptedUserID = Convert.ToBase64String(a);
                            byte[] c = System.Text.ASCIIEncoding.ASCII.GetBytes(Convert.ToString(ch.EmailAddress));
                            string encryptedun = Convert.ToBase64String(c);
                            const string valid = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
                            StringBuilder ress = new StringBuilder();
                            Random rndd = new Random();
                            while (0 < length--)
                            {
                                ress.Append(valid[rndd.Next(valid.Length)]);
                            }


                            mail.Subject = "Escrowbase Password Reset Link";
                            mail.Body = "Go to this link to reset your password: " + referer + "account/reset-password?ui=" + encryptedUserID + "&rc=" + ress.ToString() + "&un=" + encryptedun + "";
                            SmtpClient SmtpServer = new SmtpClient();
                            SmtpServer.Port = MailPort;
                            SmtpServer.Credentials = new System.Net.NetworkCredential(Mailid, MailPwd);
                            SmtpServer.Host = MailHost;
                            SmtpServer.EnableSsl = MailSSL;
                            SmtpServer.Send(mail);

                            LogMessage("forgotPassword Reset", " Mail Sent  ");
                            User.PhoneNumber = ch.PhoneNumber;
                            User.UserName = ch.EmailAddress;
                            User.EmailAddress = ch.EmailAddress;
                            User.IsActive = true;
                            User.Name = ch.Name;
                            User.Surname = ch.Surname;
                            User.PasswordResetCode = ress.ToString();
                            User.SignInTokenExpireTimeUtc = DateTime.Now.AddHours(24);
                            User.EmailConfirmationCode = ch.EmailConfirmationCode;
                            User.Id = ch.Id;
                            cuu.User = User;
                            await _IUserAppService.CreateOrUpdateUser(cuu);

                            LogMessage("forgotPassword Reset", " Update user Info  ");
                            res.message = "forgotPassword Reset Mail Sent  ";
                            res.statusCode = 200;
                            return res;

                        }

                    }

                    else
                    {
                        res.message = "Wrong Verification Code";
                        LogMessage("forgotPassword  ", "Wrong Verification Code");
                        return res;
                    }
                }
                res.message = "Something went wrong";
                return res;
            }
            catch (Exception ex)
            {
                res.message = ex.ToString();
                LogMessage("forgotPassword  ", ex.Message);
                return res;
            }
        }

        ///<Summary>
        /// Get IP address of user
        ///</Summary>
        public string GetIPAddress()
        {
            string IPAddress = "";
            IPHostEntry Host = default(IPHostEntry);
            string Hostname = null;

            Hostname = System.Environment.MachineName;
            Host = Dns.GetHostEntry(Hostname);
            foreach (IPAddress IP in Host.AddressList)
            {
                if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    IPAddress = Convert.ToString(IP);
                }
            }
            return IPAddress;
        }

        ///<Summary>]
        /// Get CompanyEscrow Details
        ///</Summary>
        public List<UserCompany> GetUserCompanyDetails(string username)
        {

            try
            {
                List<UserCompany> getComp = new List<UserCompany>();
                GetAllEscrowDetailsInput getAllEscrowDetailsInput = new GetAllEscrowDetailsInput();
                GetAllSrEscrowsInput getAllSrEscrowsInput = new GetAllSrEscrowsInput();
                getAllEscrowDetailsInput.EmailFilter = username;
                var escrowDetails = _escrowDetailRepository.GetAll().Where(x => x.Email == username).ToList();
                foreach (var detail in escrowDetails)
                {
                    UserCompany userCompany = new UserCompany();
                    getAllSrEscrowsInput.SubCompanyNameFilter = detail.Company;
                    var srEscrow = _ISrEscrowRepository.GetAll().Where(x => x.SubCompanyName == detail.Company && x.EscrowNo == detail.EscrowId).ToList();

                    foreach (var escrow in srEscrow)
                    {
                        var company = _enterpriseRepository.GetAll().Where(x => x.Id == escrow.EnterpriseId).FirstOrDefault();
                        userCompany.address = escrow.PropertyAddress;
                        userCompany.buyer = null;
                        userCompany.company = company.EnterpriseName;
                        userCompany.subCompany = escrow.SubCompanyName;
                        userCompany.escrowId = escrow.EscrowNo;
                        userCompany.seller = null;

                        userCompany.type = detail.Usertype;
                        getComp.Add(userCompany);
                    }
                }
                return getComp.ToList();
            }
            catch (Exception ex)
            {

            }
            return null;
        }


        ///<Summary>
        /// Read users details
        ///</Summary>
        public async Task<string> ReadTextFile(string path)
        {
            string status = "";
            int counter = 0;
            string line;
            var folderName = Path.Combine(@"wwwroot\Common\Paperless");
            try
            {
                // Read the file and display it line by line.  
                System.IO.StreamReader file = new System.IO.StreamReader(path);
                while ((line = file.ReadLine()) != null)
                {
                    System.Console.WriteLine(line);
                    if (!line.Contains("Email, User Name, Company, Invitee, Esc#, Access Types"))
                    {
                        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                        var reg = new Regex("(\\\")(.*?)(\\\")", RegexOptions.Compiled);
                        List<string> list = new List<string>();
                        foreach (System.Text.RegularExpressions.Match match in reg.Matches(line))
                        { list.Add(match.Value.Replace("\"", "")); }
                        if (list.Count > 1)
                        {
                            var myemail = list[0];

                            var userAction = new string[] { "SRX","BRX","BR1","BR2","BR3","BR4","BR5","BR6","BR7","BR8","BR9","BR10",
"SR1","SR2","SR3","SR4","SR5","SR6","SR7","SR8","SR9","SR10",
"RAL","RBL","RAS","RBS","RAO","RBO","LR1","LR2","LR3","LP1","LP2",
"LP3","TCX","TCA","LBX","LBP","EO1","EA1","EOX","EAX","TC1","TC2",
"TC3","TC4","TC5","TC6","TC7","TC8","TC9","TC10", "LTC","STC","OTC" };


                            var isValidUserTpye = userAction.Where(x => x == list[5].Replace("{", "").Replace("}", "")).FirstOrDefault();
                            if (isValidUserTpye == null)
                            {
                                status += "\n " + "Invalid Usertype: This UserType Is not Exist  -- Usertype: " + list[5] + " --EscrowId: " + list[4];
                                continue;
                            }
                            string CheckValidateEmail = conf["CheckValidateEmail"].ToString();
                            bool isValidEmail = true;
                            if (CheckValidateEmail == "true")
                            {
                                var client = new RestClient("https://api.apilayer.com/email_verification/check?email=" + myemail);
                                client.Timeout = -1;
                                var request = new RestRequest(Method.GET);
                                request.AddHeader("apikey", "iXb3be6bKcC7GfWoAQeLm6Ar1Q9d4Tzs");
                                IRestResponse response = client.Execute(request);
                                dynamic datasmtp = JsonConvert.DeserializeObject(response.Content);
                                Console.WriteLine(response.Content);
                                if (datasmtp.smtp_check != true)
                                {
                                    isValidEmail = true;
                                }
                                else
                                {
                                    isValidEmail = false;
                                }
                            }
                            if (isValidEmail == true)
                            {
                                var check = _userRepository.GetAll().Where(x => x.EmailAddress == list[0]).FirstOrDefault();
                                var usrtypeschk = _escrowDetailRepository.GetAll().Where(x => list[5].Contains(x.Usertype) && x.EscrowId == list[4]).FirstOrDefault();
                                if (usrtypeschk != null)
                                {
                                    var chk = _userRepository.GetAll().Where(x => x.Id == usrtypeschk.UserId).FirstOrDefault();


                                    if (chk.IsEmailConfirmed == false && usrtypeschk.Email != list[0])
                                    {

                                        _escrowDetailRepository.Delete(usrtypeschk.Id);
                                        _srInvitationRecordRepository.Delete(usrtypeschk.Id);

                                        _userRepository.Delete(chk.Id);


                                        UserData userData = new UserData();
                                        userData.fromEmail = list[0].Trim();
                                        userData.name = list[1];
                                        userData.company = list[2];
                                        userData.invitee = list[3];
                                        userData.escro = list[4].Trim();
                                        userData.type = list[5];
                                        var createuser = createUser(userData);
                                        if (createuser == 1)
                                        {
                                            status += "\n User Created";
                                            var Sendmailescrow = await SendMailEscrow(userData);
                                            status += "\n " + Sendmailescrow.message;

                                        }
                                        else if (createuser == 0)
                                        {
                                            status += "\n User Not Created";
                                        }
                                        // create CompanyUserType
                                        //var Sendmailescrow = SendMailEscrow(userData);
                                        //status += "\n " + Sendmailescrow.message;

                                    }
                                    else
                                    {
                                        status += "\n " + "Usertype Already Exist for this Escrow -- Usertype: " + list[5] + " --EscrowId: " + list[4];
                                    }
                                }
                                else
                                {
                                    if (check != null)
                                    {
                                        getid = 0;
                                        typestore = ""; string UD = "";
                                        Regex regex = new Regex(@"\{.*?\}");
                                        MatchCollection matchess = regex.Matches(list[5]);
                                        for (int i = 0; i < matchess.Count; i++)
                                        {
                                            string types = matchess[i].Value.Replace("{", "").Replace("}", "");
                                            UserData userData = new UserData();
                                            userData.fromEmail = list[0].Trim();
                                            userData.name = list[1];
                                            userData.company = list[2];
                                            userData.invitee = list[3];
                                            userData.escro = list[4].Trim();
                                            userData.type = types.Trim();
                                            var necheck = _escrowDetailRepository.GetAll();//.Where(x => x.Email == list[0]&&x.Usertype.Contains(types.Trim()) && x.EscrowId.Trim() == list[4].Trim() && x.Company == list[2]).FirstOrDefault();


                                            EscrowDetail Temp = null;

                                            foreach (var ent in necheck)
                                            {
                                                if (ent.Email == list[0] && ent.Usertype.Contains(types.Trim()) && ent.EscrowId.Trim() == list[4].Trim() && ent.Company == ValidFileName(list[2]))
                                                {
                                                    Temp = ent;
                                                }
                                            }

                                            if (Temp == null)
                                            {
                                                var sendmailupdate = SendMailUpdate(userData);
                                                if (!string.IsNullOrWhiteSpace(sendmailupdate.message))
                                                {
                                                    status += "\n " + sendmailupdate.message;
                                                }

                                                else
                                                {
                                                    status += "\n " + "Update Mail Sent";
                                                }
                                            }
                                            else
                                            {
                                                if (UD == "")
                                                {
                                                    status += "\n " + "Already Exists user details-: " + line;
                                                }
                                            }
                                        }

                                        //string filename = Path.Combine(_hostingEnvironment.WebRootPath, @"Common\Paperless\AlreadyInvited.txt");
                                        //if (!System.IO.File.Exists(filename))
                                        //{
                                        //    FileStream fs1 = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write);
                                        //}
                                        //StreamWriter writer = new StreamWriter(filename, true);
                                        //writer.WriteLine(line);
                                        //writer.Close();
                                    }
                                    else
                                    {
                                        UserData userData = new UserData();
                                        userData.fromEmail = list[0].Trim();
                                        userData.name = list[1];
                                        userData.company = list[2];
                                        userData.invitee = list[3];
                                        userData.escro = list[4].Trim();
                                        userData.type = list[5];
                                        var createuser = createUser(userData);
                                        if (createuser == 1)
                                        {
                                            status += "\n User Created";
                                            //var Sendmailescrow = SendMailEscrow(userData);
                                            //status += "\n " + Sendmailescrow.message;
                                            var Sendmailescrow = await SendMailEscrow(userData);
                                            status += "\n " + Sendmailescrow.message;

                                        }
                                        else if (createuser == 0)
                                        {
                                            status += "\n User Not Created";
                                        }
                                        // create CompanyUserType
                                        //var Sendmailescrow = SendMailEscrow(userData);
                                        //status += "\n " + Sendmailescrow.message;
                                    }
                                }


                                //verifyemailbelow
                            }
                            else
                            {
                                var escrow = _ISrEscrowRepository.GetAll().Where(x => x.EscrowNo == list[4]).FirstOrDefault();



                                MailMessage mail = new MailMessage();
                                mail.From = new MailAddress("Noreply@EscrowBasePortal.com");
                                mail.To.Add(escrow.EOEmail);
                                mail.Subject = "Invalid Email Added";
                                //Attachment datas = new Attachment(filem, MediaTypeNames.Application.Octet);

                                //mail.Attachments.Add(datas);
                                string referer = conf["App:ClientRootAddress"].ToString();
                                var Message = "Email not valid for " + line;
                                Random rnd = new Random();
                                mail.IsBodyHtml = true;
                                mail.Body = Message;
                                SmtpClient SmtpServer = new SmtpClient();
                                SmtpServer.Port = 587;
                                SmtpServer.Credentials = new System.Net.NetworkCredential("office@mandavconsultancy.com", "aouownmhogfobzbc");
                                SmtpServer.Host = "smtp.gmail.com";
                                SmtpServer.EnableSsl = true;
                                SmtpServer.Send(mail);

                                status += "\n " + "Email Invalid " + line;
                            }
                        }
                    }
                    counter++;
                }
                file.Close();
                return status;
            }
            catch (Exception ex)
            {
                string logs = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");
                if (!System.IO.File.Exists(logs))
                {
                    FileStream fs1 = new FileStream(logs, FileMode.OpenOrCreate, FileAccess.Write);
                }
                StreamWriter writer = new StreamWriter(logs, true);
                writer.WriteLine("Error in ReadTextFile method for -: error=" + ex.ToString() + DateTime.Now.ToString());
                writer.Close();
                System.Console.WriteLine("There were {0} lines.", counter);
                // Suspend the screen.  
                System.Console.ReadLine();
                status = "Error While Sending email";
                return status;
            }
        }

        public string SanitizeFileName(string fileName)
        {
            string sanitizedFileName = fileName.Replace("&", "_")
                                               .Replace("?", "_")
                                               .Replace("=", "_")
                                                  .Replace("!", "_")
                                                     .Replace("@", "_")
                                                        .Replace("$", "_")
                                                           .Replace("%", "_")
                                                              .Replace("^", "_")
                                                .Replace("+", "_");



            var now = DateTime.Now;
            string dateTimeStamp = now.ToString("yyyy-MM-dd_HH-mm-ss");

            int index = sanitizedFileName.IndexOf("~");
            if (index != -1)
            {
                sanitizedFileName = sanitizedFileName.Insert(index, $"_{dateTimeStamp}");
            }
            else
            {
                sanitizedFileName = $"{sanitizedFileName}_{dateTimeStamp}";
            }

            return sanitizedFileName;
        }

        ///<Summary>
        /// Upload all files for single user(file transfer)
        ///</Summary>
        [HttpPost]
        public async Task<IActionResult> AutoUpdate(string source, string Destination, string usname, string pname)
        {

            var us = ConfigurationManager.AppSettings["UserName"];
            var ps = ConfigurationManager.AppSettings["Password"]; 
            if (us.ToLower() == usname.ToLower() && ps == pname)
            {
                Destination = ValidFileName(Destination);
                Destination = Destination.Replace("=", "\\").Replace(".\\", "\\");
                string message = ""; bool isUploaded = false;
                try
                {
                    string[] subs = Destination.Split('\\');
                    var ids = $"{subs[2]}";
                    CreateOrEditSrFileMappingDto coesfm = new CreateOrEditSrFileMappingDto();
                    var usrid = _escrowDetailRepository.GetAll().Where(x => x.EscrowId == ids).ToList();

                    // replacing BRX AND SRX In File Name

                    var fileNameList = Request.Form.Files[0];
                    var BRXUserList = string.Empty;
                    var BRXType = string.Empty;
                    var SRXUserList = string.Empty;
                    var SRXType = string.Empty;
                    string matchLst = String.Empty;
                    string fileUpdateName = SanitizeFileName(fileNameList.FileName);
                    MatchCollection matchesData = regexx.Matches(fileUpdateName);
                    for (int i = 0; i < matchesData.Count; i++)
                    {
                        string mails = String.Empty;
                        string rep = matchesData[i].Value.Replace("{", "").Replace("}", "");
                        var userTypeAndAction = rep.Split("-");
                        matchLst = userTypeAndAction[0];

                        if (matchLst == "BRX")
                        {
                            BRXType = matchesData[i].Value;
                            foreach (var user in usrid)
                            {
                                if (user.Usertype.Contains("BR"))
                                {
                                    BRXUserList += "{" + user.Usertype + "-" + userTypeAndAction[1] + "}";
                                }
                            }
                        }
                        if (matchLst == "SRX")
                        {
                            SRXType = matchesData[i].Value;
                            foreach (var user in usrid)
                            {
                                if (user.Usertype.Contains("SR"))
                                {
                                    SRXUserList += "{" + user.Usertype + "-" + userTypeAndAction[1] + "}";
                                }
                            }
                        }
                    }
                    var fileNewName = fileUpdateName;//Request.Form.Files[0].FileName;
                    if (!string.IsNullOrWhiteSpace(BRXUserList))
                    {
                        if (fileNewName.Contains(BRXType))
                        {
                            fileNewName = fileNewName.Replace(BRXType, BRXUserList);
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(SRXUserList))
                    {
                        if (fileNewName.Contains(SRXType))
                        {
                            fileNewName = fileNewName.Replace(SRXType, SRXUserList);
                        }
                    }




                    var shad = Path.Combine(_hostingEnvironment.WebRootPath + @"\Common\Paperless\");
                    var Destdirect = Path.Combine(shad, Destination);
                    if (!Directory.Exists(Destdirect))
                    {
                        Directory.CreateDirectory(Destdirect);
                    }
                    responseData res = new responseData();
                    bool isFileConverted = false;
                    var pathFullZoho = Path.Combine(Destdirect + "\\" + fileNewName);

                    string destnation1 = Path.Combine(Destdirect + "\\" + fileNewName);
                    var pathZohoPdf = destnation1.Substring(destnation1.LastIndexOf("Paperless\\") + 10);
                    string escrowidZohoPdf = Destination.Substring(Destination.LastIndexOf('\\') + 1);



                    foreach (var usr in usrid)
                    {
                        foreach (var File in Request.Form.Files)
                        {
                            long SrEscrowFileMasterId = 0;
                            string destnationsForMaster = Path.Combine(Destdirect + "\\" + fileUpdateName);

                            var dbSREscrowFileMaster = _srEscrowFileMasterRepository
                                              .GetAll()
                                              .Where(x => x.FileFullName == destnationsForMaster)
                                              .FirstOrDefault();

                            // insert into Escrow File Mater table
                            if (dbSREscrowFileMaster == null)
                            {
                                SREscrowFileMaster sREscrowFileMaster = new SREscrowFileMaster();
                                sREscrowFileMaster.FileFullName = destnationsForMaster;
                                sREscrowFileMaster.FileShortName = fileNewName;
                                SrEscrowFileMasterId = _srEscrowFileMasterRepository.InsertAndGetId(sREscrowFileMaster);

                                CreateOrEditEscrowFileHistoryDto escrowFileHistory = new CreateOrEditEscrowFileHistoryDto();
                                escrowFileHistory.SrEscrowFileMasterId = SrEscrowFileMasterId;
                                escrowFileHistory.FileFullPath = fileNewName;
                                escrowFileHistory.UserId = 8;
                                escrowFileHistory.Message = FileConstant.ADD_File;
                                escrowFileHistory.ActionType = FileConstantAction.ADD_File;
                                await _escrowFileHistoriesAppService.CreateOrEdit(escrowFileHistory);
                            }
                            else
                            {
                                SrEscrowFileMasterId = dbSREscrowFileMaster.Id;
                            }

                            string acces = "", record = "";
                            string fileName = fileNewName;
                            if (Destination.Contains("Other"))
                            {
                                string escrowid = Destination.Substring(Destination.LastIndexOf('\\') + 1);
                                string destnations = Path.Combine(Destdirect + "\\" + fileName);
                                using (var stream = new FileStream(destnations, FileMode.Create))
                                {
                                    File.CopyTo(stream);
                                    isUploaded = true;
                                }
                                coesfm.FileName = destnations;
                                coesfm.UserId = (int)usr.UserId;
                                coesfm.Action = "READ";
                                coesfm.EscrowiId = escrowid;
                                coesfm.IsActive = true;
                                coesfm.SrEscrowFileMasterId = SrEscrowFileMasterId;
                                var filemap = _ISrFileMappingsAppService.CreateOrEdit(coesfm);
                            }
                            else
                            {
                                bool check = false;
                                string comp = "";
                                string escrowid = Destination.Substring(Destination.LastIndexOf('\\') + 1);
                                if (fileName.Contains("~") || fileName.Contains("-'-") || fileName.Contains("_'_"))
                                {
                                    if (fileName.Contains("~"))
                                        acces = fileName.Substring(fileName.LastIndexOf("~") + 1);
                                    if (fileName.Contains("-'-"))
                                        acces = fileName.Substring(fileName.LastIndexOf("-'-") + 3);
                                    if (fileName.Contains("_'_"))
                                        acces = fileName.Substring(fileName.LastIndexOf("_'_") + 3);
                                }
                                else
                                {
                                    acces = fileName;
                                    string destnations = Path.Combine(Destdirect + "\\" + fileName);
                                    using (var stream = new FileStream(destnations, FileMode.Create))
                                    {
                                        File.CopyTo(stream);
                                        isUploaded = true;
                                    }
                                    coesfm.FileName = destnations;
                                    coesfm.UserId = (int)usr.UserId;
                                    coesfm.Action = "READ";
                                    coesfm.EscrowiId = escrowid;
                                    coesfm.IsActive = true;
                                    coesfm.SrEscrowFileMasterId = SrEscrowFileMasterId;
                                    var filemap = _ISrFileMappingsAppService.CreateOrEdit(coesfm);
                                    check = true;
                                }
                                if (!check)
                                {
                                    var file = String.Empty;
                                    string destnation = Path.Combine(Destdirect + "\\" + fileName);
                                    if (fileName.Contains("~"))
                                    {
                                        file = fileName.Substring(0, fileName.LastIndexOf("~"));
                                    }
                                    else
                                    {
                                        return Json(new { isUploaded = isUploaded, message = "Went wrong filename should contains ~'~ in -: " + fileName });
                                    }
                                    var exists = _srfilemapRepository.GetAll().Where(a => a.FileName.Contains(file)).ToList();
                                    if (exists.Count > 0)
                                    {
                                        string[] fileNames = System.IO.Directory.GetFiles(Destdirect, "*" + file + "*.pdf");
                                        if (fileNames.Length > 0)
                                        {
                                            System.IO.File.Delete(fileNames[0]);
                                            foreach (var id in exists)
                                            {
                                                //_srfilemapRepository.Delete(id);
                                            }
                                        }
                                    }
                                    //using (var stream = new FileStream(destnation, FileMode.Create))
                                    //{
                                    //    File.CopyTo(stream);
                                    //    isUploaded = true;
                                    //}
                                    string match = String.Empty;

                                    MatchCollection matches = regexx.Matches(acces);
                                    for (int i = 0; i < matches.Count; i++)
                                    {


                                        string mails = String.Empty;
                                        string rep = matches[i].Value.Replace("{", "").Replace("}", "");
                                        int index = rep.IndexOf('-');
                                        match = rep.Substring(index + 1);
                                        //if (!match.Contains("S") && match.Contains("E"))
                                        //{
                                        //    DocumentRecord(fileName, "Input", long.Parse(usr.UserId.ToString()), SrEscrowFileMasterId);
                                        //}
                                        //else if (!match.Contains("S") && !match.Contains("E"))
                                        //{
                                        //    DocumentRecord(fileName, "Read", long.Parse(usr.UserId.ToString()), SrEscrowFileMasterId);
                                        //}
                                        //if (match.Contains("S"))
                                        //{
                                        //    if (!message.Contains("Access level contains S"))
                                        //    {
                                        //        message += "(Access level contains S If it is not signing file then there may be some glitches in fileview)";
                                        //    }
                                        //    var path = destnation.Substring(destnation.LastIndexOf("Paperless\\") + 10);

                                        //    DocumentRecord(fileName, "Sign", long.Parse(usr.UserId.ToString()), SrEscrowFileMasterId);
                                        //}
                                        if ((match.Contains("A") && (rep.Contains(usr.Usertype) && usr.Usertype != "") || (rep.Contains("BRX") && usr.Usertype.Contains("BR")) || (rep.Contains("SRX") && usr.Usertype.Contains("SR"))))
                                        {
                                            if (rep.Contains("BRX") && usr.Usertype.Contains("BR"))
                                            {


                                                //var action = matches[i].Value;
                                                //if(action.Contains("BRX"))


                                                if (!match.Contains("S") && match.Contains("E"))
                                                {
                                                    DocumentRecord(fileName, "Input", long.Parse(usr.UserId.ToString()), SrEscrowFileMasterId);
                                                }
                                                else if (!match.Contains("S") && !match.Contains("E"))
                                                {
                                                    DocumentRecord(fileName, "Read", long.Parse(usr.UserId.ToString()), SrEscrowFileMasterId);
                                                }
                                                if (match.Contains("S"))
                                                {
                                                    if (!message.Contains("Access level contains S"))
                                                    {
                                                        message += "(Access level contains S If it is not signing file then there may be some glitches in fileview)";
                                                    }
                                                    var path = destnation.Substring(destnation.LastIndexOf("Paperless\\") + 10);

                                                    DocumentRecord(fileName, "Sign", long.Parse(usr.UserId.ToString()), SrEscrowFileMasterId);
                                                }

                                                CreateOrEditSrFileMappingDto coesfm1 = new CreateOrEditSrFileMappingDto();
                                                coesfm1.FileName = destnation;
                                                coesfm1.UserId = (int)usr.UserId;
                                                coesfm1.Action = matches[i].Value.Contains("BRX") ? matches[i].Value.Replace("BRX", usr.Usertype) : matches[i].Value;
                                                coesfm1.EscrowiId = escrowid;
                                                coesfm1.IsActive = true;
                                                coesfm1.SrEscrowFileMasterId = SrEscrowFileMasterId;
                                                var filemap = _ISrFileMappingsAppService.CreateOrEditdata(coesfm1);
                                                continue;
                                            }
                                            if (rep.Contains("SRX") && usr.Usertype.Contains("SR"))
                                            {

                                                if (!match.Contains("S") && match.Contains("E"))
                                                {
                                                    DocumentRecord(fileName, "Input", long.Parse(usr.UserId.ToString()), SrEscrowFileMasterId);
                                                }
                                                else if (!match.Contains("S") && !match.Contains("E"))
                                                {
                                                    DocumentRecord(fileName, "Read", long.Parse(usr.UserId.ToString()), SrEscrowFileMasterId);
                                                }
                                                if (match.Contains("S"))
                                                {
                                                    if (!message.Contains("Access level contains S"))
                                                    {
                                                        message += "(Access level contains S If it is not signing file then there may be some glitches in fileview)";
                                                    }
                                                    var path = destnation.Substring(destnation.LastIndexOf("Paperless\\") + 10);

                                                    DocumentRecord(fileName, "Sign", long.Parse(usr.UserId.ToString()), SrEscrowFileMasterId);
                                                }

                                                CreateOrEditSrFileMappingDto coesfm1 = new CreateOrEditSrFileMappingDto();
                                                coesfm1.FileName = destnation;
                                                coesfm1.UserId = (int)usr.UserId;
                                                coesfm1.Action = matches[i].Value.Contains("SRX") ? matches[i].Value.Replace("SRX", usr.Usertype) : matches[i].Value; ;
                                                coesfm1.EscrowiId = escrowid;
                                                coesfm1.IsActive = true;
                                                coesfm1.SrEscrowFileMasterId = SrEscrowFileMasterId;
                                                var filemap = _ISrFileMappingsAppService.CreateOrEditdata(coesfm1);
                                                continue;
                                            }
                                            else
                                            {

                                                if (!match.Contains("S") && match.Contains("E"))
                                                {
                                                    DocumentRecord(fileName, "Input", long.Parse(usr.UserId.ToString()), SrEscrowFileMasterId);
                                                }
                                                else if (!match.Contains("S") && !match.Contains("E"))
                                                {
                                                    DocumentRecord(fileName, "Read", long.Parse(usr.UserId.ToString()), SrEscrowFileMasterId);
                                                }
                                                if (match.Contains("S"))
                                                {
                                                    if (!message.Contains("Access level contains S"))
                                                    {
                                                        message += "(Access level contains S If it is not signing file then there may be some glitches in fileview)";
                                                    }
                                                    var path = destnation.Substring(destnation.LastIndexOf("Paperless\\") + 10);

                                                    DocumentRecord(fileName, "Sign", long.Parse(usr.UserId.ToString()), SrEscrowFileMasterId);
                                                }

                                                CreateOrEditSrFileMappingDto coesfm1 = new CreateOrEditSrFileMappingDto();
                                                coesfm1.FileName = destnation;
                                                coesfm1.UserId = (int)usr.UserId;
                                                coesfm1.Action = matches[i].Value;
                                                coesfm1.EscrowiId = escrowid;
                                                coesfm1.IsActive = true;
                                                coesfm1.SrEscrowFileMasterId = SrEscrowFileMasterId;
                                                var filemap = _ISrFileMappingsAppService.CreateOrEditdata(coesfm1);
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }

                    var fileUesrTokenCount = 0;

                    foreach (var FileItem in Request.Form.Files)
                    {

                        string destnations = Path.Combine(Destdirect + "\\" + fileNewName);
                        string logs101 = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");
                        if (!System.IO.File.Exists(logs101))
                        {
                            FileStream fs1 = new FileStream(logs101, FileMode.OpenOrCreate, FileAccess.Write);
                        }
                        StreamWriter writer1004 = new StreamWriter(logs101, true);
                        writer1004.WriteLine("Calling File Mapping -: error=" + DateTime.Now.ToString());
                        writer1004.Close();
                        //  SrFileMapping fileInfo1 = null;
                        //using (var unit = _unitOfWorkManager.Begin())
                        // {
                        var fileInfo1 = _srfilemapRepository.GetAll().Where(x => x.FileName == destnations).FirstOrDefault();
                        // unit.Complete();

                        // }


                        string logs102 = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");
                        if (!System.IO.File.Exists(logs102))
                        {
                            FileStream fs1 = new FileStream(logs102, FileMode.OpenOrCreate, FileAccess.Write);
                        }
                        StreamWriter writer103 = new StreamWriter(logs102, true);
                        writer103.WriteLine("Done File Mapping -: error=" + DateTime.Now.ToString());
                        writer103.Close();
                        var action = fileInfo1.FileName;
                        var splitA = action.Split("-");
                        var Action = splitA[1];
                        var EscrowIds = fileInfo1.EscrowiId;
                        SrEscrow detail = null;
                        using (var unit = _unitOfWorkManager.Begin())
                        {

                            detail = _ISrEscrowRepository.GetAll().Where(x => x.EscrowNo == EscrowIds).FirstOrDefault();
                            unit.Complete();
                        }

                        string logs103 = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");
                        if (!System.IO.File.Exists(logs103))
                        {
                            FileStream fs1 = new FileStream(logs103, FileMode.OpenOrCreate, FileAccess.Write);
                        }
                        StreamWriter writer1003 = new StreamWriter(logs103, true);
                        writer1003.WriteLine("Calling File Mapping -: error=" + DateTime.Now.ToString());
                        writer1003.Close();

                        var emailing = detail.EOEmail;
                        EscrowDetail item = new EscrowDetail();
                        item.Email = emailing;
                        usrid.Add(item);

                        foreach (var usr in usrid)
                        {
                            string match = String.Empty;
                            MatchCollection matches = regexx.Matches(fileNewName);
                            for (int i = 0; i < matches.Count; i++)
                            {
                                string mails = String.Empty;
                                string rep = matches[i].Value.Replace("{", "").Replace("}", "");
                                int index = rep.IndexOf('-');
                                match = rep.Substring(index + 1);

                                if (usr.Usertype == null)
                                {
                                    usr.Usertype = "";
                                }
                                bool isInviteAccepted = false;
                                var fullName = "";
                                var dbUser = _userRepository.GetAll().Where(x => x.EmailAddress == usr.Email).FirstOrDefault();
                                if (dbUser != null)
                                {
                                    if (dbUser.IsEmailConfirmed == true)
                                    {
                                        isInviteAccepted = true;
                                        fullName = dbUser.FullName;
                                    }
                                    else
                                    {
                                        isInviteAccepted = false;

                                    }
                                }
                                //if ((match.Contains("A") && rep.Contains(usr.Usertype)) || usr.Usertype == "")
                                if ((match.Contains("A") && isInviteAccepted == true && (rep.Contains(usr.Usertype) && usr.Usertype != "") || (rep.Contains("BRX") && usr.Usertype.Contains("BR")) || (rep.Contains("SRX") && usr.Usertype.Contains("SR"))))
                                {
                                    fileUesrTokenCount = fileUesrTokenCount + 1;

                                    if (!System.IO.File.Exists(destnations))
                                    {
                                        using (var stream = new FileStream(destnations, FileMode.Create))
                                        {
                                            FileItem.CopyTo(stream);
                                            isUploaded = true;
                                        }
                                    }


                                    //CreateOrEditSrFileMappingDto coesfm1 = new CreateOrEditSrFileMappingDto();
                                    //coesfm1.FileName = destnations;
                                    //coesfm1.UserId = (int)usr.UserId;
                                    //coesfm1.Action = matches[i].Value;
                                    //coesfm1.EscrowiId = escrowidZohoPdf;
                                    //coesfm1.IsActive = true;
                                    //var filemap = _ISrFileMappingsAppService.CreateOrEditdata(coesfm1);


                                    string filem = destnations;
                                    MailMessage mail = new MailMessage();
                                    mail.From = new MailAddress("Noreply@EscrowBasePortal.com");
                                    mail.To.Add(usr.Email);
                                    mail.Subject = "New File Uploaded";
                                    //Attachment datas = new Attachment(filem, MediaTypeNames.Application.Octet);
                                    //mail.Attachments.Add(datas);
                                    string referer = conf["App:ClientRootAddress"].ToString();
                                    var escrow = _escrowDetailRepository.GetAll().Where(x => x.EscrowId == EscrowIds).FirstOrDefault();
                                    var enterprises = _enterpriseRepository.GetAll().Where(x => x.EnterpriseName == escrow.Company).FirstOrDefault();
                                    var Company = enterprises.EnterpriseName;
                                    var userName = fullName;
                                    var logo = enterprises.Logo;
                                    if (logo == "" || logo == null) { logo = "https://ayushkamiya.com/Escrow-logo.png"; }
                                    var Message = "A new document delivered to the secure web portal regarding your escrow " + EscrowIds + " that needs your attention. Please log in to review, this reminder was sent by escrowbaseweb powered by software reality." + "'" + fileUpdateName + "'";
                                    string texts = "";
                                    using (StreamReader reader = System.IO.File.OpenText("wwwroot\\notification.html")) // Path to your Email format
                                    {
                                        texts = reader.ReadToEnd();
                                        texts = texts.Replace("$$Company$$", Company).Replace("$$Logo$$", logo).Replace("$$Message$$", Message).Replace("$$userName$$", userName);
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
                                    List<ExpandoObject> result = new List<ExpandoObject>();
                                    var userid = usr.UserId;
                                    string cs = conf["ConnectionStrings:Default"].ToString();
                                    {
                                        using var con = new MySqlConnection(cs);
                                        con.Open();
                                        var sql = $"Select * from usersmsprefrence where UserId=@UserId AND functionName=@functionName";
                                        using (var cmd = new MySqlCommand(sql, con))
                                        {
                                            cmd.Parameters.AddWithValue("@UserId", userid);
                                            cmd.Parameters.AddWithValue("@functionName", "Upload");

                                            var dr = cmd.ExecuteReader();
                                            var dataTable = new DataTable();
                                            dataTable.Load(dr);
                                            string JSONString = string.Empty;
                                            JSONString = JsonConvert.SerializeObject(dataTable);
                                            result = JsonConvert.DeserializeObject<List<ExpandoObject>>(JSONString);

                                        }
                                    }
                                    string logFilePath = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");

                                    using (StreamWriter writer = new StreamWriter(logFilePath, true))
                                    {
                                        writer.WriteLine("Checking the userlist -: error=" + DateTime.Now.ToString());
                                    }

                                    using (var unit = _unitOfWorkManager.Begin())
                                    {
                                        var usertest = _userRepository.GetAll().ToList();

                                        using (StreamWriter writer2 = new StreamWriter(logFilePath, true))
                                        {
                                            writer2.WriteLine("Done with the userlist -: =" + usertest.Count + DateTime.Now.ToString());
                                        }

                                        if (result.Any())
                                        {
                                            string pno = "";
                                            string phoneno = "7148121408";
                                            //var user = _userRepository.GetAll().Where(x => x.Id == usr.UserId).FirstOrDefault();
                                            //string phoneno = user.PhoneNumber;
                                            // if( phoneno != null) { }
                                            int _min = 111111;
                                            int _max = 999999;
                                            string AccountSid = conf["Twilio:AccountSid"].ToString();
                                            string AuthToken = conf["Twilio:AuthToken"].ToString();
                                            string MessagingServiceSids = conf["Twilio:MessagingServiceSid"].ToString();
                                            string SenderNumber = conf["Twilio:SenderNumber"].ToString();

                                            var cc = ConfigurationManager.AppSettings["CountryCode"];
                                            TwilioClient.Init(AccountSid, AuthToken);
                                            var User = new UserEditDto();
                                            CreateOrUpdateUserInput cuu = new CreateOrUpdateUserInput();
                                            if (!phoneno.Contains("+1"))
                                            {
                                                pno = "+1" + phoneno.Trim();
                                            }
                                            var messageOptions = new CreateMessageOptions(
                                                new PhoneNumber(pno));
                                            //messageOptions.MessagingServiceSid = MessagingServiceSid;
                                            messageOptions.Body = "A new document delivered to the secure web portal regarding your escrow " + EscrowIds + " that needs your attention. Please log in to review, this reminder was sent by escrowbaseweb powered by software reality." + "'" + fileUpdateName + "'";
                                            messageOptions.From = SenderNumber;
                                            User.PhoneNumber = pno;
                                            GetUsersInput input = new GetUsersInput();
                                            //input.Filter = un;
                                            input.MaxResultCount = 1000;

                                            using (StreamWriter writer3 = new StreamWriter(logFilePath, true))
                                            {
                                                writer3.WriteLine("Caliing GetUsersFilteredQuery -: =" + DateTime.Now.ToString());
                                            }
                                            var ch = _IUserAppService.GetUsersFilteredQuery(input).FirstOrDefault();
                                            using (StreamWriter writer4 = new StreamWriter(logFilePath, true))
                                            {
                                                writer4.WriteLine("done with GetUsersFilteredQuery -: =" + DateTime.Now.ToString());
                                            }
                                            if (ch != null)
                                            {
                                                using (StreamWriter writer5 = new StreamWriter(logFilePath, true))
                                                {
                                                    writer5.WriteLine("opening with CreateOrUpdateUser -: =" + DateTime.Now.ToString());
                                                }
                                                User.UserName = ch.UserName;
                                                User.EmailAddress = ch.EmailAddress;
                                                User.IsActive = ch.IsActive;
                                                User.PasswordResetCode = ch.PasswordResetCode;
                                                User.Name = ch.Name;
                                                User.Surname = ch.Surname;
                                                User.Id = ch.Id;
                                                cuu.User = User;
                                                var chh = _IUserAppService.CreateOrUpdateUser(cuu);
                                                using (StreamWriter writer6 = new StreamWriter(logFilePath, true))
                                                {
                                                    writer6.WriteLine("done with CreateOrUpdateUser -: =" + DateTime.Now.ToString());
                                                }
                                            }
                                            var messages = MessageResource.Create(messageOptions);
                                        }
                                        unit.Complete();
                                    }

                                }
                                else
                                {

                                }
                            }

                        }
                    }
                    if (isFileConverted == false && fileUesrTokenCount > 0)
                    {
                        isFileConverted = true;

                        var esignPath = pathZohoPdf.ToString().Replace("Other", "");
                        var esign = destnation1;
                        string[] esignKey = esign.Split("\\");
                        string shortfileName = esignKey.Where(x => x.Contains(".pdf")).FirstOrDefault();

                        esignPath = esignPath.Replace(shortfileName, "");

                        //var esignUser = (int)usr.UserId;
                        var sign = await ZohoESignCreateDocument(esignPath, shortfileName, escrowidZohoPdf);

                    }
                    else
                    {
                        message = "There was no valid token detected for this file, or perhaps this was because the user is not invited to this escrow company. \n";
                    }
                    string logs = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");
                    if (!System.IO.File.Exists(logs))
                    {
                        FileStream fs1 = new FileStream(logs, FileMode.OpenOrCreate, FileAccess.Write);
                    }
                    StreamWriter writer101 = new StreamWriter(logs, true);
                    writer101.WriteLine("Program Executed Successfully" + message + " " + DateTime.Now.ToString());
                    writer101.Close();

                    await _hub.Clients.All.SendAsync("getFileUploadMessage", isUploaded);

                    message += "(Program Executed Successfully)";
                    string logs1 = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");
                    if (!System.IO.File.Exists(logs))
                    {
                        FileStream fs1 = new FileStream(logs1, FileMode.OpenOrCreate, FileAccess.Write);
                    }
                    StreamWriter writer102 = new StreamWriter(logs1, true);
                    writer102.WriteLine("Program Executed Successfully" + isUploaded + " " + DateTime.Now.ToString());
                    writer102.Close();

                    return Json(new { isUploaded = isUploaded, message = message });
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    string logs = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");
                    if (!System.IO.File.Exists(logs))
                    {
                        FileStream fs1 = new FileStream(logs, FileMode.OpenOrCreate, FileAccess.Write);
                    }
                    StreamWriter writer = new StreamWriter(logs, true);
                    writer.WriteLine("Error in AutoUpdate method for -: error=" + ex.ToString() + DateTime.Now.ToString());
                    writer.Close();
                    return Json(new { isUploaded = isUploaded, message = message });
                }
            }
            else
            {
                return Json(new { message = "Invalid user name and password" });
            }
        }


        ///<Summary>
        /// Rename File
        ///</Summary>
        public async Task<IActionResult> Rename()
        {
            try
            {
                var filenameold = Request.Headers["filenameold"];
                var filenamenew = Request.Headers["filenamenew"];
                var shortfilenameold = Request.Headers["shortfilenameold"];
                var action = Request.Headers["userType"];
                var UserId = Request.Headers["userId"];
                var EscrowId = Request.Headers["escrowNewId"];
                var Path = "";
                var key = "";
                string filee = _hostingEnvironment.WebRootPath + "\\Common\\Paperless\\";
                var filenameolds = filee + filenameold;
                var filenamenews = filee + filenamenew;
                var shortnm = shortfilenameold.ToString();
                string host = conf["App:ServerRootAddress"].ToString();
                var filefound = _srfilemapRepository.GetAll().Where(x => x.FileName.Contains(System.IO.Path.GetFileName(filenameolds))).ToList();
                var signin = _srAssignedFilesDetailRepository.GetAll().Where(x => x.FileName == shortnm && x.SigningStatus == "Signed").FirstOrDefault();
                var signins = _srAssignedFilesDetailRepository.GetAll().Where(x => x.FileName == shortnm).ToList();
                var getEsigndDocument = _esignRepository.GetAll().Where(x => x.FileName == shortnm).FirstOrDefault();

                //var sin = signin.SigningStatus;


                if (signin != null)
                {
                    return Json(new { message = "File Already Signed" });
                }
                else
                {
                    if (getEsigndDocument != null)
                    {
                        var fileNameShort = System.IO.Path.GetFileName(filenamenew);
                        getEsigndDocument.FileName = fileNameShort;
                        getEsigndDocument.FullFilePath = "wwwroot\\Common\\Paperless\\" + filenamenew;
                        var esign = _esignRepository.Update(getEsigndDocument);

                    }
                    var getDbSREscrowFileMaster = _srEscrowFileMasterRepository.GetAll().Where(x => x.FileShortName == System.IO.Path.GetFileName(filenameold)).FirstOrDefault();

                    if (getDbSREscrowFileMaster != null)
                    {
                        var fileNameShort = System.IO.Path.GetFileName(filenamenew);
                        getDbSREscrowFileMaster.FileShortName = fileNameShort;
                        getDbSREscrowFileMaster.FileFullName = "wwwroot\\Common\\Paperless\\" + filenamenew;
                        await _srEscrowFileMasterRepository.UpdateAsync(getDbSREscrowFileMaster);
                    }

                    foreach (var file in filefound)
                    {

                        _srfilemapRepository.Delete(file.Id);
                    }
                    foreach (var files in signins)
                    {

                        _srAssignedFilesDetailRepository.Delete(files.Id);
                    }

                    action = action[0].Split(",");

                    foreach (var act in action)
                    {
                        var typeList = act.Replace("{", " ").Replace("}", " ");
                        var type = typeList.Split("-");
                        var escrowDetails = _escrowDetailRepository.GetAll().Where(x => x.EscrowId == EscrowId.ToString() && x.Usertype == type[0].Trim()).FirstOrDefault();
                        if (escrowDetails != null)
                        {
                            var fileNameShort = System.IO.Path.GetFileName(filenamenew);

                            SrAssignedFilesDetail srAssignedFilesDetail = new SrAssignedFilesDetail();
                            var userDetails = _userRepository.GetAll().Where(x => x.EmailAddress == escrowDetails.Email).FirstOrDefault();
                            srAssignedFilesDetail.UserId = userDetails.Id;
                            srAssignedFilesDetail.FileName = fileNameShort;
                            srAssignedFilesDetail.SigningStatus = "Unsigned";
                            srAssignedFilesDetail.ReadStatus = "Unread";
                            srAssignedFilesDetail.UpdatedOn = DateTime.Now;
                            srAssignedFilesDetail.SrEscrowFileMasterId = getDbSREscrowFileMaster.Id;
                            var srAssignedFilesDetailId = _srAssignedFilesDetailRepository.InsertOrUpdateAndGetId(srAssignedFilesDetail);

                            var file = new SrFileMapping();
                            file.FileName = filenamenew;
                            file.UserId = int.Parse(escrowDetails.UserId.ToString());
                            file.EscrowiId = EscrowId;
                            file.Action = act;
                            file.IsActive = true;
                            file.SrEscrowFileMasterId = getDbSREscrowFileMaster.Id;
                            await _srfilemapRepository.InsertAsync(file);
                        }

                    }

                    CreateOrEditEscrowFileHistoryDto escrowFileHistory = new CreateOrEditEscrowFileHistoryDto();
                    escrowFileHistory.SrEscrowFileMasterId = filefound.FirstOrDefault().SrEscrowFileMasterId;
                    escrowFileHistory.FileFullPath = filenamenew;
                    escrowFileHistory.UserId = AbpSession.UserId;
                    escrowFileHistory.Message = FileConstant.Rename_File;
                    escrowFileHistory.ActionType = FileConstantAction.Rename_File;
                    await _escrowFileHistoriesAppService.CreateOrEdit(escrowFileHistory);
                    System.IO.File.Move(filenameolds, filenamenews);
                    var getFolderPath = filenamenew.ToString().Replace(System.IO.Path.GetFileName(filenamenew), "");
                    await ZohoESignUpdateDocument(System.IO.Path.GetFileName(filenamenew), getFolderPath, EscrowId);
                }
            }
            catch (Exception ex)
            {
            }
            return Ok();
        }


        public async Task<responseBack> DropAreaRename()
        {
            try
            {
                string fileFullPath = _hostingEnvironment.WebRootPath + "\\Common\\Paperless\\";
                string fileNameOld = fileFullPath + Request.Headers["filenameold"];
                string fileNameNew = fileFullPath + Request.Headers["filenamenew"];

                fileNameOld = fileNameOld.Replace("/", "\\").Replace("\\\\", "\\");
                fileNameNew = fileNameNew.Replace("/", "\\").Replace("\\\\", "\\");

                Console.WriteLine($"fileNameOld: {fileNameOld}");
                Console.WriteLine($"fileNameNew: {fileNameNew}");

                var filefound = _srfilemapRepository.GetAll()
                    .Where(x => x.FileName.ToLower().EndsWith("\\" + Path.GetFileName(fileNameOld.ToLower())))
                    .ToArray();

                if (filefound.Length > 0)
                {
                    foreach (var file in filefound)
                    {
                        file.FileName = fileNameNew;
                        await _srfilemapRepository.UpdateAsync(file);
                    }

                    if (System.IO.File.Exists(fileNameOld))
                    {
                        System.IO.File.Move(fileNameOld, fileNameNew);
                    }
                    else
                    {
                        Console.WriteLine($"File not found on disk: {fileNameOld}");
                    }
                }
                else
                {
                    Console.WriteLine("File not found in database!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DropAreaRename: {ex.Message}");
            }
            return null;
        }

        public async Task<responseBack> Move()
        {
            responseBack response = new responseBack();
            try
            {
                var filenameold = Request.Headers["filenameold"];
                var filenamenew = Request.Headers["filenamenew"];
                var UserId = Request.Headers["userId"];
                var EscrowId = Request.Headers["escrowNewId"];
                var usrid = _escrowDetailRepository.GetAll();
                var action = Request.Headers["userType"];
                var fileExtension = Request.Headers["fileExtension"];
                var typeExist = Request.Headers["typeExist"];
                var oldName = Request.Headers["oldName"];
                var newPdfName = "";
                string filee = _hostingEnvironment.WebRootPath + "\\Common\\Paperless\\";
                if (oldName[0] != null && typeExist == "true")
                {
                    filenameold = oldName;
                }
                if (fileExtension[0] != "pdf")
                {
                    var fileNameSplitted = filenameold[0].Split("~");
                    filenameold = fileNameSplitted[0];
                    var docName = filee + filenameold;
                    var lastIndex = filenameold[0].LastIndexOf(".");
                    var result = filenameold[0].Substring(0, lastIndex);
                    newPdfName = result + ".pdf";
                    var newLocName = ("wwwroot\\Common\\Paperless\\" + newPdfName);
                    var oldLocName = ("wwwroot\\Common\\Paperless\\" + filenameold);
                    oldLocName = oldLocName.Replace("\\", "\\").Replace("/", "\\");

                    var uploadResponse = await UploadFileToPdfCo(oldLocName);

                    if (uploadResponse.Error)
                    {
                        throw new Exception("File upload to PDF.co failed: " + uploadResponse.Error);
                    }

                    // Step 2: Convert file to PDF
                    var convertResponse = await ConvertDocToPdf(uploadResponse.Url);

                    if (!convertResponse.success)
                    {
                        throw new Exception("Conversion to PDF failed: " + convertResponse.error);
                    }

                    // Step 3: Download the converted PDF
                    using (HttpClient httpClient = new HttpClient())
                    {
                        var pdfBytes = await httpClient.GetByteArrayAsync(convertResponse.url);
                        System.IO.File.WriteAllBytes(newLocName, pdfBytes);
                    }

                    // Step 4: Optionally delete the original DOC file
                    if (System.IO.File.Exists(oldLocName))
                    {
                        System.IO.File.Delete(oldLocName);
                    }
                
            }
                action = action[0].Split(",");
                var Path = "";
                var key = "";
                int Userid = 0;
                var Escrowid = "";
                var filenameolds = filee + filenameold;
                var filenamenews = filee + filenamenew;
                string host = conf["App:ServerRootAddress"].ToString();
                filenameolds = filenameolds.Replace("\\", "\\").Replace("/", "\\");
                var selectedfile = _srfilemapRepository.GetAll().Where(x => x.FileName == filenameolds).ToList();
                var fileNameShort = System.IO.Path.GetFileName(filenamenews);
                long SrFileId = 0;
                SREscrowFileMaster sREscrowFileMaster = new SREscrowFileMaster();
                sREscrowFileMaster.FileFullName = selectedfile.FirstOrDefault().FileName;

                sREscrowFileMaster.FileShortName = fileNameShort;
                var SrEscrowFileMasterId = _srEscrowFileMasterRepository.InsertAndGetId(sREscrowFileMaster);
                SrFileId = SrEscrowFileMasterId;
                foreach (var act in action)
                {

                    if (selectedfile.Count() > 0)
                    {
                        foreach (var item in selectedfile)
                        {
                            var typeList = act.Replace("{", " ").Replace("}", " ");
                            var type = typeList.Split("-");
                            var escrowDetails = _escrowDetailRepository.GetAll().Where(x => x.EscrowId == item.EscrowiId && x.Usertype == type[0].Trim()).FirstOrDefault();
                            if (escrowDetails != null)
                            {
                                SrAssignedFilesDetail srAssignedFilesDetail = new SrAssignedFilesDetail();
                                var userDetails = _userRepository.GetAll().Where(x => x.EmailAddress == escrowDetails.Email).FirstOrDefault();
                                srAssignedFilesDetail.UserId = userDetails.Id;
                                srAssignedFilesDetail.FileName = fileNameShort;
                                srAssignedFilesDetail.SigningStatus = "Unsigned";
                                srAssignedFilesDetail.ReadStatus = "Unread";
                                srAssignedFilesDetail.UpdatedOn = DateTime.Now;
                                srAssignedFilesDetail.SrEscrowFileMasterId = SrEscrowFileMasterId;
                                var srAssignedFilesDetailId = _srAssignedFilesDetailRepository.InsertOrUpdateAndGetId(srAssignedFilesDetail);

                              
                                MailMessage mail = new MailMessage();
                                mail.From = new MailAddress("Noreply@EscrowBasePortal.com");
                                mail.To.Add(userDetails.EmailAddress);
                                mail.Subject = "New File Uploaded";

                                string referer = conf["App:ClientRootAddress"].ToString();
                                var escrow = _escrowDetailRepository.GetAll().Where(x => x.EscrowId == item.EscrowiId).FirstOrDefault();
                                var enterprises = _enterpriseRepository.GetAll().Where(x => x.EnterpriseName == escrow.Company).FirstOrDefault();
                                var Company = enterprises.EnterpriseName;
                                var userName = userDetails.FullName;
                                var logo = enterprises.Logo;
                                if (logo == "" || logo == null) { logo = "https://ayushkamiya.com/Escrow-logo.png"; }
                                var Message = "A new document delivered to the secure web portal regarding your escrow " + item.EscrowiId + " that needs your attention. Please log in to review, this reminder was sent by escrowbaseweb powered by software reality." + "'" + fileNameShort + "'";
                                string texts = "";
                                using (StreamReader reader = System.IO.File.OpenText("wwwroot\\notification.html")) // Path to your Email format
                                {
                                    texts = reader.ReadToEnd();
                                    texts = texts.Replace("$$Company$$", Company).Replace("$$Logo$$", logo).Replace("$$Message$$", Message).Replace("$$userName$$", userName);
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

                            }
                        }
                    }
                    else
                    {
                        var file = new SrFileMapping();
                        file.FileName = filenamenews;
                        file.UserId = int.Parse(UserId);
                        file.EscrowiId = EscrowId;
                        file.Action = act;
                        file.IsActive = true;
                        await _srfilemapRepository.InsertAsync(file);
                    }
                }

                CreateOrEditEscrowFileHistoryDto escrowFileHistory = new CreateOrEditEscrowFileHistoryDto();
                escrowFileHistory.SrEscrowFileMasterId = SrFileId;
                escrowFileHistory.FileFullPath = fileNameShort;
                escrowFileHistory.UserId = AbpSession.UserId;
                escrowFileHistory.Message = FileConstant.ADD_File;
                escrowFileHistory.ActionType = FileConstantAction.ADD_File;
                await _escrowFileHistoriesAppService.CreateOrEdit(escrowFileHistory);

                foreach (var file in selectedfile)
                {
                    Userid = file.UserId;
                    Escrowid = file.EscrowiId;
                    _srfilemapRepository.Delete(file);

                    file.FileName = filenamenews;
                    if (fileExtension[0] != "pdf")
                    {
                        filenameolds = filee + newPdfName;
                    }
                    file.Action = "READ";
                    file.SrEscrowFileMasterId = SrEscrowFileMasterId;
                    await _srfilemapRepository.UpdateAsync(file);
                }


                System.IO.File.Move(filenameolds, filenamenews);
                System.IO.File.Delete(filenameolds);


            }
            catch (Exception ex)
            
            {
                response.statusCode = 409;
                response.message = "File Alerady Exist with same name and permissions";
                return response;
            }
            response.statusCode = 200;
            response.message = "Sucess";
            _hub.Clients.All.SendAsync("getFileUploadMessage", true);

            return response;
        }

        static async Task<FileUploadResponse> uploadFileToConvert(string filePath)
        {
            FileUploadResponse responseData = new FileUploadResponse();
            string apiUrl = "https://api.pdf.co/v1/file/upload";
            string apiKey = "bloodhollow01@gmail.com_YOWMUbSfp9BYXmkdmjbiQ0VTAEF14RqmbmHui489hKFciApvKGtCLcy56ULcrxSi";

            if (!System.IO.File.Exists(filePath))
            {
                Console.WriteLine("File not found: " + filePath);
                return responseData;
            }

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("x-api-key", apiKey);

                using (var content = new MultipartFormDataContent())
                {
                    var fileContent = new ByteArrayContent(System.IO.File.ReadAllBytes(filePath));
                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");
                    content.Add(fileContent, "file", Path.GetFileName(filePath));
                    content.Add(new StringContent($"{Path.GetFileName(filePath)}"), "name");

                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        responseData = JsonConvert.DeserializeObject<FileUploadResponse>(result);
                        return responseData;
                    }
                    else
                    {
                        Console.WriteLine("Failed to upload file. Status Code: " + response.StatusCode);
                        Console.WriteLine("Response: " + await response.Content.ReadAsStringAsync());
                    }
                }
            }
            return responseData;
        }


        public async Task<responseBack> SignRename(string EscrowId)
        {
            var parentpath = Request.Headers["parentpath"];
            var shortfilename = Request.Headers["shortfilename"];
            //var subs = parentpath.ToString().Split("\\");
            //var ids = subs[2];
            var usrid = _escrowDetailRepository.GetAll().Where(x => x.EscrowId == EscrowId).FirstOrDefault();
            if (usrid != null)
            {
                // var sign =  E_Sign(parentpath, shortfilename, usrid.UserId.ToString());
                var esignPath = parentpath.ToString().Replace("Other", "");
                var esignKey = shortfilename;
                var esignUser = usrid.UserId.ToString();
                var sign = ZohoESignCreateDocument(esignPath, esignKey, EscrowId);
            }

            return null;
        }


        ///<Summary>
        /// Assign signing in a doc for users
        ///</Summary>
        public responseBack E_Sign(string path, string key, string user)
        {
            try
            {
                responseBack res = new responseBack();
                string private_key_file_path = String.Empty, keyfile_password = String.Empty;
                path = path.Replace("%23", "#");
                path = ValidFileName(path);
                path = path.Replace("=", "\\");
                string host = conf["App:ServerRootAddress"].ToString();
                string client_id = conf["ESign:client_id"].ToString();
                string client_secret = conf["ESign:client_secret"].ToString();
                var folderName = Path.Combine(@"Common/Paperless/" + path);
                string referer = conf["App:ClientRootAddress"].ToString();
                var folderName2 = Path.Combine(folderName, key);
                string newpath = folderName2.Replace("/", "\\");
                string urlpath = folderName2.Replace("\\", "/");
                string file = Path.Combine(_hostingEnvironment.WebRootPath, newpath.Replace("%23", "#"));
                var file_Id = _srfilemapRepository.GetAll().Where(x => x.FileName == file).FirstOrDefault();

                #region
                string urlfile = Path.Combine(host + urlpath);
                var request = (HttpWebRequest)WebRequest.Create("https://na1.foxitesign.foxit.com/api/oauth2/access_token");
                var postData = "grant_type=client_credentials" + "&client_id=" + client_id + "&client_secret=" + client_secret + "&scope=read-write";
                var data = Encoding.ASCII.GetBytes(postData);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();
                string accessToken = "";
                string mails = "";
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var responseString = streamReader.ReadToEnd();
                    AccessToken myDeserializedClass = JsonConvert.DeserializeObject<AccessToken>(responseString);
                    accessToken = myDeserializedClass.access_token;
                }
                var webAddr = "https://na1.foxitesign.foxit.com/api/folders/createfolder";
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";
                httpWebRequest.Headers.Add("Authorization", "Bearer " + accessToken);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    createFolderForEsign.Root folderForEsign

                        = new createFolderForEsign.Root();
                    string token = "Saved";
                    string token1 = "Error";
                    string token2 = "Decline";
                    string token3 = "Later";

                    byte[] a = System.Text.ASCIIEncoding.ASCII.GetBytes(Convert.ToString(token));
                    byte[] b = System.Text.ASCIIEncoding.ASCII.GetBytes(Convert.ToString(token1));
                    byte[] c = System.Text.ASCIIEncoding.ASCII.GetBytes(Convert.ToString(token2));
                    byte[] d = System.Text.ASCIIEncoding.ASCII.GetBytes(Convert.ToString(token3));
                    byte[] g = System.Text.Encoding.UTF8.GetBytes(Convert.ToString(key.Replace("#", "%23")));
                    string encrypted = Convert.ToBase64String(a);
                    string encrypted1 = Convert.ToBase64String(b);
                    string encrypted2 = Convert.ToBase64String(c);
                    string encrypted3 = Convert.ToBase64String(d);
                    string encryptedFile = Convert.ToBase64String(g);
                    List<createFolderForEsign.Field> fields = new List<createFolderForEsign.Field>();
                    List<createFolderForEsign.Party> parties = new List<createFolderForEsign.Party>();
                    List<string> fileurl = new List<string>();
                    List<string> fileNames = new List<string>();
                    urlfile = urlfile.Replace(" ", "%20").Replace("#", "%23");
                    fileurl.Add(urlfile);
                    fileNames.Add(urlfile.Substring(urlfile.LastIndexOf('/') + 1));
                    folderForEsign.folderName = "test";
                    folderForEsign.fileUrls = fileurl;
                    folderForEsign.fileNames = fileNames;
                    folderForEsign.processTextTags = true;
                    folderForEsign.processAcroFields = true;
                    folderForEsign.signInSequence = false;
                    folderForEsign.sendNow = false;
                    folderForEsign.createEmbeddedSendingSession = false;
                    folderForEsign.fixRecipientParties = true;
                    folderForEsign.fixDocuments = true;
                    folderForEsign.sendSuccessUrl = referer + "app/main/File?token=" + encrypted + "&f=" + encryptedFile;
                    folderForEsign.sendErrorUrl = referer + "app/main/File?token=" + encrypted1;
                    folderForEsign.createEmbeddedSigningSession = true;
                    folderForEsign.createEmbeddedSigningSessionForAllParties = true;
                    folderForEsign.signSuccessUrl = referer + "app/main/File?token=" + encrypted + "&f=" + encryptedFile;
                    folderForEsign.signDeclineUrl = referer + "app/main/File?token=" + encrypted2;
                    folderForEsign.signLaterUrl = referer + "app/main/File?token=" + encrypted3;
                    folderForEsign.signErrorUrl = referer + "app/main/File?token=" + encrypted1;
                    folderForEsign.themeColor = "";
                    folderForEsign.allowSendNowAndEmbeddedSigningSession = false;


                    int i = 1;
                    Spire.Pdf.PdfDocument doc = new Spire.Pdf.PdfDocument();
                    doc.LoadFromFile(file);

                    Spire.Pdf.General.Find.PdfTextFind[] results = null;
                    foreach (Spire.Pdf.PdfPageBase page in doc.Pages)
                    {
                        results = page.FindText("@{").Finds;

                        foreach (Spire.Pdf.General.Find.PdfTextFind text in results)
                        {
                            createFolderForEsign.Party party = new createFolderForEsign.Party();
                            createFolderForEsign.Field field = new createFolderForEsign.Field();

                            #region 
                            var strf = text.OuterText.Replace("@", "").Replace("{", "").Replace("}", "");
                            var str = strf.Split(':');
                            var esc = path.Split('\\');
                            var escs = esc[2];
                            var esd = _escrowDetailRepository.GetAll().Where(x => x.Usertype.Contains(str[0].ToUpperInvariant()) && x.EscrowId == escs).FirstOrDefault();
                            if (esd != null)
                            {
                                //foreach (var dt in esd)
                                {
                                    string str1 = "";
                                    var s = esd.Name.Split(" ");
                                    if (s.Length > 1)

                                    {
                                        str1 = s[1];
                                    }
                                    else { str1 = ""; }
                                    if (!mails.Contains(esd.Email))
                                    {
                                        if (mails != "")
                                        {
                                            mails += "," + esd.Email + ":" + str[0].ToUpperInvariant().Trim();
                                        }
                                        else
                                        {
                                            mails += esd.Email + ":" + str[0].ToUpperInvariant().Trim();
                                        }
                                        party.firstName = s[0];
                                        party.lastName = str1;
                                        party.emailId = esd.Email;
                                        party.permission = "FILL_FIELDS_AND_SIGN";
                                        party.sequence = Convert.ToInt32(str[2]);
                                        party.dialingCode = "+1";
                                        party.mobileNumber = "";
                                        party.signerAuthLevel = "NO";
                                        parties.Add(party);
                                    }
                                }
                                if (str[1].ToLowerInvariant() == "i")
                                {
                                    field.type = "initial";
                                    field.party = Convert.ToInt32(str[2]);
                                    PointF p = text.Position;

                                    field.x = (int)p.X;
                                    field.y = (int)p.Y - 12;
                                    field.width = 100;
                                    field.height = 25;
                                    field.documentNumber = 1;
                                    field.pageNumber = i;
                                    field.required = true;
                                    fields.Add(field);
                                }
                                if (str[1].ToLowerInvariant() == "s")
                                {
                                    field.type = "signature";
                                    field.party = Convert.ToInt32(str[2]);
                                    PointF p = text.Position;

                                    field.x = (int)p.X;
                                    field.y = (int)p.Y - 12;
                                    field.width = 100;
                                    field.height = 25;
                                    field.documentNumber = 1;
                                    field.pageNumber = i;
                                    field.required = true;
                                    fields.Add(field);
                                }
                                if (str[0].ToLowerInvariant() == "d")
                                {
                                    field.type = "date";
                                    field.party = Convert.ToInt32(str[2]);
                                    PointF p = text.Position;

                                    field.x = (int)p.X;
                                    field.y = (int)p.Y - 17;
                                    field.width = 100;
                                    field.height = 30;
                                    field.documentNumber = 1;
                                    field.pageNumber = i;
                                    fields.Add(field);
                                }
                            }
                            #endregion
                        }
                        i++;
                    }
                    doc.Dispose();
                    if (mails != "")
                    {
                        var id = mails.Split(',');
                        foreach (var mail in id)
                        {
                            var typmail = mail.Split(":");
                            if (typmail[0].Trim() != username)
                            {
                                CreateOrEditSrFileMappingDto coesfmdto = new CreateOrEditSrFileMappingDto();
                                var usr = _userRepository.GetAll().Where(x => x.UserName == typmail[0].Trim()).FirstOrDefault();
                                if (usr != null)
                                {
                                    var escrowid = file.Substring(0, file.LastIndexOf('\\'));
                                    escrowid = escrowid.Substring(escrowid.LastIndexOf('\\') + 1);
                                    coesfmdto.FileName = file;
                                    coesfmdto.UserId = (int)usr.Id;
                                    coesfmdto.Action = "{" + typmail[1].Trim() + "-READS}";
                                    coesfmdto.EscrowiId = escrowid;
                                    coesfmdto.IsActive = true;
                                    // var ins = _ISrFileMappingsAp pService.CreateOrEdit(coesfmdto);
                                    // DocumentRecord(file.Substring(file.LastIndexOf('\\') + 1), "Sign", usr.Id);
                                }
                            }
                        }
                        folderForEsign.parties = parties;
                        folderForEsign.fields = fields;
                        var SerializedClass = JsonConvert.SerializeObject(folderForEsign);
                        streamWriter.Write(SerializedClass);
                        streamWriter.Flush();
                    }
                }
                if (mails != "")
                {
                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        Esign.Root myDeserializedClass = JsonConvert.DeserializeObject<Esign.Root>(result);
                        if (myDeserializedClass.embeddedSigningSessions != null)
                        {
                            //string logs = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");
                            //if (!System.IO.File.Exists(logs))
                            //{
                            //    FileStream fs1 = new FileStream(logs, FileMode.OpenOrCreate, FileAccess.Write);
                            //}
                            //StreamWriter writer = new StreamWriter(logs, true);
                            //writer.WriteLine("test my data nitin -: error=" + result.ToString() + DateTime.Now.ToString());

                            foreach (var sign in myDeserializedClass.embeddedSigningSessions)
                            {
                                CreateOrEditE_SignRecordDto std = new CreateOrEditE_SignRecordDto();
                                var folder = myDeserializedClass.folder;
                                {
                                    std.FolderId = folder.folderId;
                                    std.FolderName = folder.folderName;
                                    std.FolderPassword = (string)folder.folderPassword;
                                    std.FileName = key;
                                    std.Status = "Unsigned";
                                    foreach (var doc in myDeserializedClass.folder.documentsList)
                                    {
                                        std.DocumentId = 1;
                                        std.ContractId = doc.contractId;

                                        std.CompanyId = doc.companyId;
                                    }
                                }
                                std.EmailId = sign.emailIdOfSigner;
                                std.EmbeddedToken = sign.embeddedToken;
                                std.EmbeddedURL = sign.embeddedSessionURL;
                                var i = _e_SignRecordsAppService.CreateOrEdit(std);
                            }
                        }
                    }
                }
                return res;
                #endregion
            }
            catch (Exception ex)
            {
                string logs = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");
                if (!System.IO.File.Exists(logs))
                {
                    FileStream fs1 = new FileStream(logs, FileMode.OpenOrCreate, FileAccess.Write);
                }
                StreamWriter writer = new StreamWriter(logs, true);
                writer.WriteLine("Error in E_Sign method for -: error=" + ex.ToString() + DateTime.Now.ToString());
                writer.Close();
                return null;
            }
        }

        ///<Summary>
        /// Record status of file
        ///</Summary>
        public void DocumentRecord(string filename, string type, long userId, long srEscrowFileMasterId)

        {
            SrAssignedFilesDetail srAssign = new SrAssignedFilesDetail();
            var temp = _srAssignedFilesDetailRepository.GetAll().Where(x => x.FileName == filename && x.UserId == userId).FirstOrDefault();
            if (temp != null)
            {
                if (type == "Read")
                {
                    temp.ReadStatus = "Unread";
                }
                if (type == "Input")
                {
                    temp.InputStatus = "Input Incomplete";
                }
                if (type == "Sign")
                {
                    temp.SigningStatus = "Unsigned";
                }
                temp.UpdatedOn = DateTime.UtcNow;
                srAssign = temp;
            }
            else
            {
                srAssign.UserId = userId;
                srAssign.FileName = filename;
                if (type == "Read")
                {
                    srAssign.ReadStatus = "Unread";
                }
                else if (type == "Input")
                {
                    srAssign.InputStatus = "Input Incomplete";
                }
                else
                {
                    srAssign.SigningStatus = "Unsigned";
                }
                srAssign.UpdatedOn = DateTime.UtcNow;
                srAssign.SrEscrowFileMasterId = srEscrowFileMasterId;
            }

            var insert = _srAssignedFilesDetailRepository.InsertOrUpdateAndGetId(srAssign);
        }
        ///<Summary>
        /// Send Notification to the users
        ///</Summary>
        public void Publish_Signing_Notification(string filename, string name)
        {
            try
            {
                var sign = _esignRepository.GetAll().ToList();
                var detail = _escrowDetailRepository.GetAll().ToList();
                var user = _userRepository.GetAll().ToList();
                var JoinResult = (from p in sign.AsEnumerable()
                                  join t in detail.AsEnumerable()
                                  on p.EmailId equals t.Email
                                  join u in user.AsEnumerable()
                                  on p.EmailId equals u.EmailAddress
                                  where p.FileName == filename
                                  select new EsignNameStatus()
                                  {
                                      UserId = u.Id,
                                      Name = t.Name,
                                      Status = p.Status
                                  }).ToList();

                foreach (var data in JoinResult)
                {
                    string userid = data.UserId.ToString();
                    var ghh = UserIdentifier.Parse(userid);
                    string message = "Doc signed by " + name;


                    _notificationPublisher.PublishAsync("NotificationTest", new MessageNotificationData(message), severity: NotificationSeverity.Info, userIds: new[] { ghh });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        ///<Summary>
        /// Create new users
        ///</Summary>
        protected long? createUser(UserData userData)
        {
            try
            {
                string temp = String.Empty;
                string types = String.Empty;
                string firstname = String.Empty;
                string surname = String.Empty;
                CreateOrUpdateUserInput obj = new CreateOrUpdateUserInput();
                CreateOrEditEscrowClientDto o = new CreateOrEditEscrowClientDto();
                EscrowClientsAppService item = new EscrowClientsAppService(_escrowClientRepository);
                CreateOrEditEscrowUserMappingDto coeeum = new CreateOrEditEscrowUserMappingDto();
                CreateOrEditSrFileMappingDto coesfm = new CreateOrEditSrFileMappingDto();
                CreateOrEditEscrowDetailDto createOrEditEscrowDetail = new CreateOrEditEscrowDetailDto();
                CreateOrEditSrInvitationRecordDto createOrEditSrInvitationRecordDto = new CreateOrEditSrInvitationRecordDto();
                CreateOrEditSRInviteeDto createOrEditSRInviteeDto = new CreateOrEditSRInviteeDto();
                #region
                if (userData.name.Contains(" "))
                {
                    char[] separator = { ' ' };
                    string[] strlists = userData.name.Split(separator, StringSplitOptions.None);
                    firstname = strlists[0];
                    surname = strlists[1];
                    if (strlists.Length > 2)
                    {
                        if (strlists[2] != "")
                        {
                            surname += " " + strlists[2];
                        }
                    }
                }
                else
                {
                    firstname = userData.name;
                    surname = "";
                }
                #endregion
                var User = new UserEditDto();
                //in
                //0t escrowclient = Convert.ToInt
                //32(userData.escro);
                #region
                User.Id = null;
                User.UserName = userData.fromEmail.ToString();
                User.Name = firstname.Trim();
                User.Surname = surname;
                User.EmailAddress = userData.fromEmail.ToString();
                User.IsActive = true;
                #endregion
                #region
                obj.SetRandomPassword = true;
                obj.SendActivationEmail = true;
                obj.AssignedRoleNames = new string[] { "Admin" };
                #endregion
                #region
                o.Name = userData.name.ToString();
                o.Email = userData.fromEmail.ToString();
                o.EscrowNumber = userData.escro.ToString();
                #endregion
                User.ShouldChangePasswordOnNextLogin = true;
                obj.User = User;
                User user = new User();
                user.IsActive = User.IsActive;
                user.UserName = User.UserName;
                user.Name = User.Name;
                user.Surname = User.Surname;
                user.EmailAddress = User.EmailAddress;
                user.NormalizedEmailAddress = User.EmailAddress;
                user.NormalizedUserName = User.Name;
                user.ShouldChangePasswordOnNextLogin = true;
                int length = 5;
                const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
                StringBuilder res = new StringBuilder();
                Random rnd = new Random();
                while (0 < length--)
                {
                    res.Append(valid[rnd.Next(valid.Length)]);
                }

                string randpass = _passwordHasher.HashPassword(user, res.ToString());
                user.Password = randpass;
                #region
                passw = res.ToString();
                username = User.UserName;
                #endregion
                var xxx = _userRepository.InsertAndGetId(user);
                var enterprises = _enterpriseRepository.GetAll().Where(x => x.EnterpriseName == userData.company || x.Subcompany == userData.company).FirstOrDefault();
                var escrowdata = _ISrEscrowRepository.GetAll().Where(x => x.EscrowNo == userData.escro && x.EnterpriseId == enterprises.Id).FirstOrDefault();
                string esmail = "", esaddress = "", esphone = "", escellphone = "";
                if (escrowdata != null)
                {
                    esmail = escrowdata.EOEmail;
                    esaddress = escrowdata.PropertyAddress;
                    escellphone = escrowdata.EoPhoneCell;
                    esphone = escrowdata.EOPhone;
                }
                MatchCollection matchess = regexx.Matches(userData.type);
                for (int i = 0; i < matchess.Count; i++)
                {
                    string typ = matchess[i].Value.Replace("{", "");
                    string typess = matchess[i].Value.Replace("{", "");
                    typess = typess.Replace("}", "");

                    if (types != "")
                    {
                        types += "," + typess;
                    }
                    else
                    {
                        types = typess;
                    }

                }
                createOrEditEscrowDetail.Company = userData.company;
                createOrEditEscrowDetail.Email = userData.fromEmail;
                createOrEditEscrowDetail.EscrowId = userData.escro;
                createOrEditEscrowDetail.Name = userData.name;
                createOrEditEscrowDetail.UserId = (int)xxx;
                createOrEditEscrowDetail.Usertype = types;
                var Escrod = _IescrowDetailsAppService.CreateOrEdit(createOrEditEscrowDetail);
                createOrEditSrInvitationRecordDto.Email = userData.fromEmail;
                createOrEditSrInvitationRecordDto.EscrowCompany = userData.company;
                createOrEditSrInvitationRecordDto.DomainAccessInstance = null;
                createOrEditSrInvitationRecordDto.EscrowContactEmail = null;
                createOrEditSrInvitationRecordDto.EscrowNumber = userData.escro;
                createOrEditSrInvitationRecordDto.EscrowOfficer = userData.invitee;
                createOrEditSrInvitationRecordDto.EscrowOfficerPhoneNumber = null;
                createOrEditSrInvitationRecordDto.UserId = (int)xxx;
                createOrEditSrInvitationRecordDto.Usertype = userData.type;
                var invirecord = _ISrInvitationRecordsAppService.CreateOrEdit(createOrEditSrInvitationRecordDto);
                UserRole role = new UserRole();
                role.CreationTime = DateTime.UtcNow;
                role.UserId = xxx;
                role.RoleId = 15;
                var rrole = _roleRepository.Insert(role);
                List<String> files = new List<String>();
                try
                {
                    if (finalstring != "" && finalstring != null)
                    {
                        string[] spearator = { "%$%" };
                        string accesslevel = "", temp1 = "", temp2 = "", temp3 = "";
                        int no = 1;
                        string[] strlist = finalstring.Split(spearator, StringSplitOptions.None);
                        string[] strlist1 = pdfname.Split(spearator, StringSplitOptions.None);
                        if (temp == "")
                        {
                            foreach (string itempdf in strlist)
                            {
                                var getMail = itempdf.Split(new string[] { "((", "))" }, StringSplitOptions.RemoveEmptyEntries);
                                var trim = Regex.Replace(itempdf, @"\(\([^()]*\)\)", string.Empty);
                                if (userData.fromEmail == getMail[0])
                                {
                                    trim = trim.Replace("()", "");
                                    string access = "";
                                    if (trim.Contains("~'~"))
                                    {
                                        access = trim.Substring(trim.LastIndexOf("~'~") + 3);
                                    }
                                    else if (trim.Contains("_'_"))
                                    {
                                        access = trim.Substring(trim.LastIndexOf("_'_") + 3);
                                    }
                                    else if (trim.Contains("-'-"))
                                    {
                                        access = trim.Substring(trim.LastIndexOf("-'-") + 3);
                                    }
                                    string match = "";
                                    MatchCollection matches = regexx.Matches(access);
                                    for (int i = 0; i < matches.Count; i++)
                                    {
                                        string my = matches[i].Value.Replace("{", "");
                                        my = my.Replace("}", "");
                                        int index = my.IndexOf('-');
                                        match = my.Substring(0, index);
                                        MatchCollection matchesss = regexx.Matches(userData.type);
                                        for (int x = 0; x < matchesss.Count; x++)
                                        {
                                            string mmy = matchesss[x].Value.Replace("{", "");
                                            mmy = mmy.Replace("}", "");

                                            if (temp3 == "")
                                            {
                                                temp3 = "filled";
                                                accesslevel = matches[i].Value;
                                                string splitstr = trim.Substring(0, trim.LastIndexOf('\\'));
                                                string strwithoutexten = System.IO.Path.GetFileNameWithoutExtension(splitstr);
                                                fileslist += no + ". " + strwithoutexten + "\n\n";
                                                coesfm.FileName = Path.Combine(splitstr + "\\Other");
                                                coesfm.UserId = (int)xxx;
                                                coesfm.Action = accesslevel;
                                                coesfm.EscrowiId = userData.escro.Trim();
                                                coesfm.IsActive = true;



                                                var filemap = _ISrFileMappingsAppService.CreateOrEdit(coesfm);
                                            }

                                            if (mmy == "SR1" || mmy == "SR2" || mmy == "SR3" || mmy == "SR4" || mmy == "SR5" || mmy == "SR6" || mmy == "SR7" || mmy == "SR8" || mmy == "SR9" || mmy == "SR10" || mmy == "SRX")
                                            {
                                                if (temp1 == "")
                                                {
                                                    temp1 = "filled";
                                                    string splitstr = trim.Substring(0, trim.LastIndexOf('\\'));
                                                    string strwithoutexten = System.IO.Path.GetFileNameWithoutExtension(splitstr);
                                                    fileslist += no + ". " + strwithoutexten + "\n\n";
                                                    coesfm.FileName = Path.Combine(splitstr + "\\(0)0000aaaSeller Opening Documents.txt");
                                                    coesfm.UserId = (int)xxx;
                                                    coesfm.Action = "";
                                                    coesfm.EscrowiId = userData.escro.Trim();
                                                    coesfm.IsActive = true;

                                                    var filemap = _ISrFileMappingsAppService.CreateOrEdit(coesfm);
                                                }
                                            }
                                            else if (mmy == "BR1" || mmy == "BR2" || mmy == "BR3" || mmy == "BR4" || mmy == "BR5" || mmy == "BR6" || mmy == "BR7" || mmy == "BR8" || mmy == "BR9" || mmy == "BR10" || mmy == "BRX")
                                            {
                                                if (temp2 == "")
                                                {
                                                    temp2 = "filled";
                                                    string splitstr = trim.Substring(0, trim.LastIndexOf('\\'));
                                                    string strwithoutexten = System.IO.Path.GetFileNameWithoutExtension(splitstr);
                                                    fileslist += no + ". " + strwithoutexten + "\n\n";
                                                    coesfm.FileName = Path.Combine(splitstr + "\\(0)0000aaaBuyer Opening Documents.txt");
                                                    coesfm.UserId = (int)xxx;
                                                    coesfm.Action = "";
                                                    coesfm.EscrowiId = userData.escro.Trim();
                                                    coesfm.IsActive = true;
                                                    var filemap = _ISrFileMappingsAppService.CreateOrEdit(coesfm);
                                                }
                                            }
                                            if (match == mmy)
                                            {
                                                accesslevel = matches[i].Value;
                                                string strcheck = accesslevel.Replace("{", "");
                                                strcheck = strcheck.Replace("}", "");
                                                strcheck = strcheck.Substring(strcheck.LastIndexOf("-") + 1);
                                                if (strcheck.Contains("R"))
                                                {
                                                    temp = "filled";
                                                    string splitstr = trim.Substring(trim.LastIndexOf('\\') + 1);
                                                    if (!strcheck.Contains("S") && strcheck.Contains("E"))
                                                    {
                                                        DocumentRecord(splitstr, "Input", user.Id, 0);
                                                    }
                                                    else if (!strcheck.Contains("S") && !strcheck.Contains("E"))
                                                    {
                                                        DocumentRecord(splitstr, "Read", user.Id, 0);
                                                    }
                                                    if (strcheck.Contains("S"))
                                                    {
                                                        var path = trim.Substring(0, trim.LastIndexOf("\\"));
                                                        path = path.Substring(trim.IndexOf("Paperless\\") + 10);
                                                        //DocumentRecord(splitstr, "Sign", user.Id);
                                                        var sign = E_Sign(path.Trim(), splitstr, user.Id.ToString());
                                                    }
                                                    string strwithoutexten = System.IO.Path.GetFileNameWithoutExtension(splitstr);
                                                    fileslist += no + ". " + strwithoutexten + "\n\n";
                                                    coesfm.FileName = Path.Combine(trim);
                                                    coesfm.UserId = (int)xxx;
                                                    coesfm.Action = accesslevel;
                                                    coesfm.EscrowiId = userData.escro.Trim();
                                                    coesfm.IsActive = true;
                                                    var filemap = _ISrFileMappingsAppService.CreateOrEdit(coesfm);
                                                    no += 1;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (System.Exception excpt)
                {
                    string logs = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");
                    if (!System.IO.File.Exists(logs))
                    {
                        FileStream fs1 = new FileStream(logs, FileMode.OpenOrCreate, FileAccess.Write);
                    }
                    StreamWriter writer = new StreamWriter(logs, true);
                    writer.WriteLine("Error in createUser method for -: " + userData.fromEmail + " error=" + excpt.ToString() + DateTime.Now.ToString());
                    writer.Close();
                    return 0;
                }
                var escroentry = item.CreateOrEdit(o);
                return 1;
            }
            catch (Exception ex)
            {
                string logs = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");
                if (!System.IO.File.Exists(logs))
                {
                    FileStream fs1 = new FileStream(logs, FileMode.OpenOrCreate, FileAccess.Write);
                }
                StreamWriter writer = new StreamWriter(logs, true);
                writer.WriteLine("Error in createUser method for -: " + userData.fromEmail + " error=" + ex.ToString() + DateTime.Now.ToString());
                writer.Close();
                return 0;
            }
        }

        ///<Summary>
        /// Sending mail for new user
        ///</Summary>
        public async Task<responseData> SendMailEscrow(UserData userData)
        {
            responseData res = new responseData();
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("Noreply@EscrowBasePortal.com");
                mail.To.Add(userData.fromEmail);
                mail.Subject = "Invitation";
                string Mailid = conf["Email:Mailid"].ToString();
                string MailPwd = conf["Email:MailPwd"].ToString();
                string MailHost = conf["Email:MailHost"].ToString();
                int MailPort = Convert.ToInt32(conf["Email:MailPort"].ToString());
                bool MailSSL = Convert.ToBoolean(conf["Email:MailSSL"].ToString());

                string referer = conf["App:ClientRootAddress"].ToString();
                var enterprises = _enterpriseRepository.GetAll().Where(x => x.EnterpriseName == userData.company || x.Subcompany == userData.company).FirstOrDefault();
                var escrowdata = _ISrEscrowRepository.GetAll().Where(x => x.EscrowNo == userData.escro && x.SubCompanyName == userData.company).FirstOrDefault();
                var token = await _tokenAuthControler.AuthenticateByEmail(username, passw);
                string resetcode = token.resetpasswordtoken;
                string esmail = "", esaddress = "", esphone = "", escellphone = "", esname = "", ext = "", companyh = "", link = "", Fulltype = "";
                if (escrowdata != null)
                {
                    esmail = escrowdata.EOEmail;
                    esaddress = escrowdata.PropertyAddress;
                    escellphone = escrowdata.EoPhoneCell;
                    esphone = escrowdata.EOPhone;
                    esname = escrowdata.EscrowOfficerName;
                    ext = escrowdata.EoPhoneExt;
                    companyh = userData.company;
                    if (escrowdata.Logo != "")
                    {
                        link = escrowdata.Logo;
                    }
                }
                MatchCollection matchess = regexx.Matches(userData.type);
                for (int i = 0; i < matchess.Count; i++)
                {
                    string types = matchess[i].Value.Replace("{", "").Replace("}", "");
                    if (types.Trim() == /*"LRA"*/"RAL")
                    {
                        if (Fulltype != "")
                        {
                            Fulltype += ", Listing Real Estate Agent";
                        }
                        else
                        {
                            Fulltype = "Listing Real Estate Agent";
                        }
                    }
                    if (types.Trim() == /*"LRB"*/"RBL")
                    {
                        if (Fulltype != "")
                        {
                            Fulltype += ", Listing Real Estate Broker";
                        }
                        else
                        {
                            Fulltype = "Listing Real Estate Broker";
                        }
                    }
                    if (types.Trim() == /*"SRA"*/"RAS")
                    {
                        if (Fulltype != "")
                        {
                            Fulltype += ", Selling Real Estate Agent";
                        }
                        else
                        {
                            Fulltype = "Selling Real Estate Agent";
                        }
                    }
                    if (types.Trim() == /*"SRB"*/"RBS")
                    {
                        if (Fulltype != "")
                        {
                            Fulltype += ", Selling Real Estate Broker";
                        }
                        else
                        {
                            Fulltype = "Selling Real Estate Broker";
                        }
                    }
                    if (types.Trim() == "RAO")
                    {
                        if (Fulltype != "")
                        {
                            Fulltype += ", Other Real Estate Agent";
                        }
                        else
                        {
                            Fulltype = "Other Real Estate Agent";
                        }
                    }
                    if (types.Trim() == "RBO")
                    {
                        if (Fulltype != "")
                        {
                            Fulltype += ", Other Real Estate Broker";
                        }
                        else
                        {
                            Fulltype = "Other Real Estate Broker";
                        }
                    }
                    if (types.Trim() == "BR1" || types.Trim() == "BR2" || types.Trim() == "BR3" || types.Trim() == "BR4" || types.Trim() == "BR5" || types.Trim() == "BR6" || types.Trim() == "BR7" || types.Trim() == "BR8" || types.Trim() == "BR9" || types.Trim() == "BR10" || types.Trim() == "BRX")
                    {
                        if (Fulltype != "")
                        {
                            Fulltype += ", Buyer";
                        }
                        else
                        {
                            Fulltype = "Buyer";
                        }
                    }
                    if (types.Trim() == "SR1" || types.Trim() == "SR2" || types.Trim() == "SR3" || types.Trim() == "SR4" || types.Trim() == "SR5" || types.Trim() == "SR6" || types.Trim() == "SR7" || types.Trim() == "SR8" || types.Trim() == "SR9" || types.Trim() == "SR10" || types.Trim() == "SRX")
                    {
                        if (Fulltype != "")
                        {
                            Fulltype += ", Seller";
                        }
                        else
                        {
                            Fulltype = "Seller";
                        }
                    }
                    if (types.Trim() == "LBX")
                    {
                        if (Fulltype != "")
                        {
                            Fulltype += ", Mortgage/Loan Broker";
                        }
                        else
                        {
                            Fulltype = "Mortgage/Loan Broker";
                        }
                    }
                    if (types.Trim() == "LBP")
                    {
                        if (Fulltype != "")
                        {
                            Fulltype += ", Loan Broker Processor";
                        }
                        else
                        {
                            Fulltype = "Loan Broker Processor";
                        }
                    }
                    if (types.Trim() == "TC1" || types.Trim() == "TC2" || types.Trim() == "TC3" || types.Trim() == "TC4" || types.Trim() == "TC5" || types.Trim() == "TC6" || types.Trim() == "TC7" || types.Trim() == "TC8" || types.Trim() == "TC9" || types.Trim() == "TC10" || types.Trim() == "TCX" || types.Trim() == "TCL" || types.Trim() == "TCO" || types.Trim() == "TCS")
                    {
                        if (Fulltype != "")
                        {
                            Fulltype += ", Transaction Coordinator";
                        }
                        else
                        {
                            Fulltype = "Transaction Coordinator";
                        }
                    }
                    if (types.Trim() == "LR1" || types.Trim() == "LR2" || types.Trim() == "LR3")
                    {
                        if (Fulltype != "")
                        {
                            Fulltype += ", Lender";
                        }
                        else
                        {
                            Fulltype = "Lender";
                        }
                    }
                    if (types.Trim() == "LP1" || types.Trim() == "LP2" || types.Trim() == "LP3")
                    {
                        if (Fulltype != "")
                        {
                            Fulltype += ", Loan Processor";
                        }
                        else
                        {
                            Fulltype = "Loan Processor";
                        }
                    }
                    //if (types.Trim() == "TCX")
                    //{
                    //    if (Fulltype != "")
                    //    {
                    //        Fulltype += ", Title Company";
                    //    }
                    //    else
                    //    {
                    //        Fulltype = "Title Company";
                    //    }
                    //}

                    if (types.Trim() == "TCA")
                    {
                        if (Fulltype != "")
                        {
                            Fulltype += ", Trans. Crdntr. Assistant";
                        }
                        else
                        {
                            Fulltype = "Trans. Crdntr. Assistant";
                        }
                    }
                    if (types.Trim() == "EO1")
                    {
                        if (Fulltype != "")
                        {
                            Fulltype += ", The Escrow Officer";
                        }
                        else
                        {
                            Fulltype = "The Escrow Officer";
                        }
                    }
                    if (types.Trim() == "EA1")
                    {
                        if (Fulltype != "")
                        {
                            Fulltype += ", The Escrow Assistant";
                        }
                        else
                        {
                            Fulltype = "The Escrow Assistant";
                        }
                    }
                    if (types.Trim() == "EOX")
                    {
                        if (Fulltype != "")
                        {
                            Fulltype += ", an Escrow Officer";
                        }
                        else
                        {
                            Fulltype = "an Escrow Officer";
                        }
                    }
                    if (types.Trim() == "EAX")
                    {
                        if (Fulltype != "")
                        {
                            Fulltype += ", an Escrow Assistant";
                        }
                        else
                        {
                            Fulltype = "an Escrow Assistant";
                        }
                    }
                    if (types.Trim() == "LTC")
                    {
                        if (Fulltype != "")
                        {
                            Fulltype += ",an Listing Trans. Coordinator";
                        }
                        else
                        {
                            Fulltype = "an Listing Trans. Coordinator";
                        }
                    }
                    if (types.Trim() == "STC")
                    {
                        if (Fulltype != "")
                        {
                            Fulltype += ", an Selling Trans. Coordinator";
                        }
                        else
                        {
                            Fulltype = "an Selling Trans. Coordinator";
                        }
                    }
                    if (types.Trim() == "OTC")
                    {
                        if (Fulltype != "")
                        {
                            Fulltype += ", an Listing Trans. Coordinator";
                        }
                        else
                        {
                            Fulltype = "an Listing Trans. Coordinator";
                        }
                    }
                }
                byte[] b = System.Text.ASCIIEncoding.ASCII.GetBytes(Convert.ToString(token.userid));
                string encryptedUserID = Convert.ToBase64String(b);
                var userid = encryptedUserID;
                byte[] uu = System.Text.ASCIIEncoding.ASCII.GetBytes(Convert.ToString(userData.fromEmail));
                string encryptedUserName = Convert.ToBase64String(uu);
                var un = encryptedUserName;
                byte[] c = System.Text.ASCIIEncoding.ASCII.GetBytes(userData.name);
                string encrypted = Convert.ToBase64String(c);
                var Name = encrypted;
                string texts = "";
                using (StreamReader reader = System.IO.File.OpenText("wwwroot\\Email.html")) // Path to your Email format
                {
                    texts = reader.ReadToEnd();
                    texts = texts.Replace("$$NameofUser$$", userData.name).Replace("$$CompanyH$$", companyh).Replace("$$Link$$", link).Replace("$$UserType$$", Fulltype).Replace("$$Ext$$", ext).Replace("$$EoPhone$$", esphone).Replace("$$EscrowNo$$", userData.escro).Replace("$$NameofEscrowOfficer$$", esname).Replace("$$EoMail$$", esmail).Replace("$$EoPhoneCell$$", escellphone).Replace("$$Company$$", userData.company).Replace("$$EoAddress$$", esaddress).Replace("&&referer$$", referer).Replace("$$userid$$", userid).Replace("&&resetcode$$", resetcode).Replace("$$Name$$", Name).Replace("$$un$$", un).Replace("$$Usermail$$", userData.fromEmail);
                }
                mail.IsBodyHtml = true;
                mail.Body = texts;
                SmtpClient SmtpServer = new SmtpClient();
                SmtpServer.Port = MailPort;
                SmtpServer.Credentials = new System.Net.NetworkCredential(Mailid, MailPwd);
                SmtpServer.Host = MailHost;
                SmtpServer.EnableSsl = MailSSL;
                SmtpServer.Send(mail);
                string logs = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");
                if (!System.IO.File.Exists(logs))
                {
                    FileStream fs1 = new FileStream(logs, FileMode.OpenOrCreate, FileAccess.Write);
                }
                StreamWriter writer = new StreamWriter(logs, true);
                writer.WriteLine("New user mail sent for -: " + userData.fromEmail + DateTime.Now.ToString());
                writer.Close();
                res.message = "Mail Sent";
                return res;
            }
            catch (Exception ex)
            {
                string logs = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");
                if (!System.IO.File.Exists(logs))
                {
                    FileStream fs1 = new FileStream(logs, FileMode.OpenOrCreate, FileAccess.Write);
                }
                StreamWriter writer = new StreamWriter(logs, true);
                writer.WriteLine("Error occured in SendMailEscrow method for -: " + userData.fromEmail + " error-:" + ex.ToString() + DateTime.Now.ToString());
                writer.Close();
                res.message = $"Mail Not Sent { userData.fromEmail}";
                return res;
            }
        }

        ///<Summary>
        /// Sending mail for update user
        ///</Summary>
        public responseData SendMailUpdate(UserData userData)
        {
            responseData res = new responseData();
            string filess = String.Empty;
            string referer = conf["App:ClientRootAddress"].ToString();
            string Mailid = conf["Email:Mailid"].ToString();
            string MailPwd = conf["Email:MailPwd"].ToString();
            string MailHost = conf["Email:MailHost"].ToString();
            int MailPort = Convert.ToInt32(conf["Email:MailPort"].ToString());
            bool MailSSL = Convert.ToBoolean(conf["Email:MailSSL"].ToString());
            string temp = "";
            var check = _userRepository.GetAll().Where(x => x.UserName == userData.fromEmail && x.IsEmailConfirmed == true).FirstOrDefault();
            if (check == null)

            {
                res.message = "Email Not Valid OR Invitation Not Accepted Yet";
                return res;
            }
            var checknew = _escrowDetailRepository.GetAll().Where(x => x.EscrowId == userData.escro).FirstOrDefault();
            CreateOrEditSrFileMappingDto coesfm = new CreateOrEditSrFileMappingDto();
            CreateOrEditEscrowDetailDto createOrEditEscrowDetail = new CreateOrEditEscrowDetailDto();
            CreateOrEditSrInvitationRecordDto createOrEditSrInvitationRecordDto = new CreateOrEditSrInvitationRecordDto();
            List<String> files = new List<String>();
            #region
            try
            {
                if (finalstring != "" && finalstring != null)
                {
                    string[] separator = { "%$%" };
                    string accesslevel = "";
                    int no = 1;
                    string[] strlist = finalstring.Split(separator, StringSplitOptions.None);
                    string[] strlist1 = pdfname.Split(separator, StringSplitOptions.None);
                    if (temp == "")
                    {
                        foreach (string itempdf in strlist)
                        {
                            var getMail = itempdf.Split(new string[] { "((", "))" }, StringSplitOptions.RemoveEmptyEntries);
                            var trim = Regex.Replace(itempdf, @"\(\([^()]*\)\)", string.Empty);
                            if (userData.fromEmail == getMail[0])
                            {
                                trim = trim.Replace("()", "");
                                string access = trim.Substring(trim.LastIndexOf("~'~") + 3);
                                string match = "", trunk = "";
                                Regex regex = new Regex(@"\{.*?\}");
                                MatchCollection matches = regex.Matches(access);
                                for (int i = 0; i < matches.Count; i++)
                                {
                                    string my = matches[i].Value.Replace("{", "").Replace("}", "");
                                    int index = my.IndexOf('-');
                                    match = my.Substring(0, index);
                                    if (match == userData.type)
                                    {
                                        accesslevel = matches[i].Value;
                                        string strcheck = accesslevel.Replace("{", "").Replace("}", "");
                                        strcheck = strcheck.Substring(strcheck.LastIndexOf("-") + 1);
                                        string splitstr = trim.Substring(trim.LastIndexOf('\\') + 1);
                                        if (!strcheck.Contains("S") && strcheck.Contains("E"))
                                        {
                                            DocumentRecord(splitstr, "Input", check.Id, 0);
                                        }
                                        else if (!strcheck.Contains("S") && !strcheck.Contains("E"))
                                        {
                                            DocumentRecord(splitstr, "Read", check.Id, 0);
                                        }
                                        if (strcheck.Contains("S"))
                                        {
                                            if (trunk == "")
                                            {
                                                trunk = "filled";
                                                var path = trim.Substring(0, trim.LastIndexOf("\\"));
                                                path = path.Substring(trim.IndexOf("Paperless\\") + 10);
                                                var sign = E_Sign(path.Trim(), splitstr, check.Id.ToString());
                                            }
                                        }
                                        string strwithoutexten = System.IO.Path.GetFileNameWithoutExtension(splitstr);
                                        filess += no + ". " + strwithoutexten + "\n\n";
                                        coesfm.FileName = Path.Combine(trim);
                                        coesfm.UserId = (int)check.Id;
                                        coesfm.Action = accesslevel;
                                        coesfm.EscrowiId = userData.escro;
                                        coesfm.IsActive = true;
                                        var filemap = _ISrFileMappingsAppService.CreateOrEdit(coesfm);
                                        no += 1;
                                    }

                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception excpt)
            {
                string logs = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");
                if (!System.IO.File.Exists(logs))
                {
                    FileStream fs1 = new FileStream(logs, FileMode.OpenOrCreate, FileAccess.Write);
                }
                StreamWriter writer = new StreamWriter(logs, true);
                writer.WriteLine("Error in SendMailUpdate method for -: " + userData.fromEmail + " error=" + excpt.ToString() + DateTime.Now.ToString());
                writer.Close();
            }
            #endregion

            try
            {
                var entrycheckescrow = _escrowDetailRepository.GetAll();//.Where(x => x.Name == userData.name && ValidFileName(x.Company) == userData.company && x.Email == userData.fromEmail && x.EscrowId == userData.escro && x.Usertype == userData.type).FirstOrDefault();

                EscrowDetail escroDetail = null;
                foreach (var ent in entrycheckescrow)
                {
                    if (ent.Name == userData.name && ValidFileName(ent.Company) == userData.company && ent.Email == userData.fromEmail && ent.EscrowId == userData.escro && ent.Usertype == userData.type)
                    {
                        escroDetail = ent;

                    }
                }
                var escrowdetails = _escrowDetailRepository.GetAll();//.Whe re(x => x.Company == userData.company && x.Email == userData.fromEmail && x.EscrowId == userData.escro).FirstOrDefault();
                foreach (var ent in escrowdetails)
                {
                    if (ValidFileName(ent.Company) == userData.company && ent.Email == userData.fromEmail && ent.EscrowId == userData.escro && ent.Usertype == userData.type)
                    {
                        escroDetail = ent;
                    }
                }
                if (escroDetail == null)
                {
                    if (getid != 0)
                    {
                        createOrEditEscrowDetail.Id = getid;
                        createOrEditEscrowDetail.Company = userData.company;
                        createOrEditEscrowDetail.Email = userData.fromEmail;
                        createOrEditEscrowDetail.EscrowId = userData.escro;
                        createOrEditEscrowDetail.Name = userData.name;
                        createOrEditEscrowDetail.UserId = (int)check.Id;
                        createOrEditEscrowDetail.Usertype = typestore + "," + userData.type;
                        var Escrod = _IescrowDetailsAppService.CreateOrEdit(createOrEditEscrowDetail);
                    }
                    else
                    {
                        if (escroDetail == null)
                        {
                            createOrEditEscrowDetail.Company = userData.company;
                            createOrEditEscrowDetail.Email = userData.fromEmail;
                            createOrEditEscrowDetail.EscrowId = userData.escro;
                            createOrEditEscrowDetail.Name = userData.name;
                            createOrEditEscrowDetail.UserId = (int)check.Id;
                            createOrEditEscrowDetail.Usertype = userData.type;
                            typestore = userData.type;
                            EscrowDetail obj = new EscrowDetail();
                            obj.Company = createOrEditEscrowDetail.Company;

                            obj.Email = createOrEditEscrowDetail.Email;
                            obj.EscrowId = createOrEditEscrowDetail.EscrowId;
                            obj.Name = createOrEditEscrowDetail.Name;
                            obj.UserId = createOrEditEscrowDetail.UserId;
                            obj.Usertype = createOrEditEscrowDetail.Usertype;
                            var gotid = _escrowDetailRepository.InsertAndGetId(obj);
                            getid = (int)gotid;
                        }
                        else
                        {
                            createOrEditEscrowDetail.Id = escroDetail.Id;
                            createOrEditEscrowDetail.Company = userData.company;
                            createOrEditEscrowDetail.Email = userData.fromEmail;
                            createOrEditEscrowDetail.EscrowId = userData.escro;
                            createOrEditEscrowDetail.Name = userData.name;
                            createOrEditEscrowDetail.UserId = (int)check.Id;
                            createOrEditEscrowDetail.Usertype = escroDetail.Usertype + "," + userData.type;
                            var Escrod = _IescrowDetailsAppService.CreateOrEdit(createOrEditEscrowDetail);
                        }
                    }
                    var entrycheckinviterecord = _srInvitationRecordRepository.GetAll();//.Where(x => x.EscrowCompany == userData.company && x.Email == userData.fromEmail && x.EscrowNumber == userData.escro && x.UserId == (int)check.Id && x.Usertype == userData.type).FirstOrDefault();
                    SrInvitationRecord CheckInvite = null;
                    foreach (var et in entrycheckinviterecord)
                    {
                        if (ValidFileName(et.EscrowCompany) == userData.company && et.Email == userData.fromEmail && et.EscrowNumber == userData.escro && et.UserId == (int)check.Id && et.Usertype == userData.type)
                        {
                            CheckInvite = et;
                        }
                    }

                    if (CheckInvite == null)
                    {
                        createOrEditSrInvitationRecordDto.Email = userData.fromEmail;
                        createOrEditSrInvitationRecordDto.EscrowCompany = userData.company;
                        createOrEditSrInvitationRecordDto.DomainAccessInstance = null;
                        createOrEditSrInvitationRecordDto.EscrowContactEmail = null;
                        createOrEditSrInvitationRecordDto.EscrowNumber = userData.escro;
                        createOrEditSrInvitationRecordDto.EscrowOfficer = userData.invitee;
                        createOrEditSrInvitationRecordDto.EscrowOfficerPhoneNumber = null;
                        createOrEditSrInvitationRecordDto.UserId = (int)check.Id;
                        createOrEditSrInvitationRecordDto.Usertype = userData.type;
                        var invirecord = _ISrInvitationRecordsAppService.CreateOrEdit(createOrEditSrInvitationRecordDto);
                    }
                }

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("softwarereality@gmail.com");
                mail.To.Add(userData.fromEmail);
                mail.Subject = "Updation";
                byte[] c = System.Text.ASCIIEncoding.ASCII.GetBytes(userData.fromEmail);
                string encrypted = Convert.ToBase64String(c);
                var UserName = encrypted;
                string tempfiles = "";
                if (filess != "")
                {
                    var idx = filess.IndexOf("~'~");
                    tempfiles = filess.Substring(0, idx);
                }

                var escrowdata = _ISrEscrowRepository.GetAll().Where(x => x.EscrowNo == userData.escro && x.SubCompanyName == userData.company).FirstOrDefault();
                string esmail = "", esaddress = "", esphone = "", escellphone = "", esname = "", ext = "", companyh = "", link = "", Fulltype = "";
                if (escrowdata != null)
                {
                    esmail = escrowdata.EOEmail;
                    esaddress = escrowdata.PropertyAddress;
                    escellphone = escrowdata.EoPhoneCell;
                    esphone = escrowdata.EOPhone;
                    esname = escrowdata.EscrowOfficerName;
                    ext = escrowdata.EoPhoneExt;
                    companyh = userData.company;
                    if (escrowdata.Logo != "")
                    {
                        link = escrowdata.Logo;
                    }
                }
                MatchCollection matchess = regexx.Matches('{' + userData.type + '}');
                for (int i = 0; i < matchess.Count; i++)
                {
                    string types = matchess[i].Value.Replace("{", "").Replace("}", "");
                    if (types.Trim() == /*"LRA"*/"RAL")
                    {
                        if (Fulltype != "")
                        {
                            Fulltype += ", Listing Real Estate Agent";
                        }
                        else
                        {
                            Fulltype = "Listing Real Estate Agent";
                        }
                    }
                    if (types.Trim() == /*"LRB"*/"RBL")
                    {
                        if (Fulltype != "")
                        {
                            Fulltype += ", Listing Real Estate Broker";
                        }
                        else
                        {
                            Fulltype = "Listing Real Estate Broker";
                        }
                    }
                    if (types.Trim() == /*"SRA"*/"RAS")
                    {
                        if (Fulltype != "")
                        {
                            Fulltype += ", Selling Real Estate Agent";
                        }
                        else
                        {
                            Fulltype = "Selling Real Estate Agent";
                        }
                    }
                    if (types.Trim() == /*"SRB"*/"RBS")
                    {
                        if (Fulltype != "")
                        {
                            Fulltype += ", Selling Real Estate Broker";
                        }
                        else
                        {
                            Fulltype = "Selling Real Estate Broker";
                        }
                    }
                    if (types.Trim() == "RAO")
                    {
                        if (Fulltype != "")
                        {
                            Fulltype += ", Other Real Estate Agent";
                        }
                        else
                        {
                            Fulltype = "Other Real Estate Agent";
                        }
                    }
                    if (types.Trim() == "RBO")
                    {
                        if (Fulltype != "")
                        {
                            Fulltype += ", Other Real Estate Broker";
                        }
                        else
                        {
                            Fulltype = "Other Real Estate Broker";
                        }
                    }
                    if (types.Trim() == "BR1" || types.Trim() == "BR2" || types.Trim() == "BR3" || types.Trim() == "BR4" || types.Trim() == "BR5" || types.Trim() == "BR6" || types.Trim() == "BR7" || types.Trim() == "BR8" || types.Trim() == "BR9" || types.Trim() == "BR10" || types.Trim() == "BRX")
                    {
                        if (Fulltype != "")
                        {
                            Fulltype += ", Buyer";
                        }
                        else
                        {
                            Fulltype = "Buyer";
                        }
                    }
                    if (types.Trim() == "SR1" || types.Trim() == "SR2" || types.Trim() == "SR3" || types.Trim() == "SR4" || types.Trim() == "SR5" || types.Trim() == "SR6" || types.Trim() == "SR7" || types.Trim() == "SR8" || types.Trim() == "SR9" || types.Trim() == "SR10" || types.Trim() == "SRX")
                    {
                        if (Fulltype != "")
                        {
                            Fulltype += ", Seller";
                        }
                        else
                        {
                            Fulltype = "Seller";
                        }
                    }
                    if (types.Trim() == "LBX")
                    {
                        if (Fulltype != "")
                        {
                            Fulltype += ", Mortgage/Loan Broker";
                        }
                        else
                        {
                            Fulltype = "Mortgage/Loan Broker";
                        }
                    }
                    if (types.Trim() == "LBP")
                    {
                        if (Fulltype != "")
                        {
                            Fulltype += ", Loan Broker Processor";
                        }
                        else
                        {
                            Fulltype = "Loan Broker Processor";
                        }
                    }
                    if (types.Trim() == "TC1" || types.Trim() == "TC2" || types.Trim() == "TC3" || types.Trim() == "TC4" || types.Trim() == "TC5" || types.Trim() == "TC6" || types.Trim() == "TC7" || types.Trim() == "TC8" || types.Trim() == "TC9" || types.Trim() == "TC10" || types.Trim() == "TCX" || types.Trim() == "TCL" || types.Trim() == "TCO" || types.Trim() == "TCS") ;

                    {
                        if (Fulltype != "")
                        {
                            Fulltype += ", Transaction Coordinator";
                        }
                        else
                        {
                            Fulltype = "Transaction Coordinator";
                        }
                    }
                    if (types.Trim() == "LR1" || types.Trim() == "LR2" || types.Trim() == "LR3")
                    {
                        if (Fulltype != "")
                        {
                            Fulltype += ", Lender";
                        }
                        else
                        {
                            Fulltype = "Lender";
                        }
                    }
                    if (types.Trim() == "LP1" || types.Trim() == "LP2" || types.Trim() == "LP3")
                    {
                        if (Fulltype != "")
                        {
                            Fulltype += ", Loan Processor";
                        }
                        else
                        {
                            Fulltype = "Loan Processor";
                        }
                    }

                    //if (types.Trim() == "TCX")
                    //{
                    //    if (Fulltype != "")
                    //    {
                    //        Fulltype += ", Title Company";
                    //    }
                    //    else
                    //    {
                    //        Fulltype = "Title Company";
                    //    }
                    //}
                    if (types.Trim() == "TCA")
                    {
                        if (Fulltype != "")
                        {
                            Fulltype += ", Trans. Crdntr. Assistant";
                        }
                        else
                        {
                            Fulltype = "Trans. Crdntr. Assistant";
                        }
                    }
                    if (types.Trim() == "EO1")
                    {
                        if (Fulltype != "")
                        {
                            Fulltype += ", The Escrow Officer";
                        }
                        else
                        {
                            Fulltype = "The Escrow Officer";
                        }
                    }
                    if (types.Trim() == "EA1")
                    {
                        if (Fulltype != "")
                        {
                            Fulltype += ", The Escrow Assistant";
                        }
                        else
                        {
                            Fulltype = "The Escrow Assistant";
                        }
                    }
                    if (types.Trim() == "EOX")
                    {
                        if (Fulltype != "")
                        {
                            Fulltype += ", an Escrow Officer";
                        }
                        else
                        {
                            Fulltype = "an Escrow Officer";
                        }
                    }
                    if (types.Trim() == "EAX")
                    {
                        if (Fulltype != "")
                        {
                            Fulltype += ", an Escrow Assistant";
                        }
                        else
                        {
                            Fulltype = "an Escrow Assistant";
                        }
                    }
                }
                string texts = "";
                using (StreamReader reader = System.IO.File.OpenText("wwwroot\\EmailUpdate.html")) // Path to your Email format
                {
                    texts = reader.ReadToEnd();
                    texts = texts.Replace("$$NameofUser$$", userData.name).Replace("$$CompanyH$$", companyh).Replace("$$Link$$", link).Replace("$$UserType$$", Fulltype).Replace("$$Ext$$", ext).Replace("$$EoPhone$$", esphone).Replace("$$EscrowNo$$", userData.escro).Replace("$$NameofEscrowOfficer$$", esname).Replace("$$EoMail$$", esmail).Replace("$$EoPhoneCell$$", escellphone).Replace("$$Company$$", userData.company).Replace("$$EoAddress$$", esaddress).Replace("&&referer$$", referer).Replace("$$UserName$$", UserName);
                }
                mail.IsBodyHtml = true;
                mail.Body = texts;
                SmtpClient SmtpServer = new SmtpClient();
                SmtpServer.Port = MailPort;
                SmtpServer.Credentials = new System.Net.NetworkCredential(Mailid, MailPwd);
                SmtpServer.Host = MailHost;
                SmtpServer.EnableSsl = MailSSL;
                SmtpServer.Send(mail);
                string logs = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");
                if (!System.IO.File.Exists(logs))
                {
                    FileStream fs1 = new FileStream(logs, FileMode.OpenOrCreate, FileAccess.Write);
                }
                StreamWriter writer = new StreamWriter(logs, true);
                writer.WriteLine("Updation mail sent for -: " + userData.fromEmail + DateTime.Now.ToString());
                writer.Close();
                res.message = "Update Mail Sent";
            }
            catch (Exception ex)
            {
                string logs = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");
                if (!System.IO.File.Exists(logs))
                {
                    FileStream fs1 = new FileStream(logs, FileMode.OpenOrCreate, FileAccess.Write);
                }
                StreamWriter writer = new StreamWriter(logs, true);
                writer.WriteLine("Error in SendMailUpdate method for -: " + userData.fromEmail + " error=" + ex.ToString() + DateTime.Now.ToString());
                writer.Close();
                res.message = "Update Mail Not Sent";
            }
            return res;
        }

        [HttpPost]
        public IActionResult usersmsprefrence([FromBody] List<smsSettingModel> Input)
        {
            try
            {
                string cs = conf["ConnectionStrings:Default"].ToString();
                using var con = new MySqlConnection(cs);
                con.Open();
                var sql1 = $"DELETE FROM usersmsprefrence WHERE UserId=@UserId";
                using (var cmd1 = new MySqlCommand(sql1, con))
                {
                    cmd1.Parameters.AddWithValue("@UserId", Input.FirstOrDefault().userid);

                    cmd1.ExecuteNonQuery();
                }
                foreach (var data in Input)
                {

                    var sql = $"INSERT INTO usersmsprefrence(SMSprefrence,UserId,functionName)" +
                                "VALUES (@SMSprefrence, @UserId, @functionName)";
                    using (var cmd = new MySqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@SMSprefrence", data.Ichecked);
                        cmd.Parameters.AddWithValue("@UserId", data.userid);
                        cmd.Parameters.AddWithValue("@functionName", data.function);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }
        #endregion

        public IActionResult getusersmsprefrence()
        {

            try
            {
                var userid = int.Parse(Request.Headers["userid"]);
                string cs = conf["ConnectionStrings:Default"].ToString();
                {
                    using var con = new MySqlConnection(cs);
                    con.Open();
                    var sql = $"Select * from usersmsprefrence where UserId=@UserId";
                    using (var cmd = new MySqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userid);

                        var dr = cmd.ExecuteReader();
                        var dataTable = new DataTable();
                        dataTable.Load(dr);
                        string JSONString = string.Empty;
                        JSONString = JsonConvert.SerializeObject(dataTable);
                        var result = JsonConvert.DeserializeObject<List<ExpandoObject>>(JSONString);
                        if (result != null)
                        {
                            return new JsonResult(result);
                        }
                        return NoContent();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return NoContent();
        }



        #region zoho EsignApi 
        public responseBack ZohoESignAppLogin(string state)
        {
            string logs = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");
            if (!System.IO.File.Exists(logs))
            {

                FileStream fs1 = new FileStream(logs, FileMode.OpenOrCreate, FileAccess.Write);
            }

            StreamWriter writer = new StreamWriter(logs, true);
            writer.WriteLine("opening  ZohoESignAppLogin" + state);
            try
            {
                string clientBaseAddress = conf["zoho:ClientBaseAddress"].ToString();
                string clientAppTokenUrl = conf["zoho:ClientAppTokenUrl"].ToString();
                string authorizationEndpoint = clientBaseAddress + clientAppTokenUrl;
                // Define your application's client ID and redirect URI.
                string clientId = conf["zoho:ClientId"].ToString();
                string redirectUri = conf["zoho:RedirectUri"].ToString();
                // Define the scope(s) you want to request (e.g., "ZohoCRM.modules.all,ZohoCRM.settings.all").
                string scope = "ZohoSign.documents.all";
                // Generate a random state value for CSRF protection (optional).            
                //string state = Guid.NewGuid().ToString();
                // Construct the authorization URL with parameters.
                string authorizationUrl = $"{authorizationEndpoint}?client_id={clientId}&redirect_uri={redirectUri}&scope={scope}&response_type=code&state={state}&access_type=offline&prompt=consent ";
                writer.WriteLine("authorizationUrl" + authorizationUrl);
                // Open the authorization URL in the default web browser.
                string browserPath = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe"; // path for develpoment server
                                                                                                     //  string browserPath = @"C:\Program Files\Google\Chrome\Application\chrome.exe"; // path for production server 
                                                                                                     // Open the authorization URL in the specified web browser.
                Process.Start(new ProcessStartInfo(browserPath, authorizationUrl));
                writer.WriteLine("process Start" + browserPath);
                writer.WriteLine("process Start  authorizationUrl" + authorizationUrl);
                writer.Close();
            }
            catch (Exception ex)
            {
                writer.Close();
                string logs1 = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");
                if (!System.IO.File.Exists(logs1))
                {
                    FileStream fs1 = new FileStream(logs1, FileMode.OpenOrCreate, FileAccess.Write);
                }
                StreamWriter writer1 = new StreamWriter(logs1, true);
                writer1.WriteLine("Error in oppening zoho " + ex.Message);
                writer1.Close();
                return null;
            }
            return null;
        }

        public async Task<responseBack> ZohoESignGetRefreshToken(string state, string code, string location)
        {
            responseBack res = new responseBack();
            try
            {
                // Define the authorization code, client ID, client secret, redirect URI, and grant type.
                string authorizationCode = code;
                string clientId = conf["zoho:ClientId"].ToString();
                string clientSecret = conf["zoho:ClientSecret"].ToString();
                string redirectUri = conf["zoho:RedirectUri"].ToString();
                string grantType = "authorization_code";


                // Define the token endpoint URL.                                    
                string tokenUrl = "https://accounts.zoho.in/oauth/v2/token";

                // Create an HttpClient instance.
                using (HttpClient httpClient = new HttpClient())
                {
                    // Prepare the token request parameters.
                    var tokenRequestParameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("code", authorizationCode),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("redirect_uri", redirectUri),
                new KeyValuePair<string, string>("grant_type", grantType),
                new KeyValuePair<string, string>("state", state)
            };
                    // Create the token request content.
                    var tokenRequestContent = new FormUrlEncodedContent(tokenRequestParameters);
                    // Send the POST request to the token endpoint.
                    HttpResponseMessage response1 = await httpClient.PostAsync(tokenUrl, tokenRequestContent);

                    // Check if the request was successful.
                    if (response1.IsSuccessStatusCode)
                    {
                        // Parse and display the response JSON.
                        string responseBody = await response1.Content.ReadAsStringAsync();
                        if (!responseBody.Contains("invalid_code"))
                        {
                            ZohotokenApiResponse zohotokenApiResponse = JsonConvert.DeserializeObject<ZohotokenApiResponse>(responseBody);

                            string cs = conf["ConnectionStrings:Default"].ToString();
                            {
                                using var con = new MySqlConnection(cs);
                                con.Open();
                                var sql = $"Update  srescrowdev2.e_signcompany set Refreshtoken =@RefreshToken  where SystemCode=@SystemCode ";
                                using (var cmd = new MySqlCommand(sql, con))
                                {
                                    cmd.Parameters.AddWithValue("@RefreshToken", zohotokenApiResponse.refresh_token);
                                    cmd.Parameters.AddWithValue("@SystemCode", 2001);
                                    var dr = cmd.ExecuteNonQueryAsync();
                                    res.message = $"Refresh token Saved Sucessfully in Database + {responseBody}";
                                }
                            }

                        }
                    }
                    else
                    {
                        res.message = await response1.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                res.message = ex.Message;

            }
            return res;
        }

        public async Task<string> ZohoESignGetAccessToken()
        {
            responseBack res = new responseBack();
            string refreshToken = string.Empty;
            string logs = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\FoxitPdf.txt");
            if (!System.IO.File.Exists(logs))
            {
                FileStream fs1 = new FileStream(logs, FileMode.OpenOrCreate, FileAccess.Write);
            }


            string cs = conf["ConnectionStrings:Default"].ToString();
            {
                using var con = new MySqlConnection(cs);
                con.Open();
                var sql = $"Select RefreshToken,AccessToken,AccessTokenTime from   srescrowdev2.e_signcompany  where SystemCode=@SystemCode ";
                using (var cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@SystemCode", 2001);
                    var dr = cmd.ExecuteReader();
                    var dataTable = new DataTable();
                    dataTable.Load(dr);
                    string JSONString = string.Empty;
                    JSONString = JsonConvert.SerializeObject(dataTable);
                    List<RefreshTokenResponse> _refreshToken = JsonConvert.DeserializeObject<List<RefreshTokenResponse>>(JSONString);
                    var dbEsignCompany = _refreshToken.FirstOrDefault();
                    if (dbEsignCompany != null)
                    {
                        refreshToken = dbEsignCompany.RefreshToken;
                        if (dbEsignCompany.AccessTokenTime.HasValue && DateTime.Now.Subtract(dbEsignCompany.AccessTokenTime.Value).TotalMinutes < 55)
                        {
                            return dbEsignCompany.AccessToken;
                        }

                    }

                    StreamWriter writer101 = new StreamWriter(logs, true);
                    writer101.WriteLine("Getting Refresh Token From Db " + refreshToken + " " + DateTime.Now.ToString());
                    writer101.Close();
                }
            }

            try
            {
                #region
                string clientId = conf["zoho:ClientId"].ToString();
                string clientSecret = conf["zoho:ClientSecret"].ToString();
                string redirectUri = conf["zoho:RedirectUri"].ToString();
                string grant_type = "refresh_token";
                // Define the token endpoint URL.
                string tokenUrl = "https://accounts.zoho.in/oauth/v2/token";

                // Create an HttpClient instance.
                using (HttpClient httpClient = new HttpClient())
                {
                    // Prepare the token request parameters.
                    var tokenRequestParameters = new List<KeyValuePair<string, string>>
               {
               new KeyValuePair<string, string>("refresh_token", refreshToken),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                 new KeyValuePair<string, string>("redirect_uri", redirectUri),
                new KeyValuePair<string, string>("grant_type", "refresh_token"),

            };

                    // Create the token request content.
                    var tokenRequestContent = new FormUrlEncodedContent(tokenRequestParameters);
                    // Send the POST request to the token endpoint.
                    HttpResponseMessage response1 = await httpClient.PostAsync(tokenUrl, tokenRequestContent);

                    // Check if the request was successful.
                    if (response1.IsSuccessStatusCode)
                    {
                        // Parse and display the response JSON.
                        string responseBody = await response1.Content.ReadAsStringAsync();
                        if (!responseBody.Contains("invalid_code"))
                        {
                            StreamWriter writer2 = new StreamWriter(logs, true);
                            writer2.WriteLine("Getting Access Token From Zoho response" + responseBody + " " + DateTime.Now.ToString());
                            writer2.Close();
                            ZohotokenApiResponse zohotokenApiResponse = JsonConvert.DeserializeObject<ZohotokenApiResponse>(responseBody);

                            using var connection = new MySqlConnection(cs);
                            connection.Open();
                            var sqlQuery = "UPDATE srescrowdev2.e_signcompany SET AccessToken = @AccessToken, AccessTokenTime = @AccessTokenTime WHERE SystemCode = @SystemCode";
                            using (var cmd = new MySqlCommand(sqlQuery, connection))
                            {
                                cmd.Parameters.AddWithValue("@AccessToken", zohotokenApiResponse.access_token);
                                cmd.Parameters.AddWithValue("@AccessTokenTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")); // Proper DateTime format for MySQL
                                cmd.Parameters.AddWithValue("@SystemCode", 2001);
                                var dr = cmd.ExecuteNonQuery();
                            }
                            return zohotokenApiResponse.access_token;
                        }
                    }
                    else
                    {
                        string responseBody = await response1.Content.ReadAsStringAsync();
                        StreamWriter writer2 = new StreamWriter(logs, true);
                        writer2.WriteLine("Getting Access Token From Zoho response" + responseBody + " " + DateTime.Now.ToString());
                        writer2.Close();
                    }
                }

                #endregion

            }
            catch (Exception ex)
            {
                StreamWriter writer2 = new StreamWriter(logs, true);
                writer2.WriteLine("Getting Exception Access Token From Zoho response" + ex.InnerException + " " + DateTime.Now.ToString());
                writer2.WriteLine("Getting Exception Access Token From Zoho response" + ex.Message + " " + DateTime.Now.ToString());
                writer2.Close();

            }

            return "";

        }

        public responseBack ZohoESignRevokeRefreshToken(string refreshToken)
        {
            responseBack res = new responseBack();
            try
            {
                #region

                var request = (HttpWebRequest)WebRequest.Create($"https://accounts.zoho.in/oauth/v2/token/revoke?token={refreshToken}");

                request.Method = "Get";
                var response = (HttpWebResponse)request.GetResponse();
                string accessToken = "";
                string mails = "";
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var responseString = streamReader.ReadToEnd();
                    //AccessToken myDeserializedClass = JsonConvert.DeserializeObject<AccessToken>(responseString);
                    //accessToken = myDeserializedClass.access_token;
                }


                #endregion
            }
            catch (Exception ex)
            {

            }
            return res;
        }
        #endregion

        #region 
        // Upload folder to the zohowebsite 

        public async Task<responseBack> ZohoESignCreateDocument(string esignPath, string esignKey, string EscrowId)
        {
            responseBack res = new responseBack();

            List<EscrowDetail> EscrowUserList = new List<EscrowDetail>();
            try
            {
                var accessToken = await ZohoESignGetAccessToken();

                List<ZohoSigninUserMapping> zohoSigninUserMapping = new List<ZohoSigninUserMapping>();
                List<UserTypeList> userTypeList = new List<UserTypeList>();
                List<UserTypeList> userTypeListInitials = new List<UserTypeList>();
                string fileTokenList = string.Empty;
                List<string> extractedCodes = new List<string>();
                using (var unit = _unitOfWorkManager.Begin())
                {
                    EscrowUserList = _escrowDetailRepository.GetAll().Where(x => x.EscrowId == EscrowId).ToList();
                    unit.Complete();
                }
                using (HttpClient httpClient = new HttpClient())
                {
                    string pdfFilePath = "wwwroot\\Common\\Paperless\\" + esignPath;
                    string pdfFilePathAfterConvert = "wwwroot\\Common\\replacedSign.pdf";
                    string dateTimeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    string createFileForZoho = $"wwwroot\\Common\\convertToSign_{dateTimeStamp}.pdf";
                    pdfFilePath = pdfFilePath + esignKey;
                    pdfFilePath = pdfFilePath.Replace("/", "\\");
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    FileInfo file = new FileInfo(pdfFilePath);
                    if (file.Exists)
                    {
                        FileInfo fileZoho = new FileInfo(createFileForZoho);
                        if (fileZoho.Exists)
                        {
                            FileInfo fileConverted = new FileInfo(createFileForZoho);
                            fileConverted.Delete();
                        }
                        //createFileForZoho
                        System.IO.File.Copy(pdfFilePath, createFileForZoho);

                    }
                    if (pdfFilePath.Contains("~") || pdfFilePath.Contains("-'-") || pdfFilePath.Contains("_'_"))
                    {
                        if (pdfFilePath.Contains("~"))
                            fileTokenList = pdfFilePath.Substring(pdfFilePath.LastIndexOf("~") + 1);
                        if (pdfFilePath.Contains("-'-"))
                            fileTokenList = pdfFilePath.Substring(pdfFilePath.LastIndexOf("-'-") + 3);
                        if (pdfFilePath.Contains("_'_"))
                            fileTokenList = pdfFilePath.Substring(pdfFilePath.LastIndexOf("_'_") + 3);

                        string match = String.Empty;

                        MatchCollection matches = regexx.Matches(fileTokenList);
                        for (int i = 0; i < matches.Count; i++)
                        {
                            string rep = matches[i].Value.Replace("{", "").Replace("}", "");
                            int index = rep.IndexOf('-');
                            match = rep.Substring(index + 1);
                            if (match.Contains("S"))
                            {
                                extractedCodes.Add(rep.Substring(0, index));
                            }
                        }

                        List<EscrowDetail> filteredEscrowUserList = new List<EscrowDetail>();

                        // Filter using foreach loop with specific checks
                        foreach (var user in EscrowUserList)
                        {
                            // Check if extractedCodes contains SRX or BRX
                            if (extractedCodes.Contains("SRX") && user.Usertype.StartsWith("SR") ||
                                extractedCodes.Contains("BRX") && user.Usertype.StartsWith("BR"))
                            {
                                filteredEscrowUserList.Add(user);
                            }
                            // Check for exact match
                            else if (extractedCodes.Contains(user.Usertype))
                            {
                                filteredEscrowUserList.Add(user);
                            }
                        }
                        EscrowUserList = filteredEscrowUserList.Where(x => x.Usertype != "EOX").ToList();
                    }

                    var findWord = new List<string>();
                    var replaceWord = new List<string>();
                    var idx = 1;
                    string updatedUrl = "";
                    var fileData = await uploadFileToTextReplace(createFileForZoho);
                    List<AddTextToReplaceDto> addTextToReplaceList = new List<AddTextToReplaceDto>();
                    foreach (var escrowUser in EscrowUserList)
                    {

                        AddTextToReplaceDto addTextResponse = await findPdfText($"@{{{escrowUser.Usertype}:S:1}}", $"{{{{S:R{idx}*}}}}", fileData.Url, Path.GetFileName(createFileForZoho));
                        if (string.IsNullOrWhiteSpace(addTextResponse.Url))
                        {

                            findWord.Add($"@{{{escrowUser.Usertype}:S:1}}");
                            replaceWord.Add($"{{{{S:R{idx}*}}}}"); 

                            var escrowDetails = EscrowUserList.Where(x => x.Usertype == escrowUser.Usertype).FirstOrDefault();
                            if (escrowDetails != null)
                            {
                                var check = zohoSigninUserMapping.Where(x => x.recipientEmail == escrowDetails.Email).FirstOrDefault();
                                if (check == null)
                                {
                                    ZohoSigninUserMapping zohoSignin = new ZohoSigninUserMapping();
                                    zohoSignin.recipientName = escrowDetails.Name;
                                    zohoSignin.recipientEmail = escrowDetails.Email;
                                    zohoSignin.signingOrder = idx;
                                    zohoSigninUserMapping.Add(zohoSignin);
                                }
                            }
                        }
                        else
                        {
                            addTextToReplaceList.Add(addTextResponse);
                            fileData.Url = addTextResponse.Url;

                            var escrowDetails = EscrowUserList.Where(x => x.Usertype == escrowUser.Usertype).FirstOrDefault();
                            if (escrowDetails != null)
                            {
                                var check = zohoSigninUserMapping.Where(x => x.recipientEmail == escrowDetails.Email).FirstOrDefault();
                                if (check == null)
                                {
                                    ZohoSigninUserMapping zohoSignin = new ZohoSigninUserMapping();
                                    zohoSignin.recipientName = escrowDetails.Name;
                                    zohoSignin.recipientEmail = escrowDetails.Email;
                                    zohoSignin.signingOrder = idx;
                                    zohoSigninUserMapping.Add(zohoSignin);
                                }
                            }
                        }
                        idx++;
                    }

                    // If you need to convert them to arrays afterward:
                    string[] findWordArray = findWord.ToArray();
                    string[] replaceWordArray = replaceWord.ToArray();
                    FileUploadResponse fileUploadResponse = new FileUploadResponse();
                    if (findWord.Count == 0 && addTextToReplaceList.Count > 0)
                    {
                        var lastUrl = addTextToReplaceList.LastOrDefault().Url;
                        var index = 1;
                        bool isAsync = false;
                        foreach (var item in addTextToReplaceList)
                        {
                            if (index == addTextToReplaceList.Count)
                            {
                                isAsync = true;
                            }
                            fileUploadResponse = await AddPdfText(item.replaceWord, lastUrl, item.left, item.top, Path.GetFileName(createFileForZoho), isAsync);
                            if (!string.IsNullOrWhiteSpace(fileUploadResponse.Url))
                            {
                                lastUrl = fileUploadResponse.Url;
                            }
                            if (index == addTextToReplaceList.Count)
                            {
                                fileData.Url = fileUploadResponse.Url;
                                fileData.jobId = fileUploadResponse.jobId;
                            }
                            index++;
                        }

                    }

                    if (findWord.Count > 0)
                    {
                        var lastUrl = addTextToReplaceList.LastOrDefault().Url;
                        var index = 1;
                        bool isAsync = false;
                        foreach (var item in addTextToReplaceList)
                        {

                            fileUploadResponse = await AddPdfText(item.replaceWord, lastUrl, item.left, item.top, Path.GetFileName(createFileForZoho), isAsync);
                            if (!string.IsNullOrWhiteSpace(fileUploadResponse.Url))
                            {
                                lastUrl = fileUploadResponse.Url;
                            }
                            if (index == addTextToReplaceList.Count)
                            {
                                fileData.Url = fileUploadResponse.Url;
                            }
                            index++;
                        }
                        var textFindAndReplaceResponse = await TextFindAndReplace(fileData.Url, findWordArray, replaceWordArray, Path.GetFileName(createFileForZoho));
                        if (!string.IsNullOrWhiteSpace(textFindAndReplaceResponse.jobId))
                        {
                            fileData.jobId = textFindAndReplaceResponse.jobId;
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(fileData.Url))
                    {
                        var jobStatus = await TextFindAndReplaceFileJobStatus(fileData.jobId);
                        await DownloadFileAsync(jobStatus.url, pdfFilePath);
                    }
                    // Create a multipart form data content
                    var multipartContent = new MultipartFormDataContent();

                    // Read the PDF file and add it to the content
                    byte[] pdfBytes = System.IO.File.ReadAllBytes(pdfFilePath);
                    ByteArrayContent fileContent = new ByteArrayContent(pdfBytes);
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                    string fileName = "esignFile.pdf";
                    multipartContent.Add(fileContent, "file", fileName); // "file" is the form field name
                    string logs1 = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\FoxitPdf.txt");
                    if (!System.IO.File.Exists(logs1))
                    {
                        FileStream fs1 = new FileStream(logs1, FileMode.OpenOrCreate, FileAccess.Write);
                    }
                    StreamWriter writer1 = new StreamWriter(logs1, true);
                    writer1.WriteLine($" createing zoho doc " + DateTime.Now.ToString());
                    writer1.Close();

                    CreateDocumentRequest createDocumentRequest = new CreateDocumentRequest();
                    createDocumentRequest.requests = new Requests();
                    createDocumentRequest.requests.request_name = "EsignRequst";
                    createDocumentRequest.requests.expiration_days = "30";
                    createDocumentRequest.requests.is_sequential = false;
                    createDocumentRequest.requests.email_reminders = true;
                    createDocumentRequest.requests.reminder_period = 10;
                    // createDocumentRequest.requests.folder_id = "78657000000035001";
                    createDocumentRequest.requests.folder_id = conf["zoho:FolderId"].ToString();


                    createDocumentRequest.requests.actions = new List<Models.ZohoESign.Action>();

                    foreach (var item in zohoSigninUserMapping)
                    {
                        createDocumentRequest.requests.actions.Add(new Models.ZohoESign.Action
                        {
                            action_type = "SIGN",
                            recipient_email = item.recipientEmail,
                            recipient_name = item.recipientName,
                            signing_order = item.signingOrder,
                            verify_recipient = false,
                            verification_type = "EMAIL",
                            verification_code = "",
                            private_notes = "Please get back to us for further queries",
                            is_embedded = true,
                            is_bulk = true,

                        });
                    }



                    var json = JsonConvert.SerializeObject(createDocumentRequest);
                    multipartContent.Add(new StringContent(json), "data");
                    // Send the POST request with the multipart content
                    HttpResponseMessage response = await httpClient.PostAsync("https://sign.zoho.in/api/v1/requests?testing=true", multipartContent);


                    StreamWriter writer88 = new StreamWriter(logs1, true);
                    writer88.WriteLine($" Request Payload " + DateTime.Now.ToString() + "  " + json);
                    writer88.Close();
                    StreamWriter writer89 = new StreamWriter(logs1, true);
                    writer89.WriteLine($" Access Toke  " + DateTime.Now.ToString() + "  " + accessToken);
                    writer89.Close();


                    //HttpRequestMessage requestItem = new HttpRequestMessage(HttpMethod.Post, "")

                    //{
                    //    Content = new StringContent(json, Encoding.UTF8, "application/json")
                    //};

                    //// Send the POST request
                    //HttpResponseMessage response = await httpClient.SendAsync(requestItem);

                    // Check if the request was successful (status code 2xx)
                    if (response.IsSuccessStatusCode)
                    {
                        // Read and process the response content
                        string responseBody = await response.Content.ReadAsStringAsync();
                        CreateDocumentResponse CreateDocumentResponse = JsonConvert.DeserializeObject<CreateDocumentResponse>(responseBody);
                        // DocumentSignature(pdfFilePath, accessToken, CreateDocumentResponse);


                        string logs2 = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\FoxitPdf.txt");
                        if (!System.IO.File.Exists(logs2))
                        {

                            FileStream fs1 = new FileStream(logs2, FileMode.OpenOrCreate, FileAccess.Write);
                        }
                        StreamWriter writer2 = new StreamWriter(logs1, true);
                        writer2.WriteLine($" response success " + DateTime.Now.ToString());
                        writer2.Close();



                        CreateOrEditE_SignRecordDto std = new CreateOrEditE_SignRecordDto();
                        var requests = CreateDocumentResponse.requests;
                        std.FolderId = long.Parse(requests.folder_id);
                        std.FolderName = requests.folder_name;
                        std.FileName = esignKey;
                        std.Status = "Unsigned";
                        std.RequestId = requests.request_id;
                        if (!string.IsNullOrWhiteSpace(requests.document_ids.FirstOrDefault().document_id))
                        {
                            std.DocumentId = long.Parse(requests.document_ids.FirstOrDefault().document_id);
                        }
                        std.FullFilePath = pdfFilePath;
                        if (requests.actions.Count > 0)
                        {
                            std.ZohoAction = JsonConvert.SerializeObject(requests.actions);
                        }
                        else
                        {
                            std.ZohoAction = "";
                        }


                        std.EsignCompanyCode = 2001;
                        // std.CompanyId = requests.owner_id;
                        std.EmailId = requests.owner_email;
                        //  std.EmbeddedToken = sign.embeddedToken;
                        //std.EmbeddedURL = sign.embeddedSessionURL;
                        var i = await _e_SignRecordsAppService.CreateOrEdit(std);
                        var esignData = JsonConvert.SerializeObject(std);

                        string logs5 = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\FoxitPdf.txt");
                        if (!System.IO.File.Exists(logs2))
                        {

                            FileStream fs1 = new FileStream(logs5, FileMode.OpenOrCreate, FileAccess.Write);
                        }
                        StreamWriter writer5 = new StreamWriter(logs5, true);
                        writer5.WriteLine($" Created zoho file in db success " + i + DateTime.Now.ToString());
                        writer5.WriteLine($" EsignRecord Json " + esignData);
                        writer5.Close();
                        //  DocumentSignature(std.DocumentId.ToString(), accessToken);
                    }
                    else
                    {
                        string logs8 = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\FoxitPdf.txt");
                        if (!System.IO.File.Exists(logs8))
                        {
                            FileStream fs1 = new FileStream(logs8, FileMode.OpenOrCreate, FileAccess.Write);
                        }

                        string responseBody = await response.Content.ReadAsStringAsync();
                        StreamWriter writer2 = new StreamWriter(logs8, true);
                        writer2.WriteLine($" response unsuccess " + responseBody);

                        writer2.WriteLine("from dto" + json);
                        writer2.Close();
                        CreateDocumentResponse responseUpload = JsonConvert.DeserializeObject<CreateDocumentResponse>(responseBody);


                        CreateOrEditE_SignRecordDto std = new CreateOrEditE_SignRecordDto();

                        std.FolderId = 0;
                        std.FolderName = "Not valid file";
                        std.FileName = esignKey;
                        std.Status = "Unsigned";
                        std.RequestId = "0";
                        std.DocumentId = 0;
                        std.FullFilePath = pdfFilePath;
                        std.ZohoAction = "";



                        std.EsignCompanyCode = 2001;
                        // std.CompanyId = requests.owner_id;
                        std.EmailId = "";
                        //  std.EmbeddedToken = sign.embeddedToken;
                        //std.EmbeddedURL = sign.embeddedSessionURL;
                        var i = await _e_SignRecordsAppService.CreateOrEdit(std);



                    }

                }
            }
            catch (Exception ex)
            {

            }
            return res;
        }

        //public async Task<string> GetEmbeddedLink(string filePath, string escrow, string userType, long srAssignedFileId)
        //{
        //    try
        //    {
        //        long userId = long.Parse(AbpSession.UserId.ToString());
        //        var dbEsignRecord = _e_SignRecordsAppService.GetAllE_Sign(2001);
        //        var find = dbEsignRecord.FirstOrDefault(x =>
        //            string.Equals(x.FileName?.Trim(), filePath?.Trim(), StringComparison.OrdinalIgnoreCase));

        //        if (find == null)
        //        {
        //            Logger.Warn($"No e-sign record found for file: {filePath}");
        //            return "";
        //        }

        //        var dbEscrowDetails = await _IescrowDetailsAppService.GetEscrowDetailForByUserId(userId, escrow, userType);
        //        if (dbEscrowDetails?.EscrowDetail?.Email == null)
        //        {
        //            Logger.Error($"Missing escrow email for userId: {userId}");
        //            return "Error: Escrow email missing.";
        //        }

        //        var accessToken = await ZohoESignGetAccessToken();

        //        Logger.Info($"Calling DocumentSignature for DocumentId={find.DocumentId}, Email={dbEscrowDetails.EscrowDetail.Email}");
        //        var fileUrl = await DocumentSignature(
        //            find.DocumentId.ToString(),
        //            accessToken,
        //            dbEscrowDetails.EscrowDetail.Email,
        //            userType,
        //            srAssignedFileId,
        //            filePath
        //        );

        //        Logger.Info($"DocumentSignature returned: {fileUrl}");

        //        if (string.IsNullOrEmpty(fileUrl))
        //        {
        //            Logger.Warn("DocumentSignature returned null or empty.");
        //            return "Error: No URL returned.";
        //        }

        //        return fileUrl;
        //    }
        //    catch (Exception e)
        //    {
        //        Logger.Error("GetEmbeddedLink failed", e);
        //        return $"Error: {e.Message}";
        //    }
        //}


        [HttpGet]
        public async Task<string> GetEmbeddedLink(string filePath, string escrow, string userType, long srAssignedFileId)
        {
            try
            {
                var state = string.Empty;
                long userId = long.Parse(AbpSession.UserId.ToString());
                var dbEsignRecord = _e_SignRecordsAppService.GetAllE_Sign(2001);
                var find = dbEsignRecord.Where(x => x.FileName == filePath).FirstOrDefault();
                if (find != null)
                {
                    var dbEscrowDetails = await _IescrowDetailsAppService.GetEscrowDetailForByUserId(userId, escrow, userType);
                    var accessToken = await ZohoESignGetAccessToken();

                    var fileUrl = await DocumentSignature(find.DocumentId.ToString(), accessToken, dbEscrowDetails.EscrowDetail.Email, userType, srAssignedFileId, filePath); //await GetEmbeddedURLFromDb(filePath);
                    return fileUrl;

                }
                else
                {
                    return "";
                }
            }
            catch (Exception e)
            {
                Logger.Error("GetEmbeddedLink failed", e);
                return $"Error: {e.Message}";
            }
            return "";

        }

        [HttpGet]
        public async Task<string> GetEmbeddedURLFromDb(string filePath)
        {

            var dbEsignRecord = _e_SignRecordsAppService.GetAllE_Sign(2001).Where(x => x.FileName == filePath).FirstOrDefault();
            if (dbEsignRecord != null)
            {
                return dbEsignRecord.EmbeddedURL;
            }
            else
            {
                return "";
            }

        }

        public async Task<string> DocumentSignature(string documentId, string accessToken, string recipientEmail, string userType, long srAssignedFileId, string filePath)
        {
            try
            {
                var dbEsignRecord = _e_SignRecordsAppService.GetAllE_Sign(2001);

                var dbESignRecords = dbEsignRecord.Where(x => x.DocumentId == long.Parse(documentId)).FirstOrDefault();
                if (dbESignRecords != null)
                {
                    string requestId = dbESignRecords.RequestId;
                    List<ActionData> actionsData = JsonConvert.DeserializeObject<List<ActionData>>(dbESignRecords.ZohoAction);
                    // actionsData = actionsData.Where(x => x.recipient_email == recipientEmail).ToList();

                    foreach (var item in actionsData)
                    {
                        item.recipient_email = item.recipient_email;
                        item.fields = item.fields;
                        //  item.recipient_name = item.recipient_email;
                        //item.recipient_email = "jitender909thakur@gmail.com";
                        item.action_type = "SIGN";
                        item.verify_recipient = true;
                        item.verification_type = "EMAIL";
                    }

                    //actionsData.Add(recipient);
                    var _actionResponse = JsonConvert.SerializeObject(actionsData);
                    //  JArray actions = new JArray(_actionResponse);
                    // Add Fields to Recipient
                    JArray recipientActions = new JArray();
                    var actions = JsonConvert.DeserializeObject<dynamic>(_actionResponse);
                    foreach (var action in actions)
                    {

                        if ((string)action.action_type == "SIGN")
                        {

                            recipientActions.Add(action);
                        }
                    }
                    var requestObj = new JObject();
                    requestObj.Add("actions", recipientActions);

                    JObject dataJson = new JObject();
                    dataJson.Add("requests", requestObj);

                    string dataStr = Newtonsoft.Json.JsonConvert.SerializeObject(dataJson);


                    string ApiUrl = $"https://sign.zoho.in/api/v1/requests/{requestId}/submit?testing=true";
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                        var content = new StringContent(dataStr, Encoding.UTF8, "application/json");

                        HttpResponseMessage response = await client.PostAsync(ApiUrl, content);
                        string responseBody = await response.Content.ReadAsStringAsync();
                        var actionId = string.Empty;

                        List<UserEmail> userEmail = new List<UserEmail>();

                        string cs1 = conf["ConnectionStrings:Default"].ToString();

                        using var con1 = new MySqlConnection(cs1);
                        con1.Open();
                        var sql1 = $"SELECT EmailAddress FROM srescrowdev2.abpusers where Id = @UserId;";
                        using (var cmd = new MySqlCommand(sql1, con1))

                        {
                            cmd.Parameters.AddWithValue("@UserId", AbpSession.UserId);
                            var dr = cmd.ExecuteReader();
                            var dataTable = new DataTable();
                            dataTable.Load(dr);
                            string JSONString = string.Empty;
                            JSONString = JsonConvert.SerializeObject(dataTable);
                            userEmail = JsonConvert.DeserializeObject<List<UserEmail>>(JSONString);
                        }

                        var Email = userEmail.FirstOrDefault().EmailAddress;
                        actionId = actionsData?.FirstOrDefault(x => x.recipient_email == Email)?.action_id;
                        var token = await GetEmbeddedUrl(requestId, actionId, accessToken);
                        Console.WriteLine("Document sent for signature successfully.");
                        Console.WriteLine(responseBody);
                        if (dbESignRecords != null)
                        {
                            if (token != null)
                            {
                                dbESignRecords.EmbeddedURL = token.sign_url;
                                string cs = conf["ConnectionStrings:Default"].ToString();
                                {
                                    using var con = new MySqlConnection(cs);
                                    con.Open();
                                    var sql = $"Update  srescrowdev2.e_signrecords set EmbeddedURL =@EmbeddedURL  where Id=@Id ";
                                    using (var cmd = new MySqlCommand(sql, con))
                                    {
                                        cmd.Parameters.AddWithValue("@EmbeddedURL", dbESignRecords.EmbeddedURL.ToString());
                                        cmd.Parameters.AddWithValue("@Id", dbESignRecords.Id);
                                        var dr = cmd.ExecuteNonQueryAsync();
                                    }
                                }

                                // var srAssignedFilesDetailId =  _srAssignedFilesDetailRepository.InsertOrUpdateAndGetId(srAssignedFilesDetail);                               

                                CreateOrEditEscrowFileHistoryDto escrowFileHistory = new CreateOrEditEscrowFileHistoryDto();
                                escrowFileHistory.SrEscrowFileMasterId = srAssignedFileId;
                                escrowFileHistory.FileFullPath = filePath;
                                escrowFileHistory.UserId = AbpSession.UserId;
                                escrowFileHistory.Message = FileConstant.Sign_File;
                                escrowFileHistory.ActionType = FileConstantAction.Sign_File;
                                await _escrowFileHistoriesAppService.CreateOrEdit(escrowFileHistory);

                            }

                            //_e_SignRecordsAppService.UpdateEsignRecord(dbESignRecords.Id, dbESignRecords.EmbeddedURL);
                        }

                        return dbESignRecords.EmbeddedURL;
                    }

                }
            }
            catch (Exception e)
            {
                return null;
            }
            return null;
        }


        public async Task<EmbeddedZohoResponse> GetEmbeddedUrl(string RequestId, string ActionId, string AccessToken)
        {
            using (HttpClient httpClient = new HttpClient())
            {

                string zohoApiUrl = $"https://sign.zoho.in/api/v1/requests/{RequestId}/actions/{ActionId}/embedtoken?host=https://www.escrowbaseweb.com?testing=true";
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {AccessToken}");

                HttpResponseMessage response = await httpClient.PostAsync(zohoApiUrl, null);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    EmbeddedZohoResponse myDeserializedClass = JsonConvert.DeserializeObject<EmbeddedZohoResponse>(responseBody);

                    Console.WriteLine(responseBody);
                    return myDeserializedClass;
                }
                else
                {
                    Console.WriteLine("Error retrieving document details.");
                }

            }
            return null;
        }


        public async Task<responseBack> GetDocumentLink(string AccessToken, long DocumentId)
        {
            // Replace with your Zoho Sign API access token
            string zohoApiAccessToken = AccessToken;

            // Replace with the document ID or relevant details
            string documentId = DocumentId.ToString();

            // Zoho Sign API endpoint to retrieve document details
            string zohoApiUrl = $"https://sign.zoho.in/api/v1/documents/{documentId}";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {zohoApiAccessToken}");

                    HttpResponseMessage response = await client.GetAsync(zohoApiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        // Parse the API response to extract recipient details and signing links
                        // Typically, the signing link will be part of the recipient information
                        Console.WriteLine(responseBody);
                    }
                    else
                    {
                        Console.WriteLine("Error retrieving document details.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            return null;
        }


        [HttpPost("ReceiveWebhook")]
        public async Task<IActionResult> ReceiveWebhook([FromBody] ZohoSignWebhookPayload payload)
        {
            // Handle the webhook payload, which contains information about the signed document.
            // You can retrieve the signed PDF and perform the download here.
            // Be sure to implement error handling and security measures.
            return Ok();
        }


        public async Task<responseBack> downloadZohoPdf(string filePath, long srAssignedFileId)
        {
            string logs11 = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");
            if (!System.IO.File.Exists(logs11))
            {
                FileStream fs1 = new FileStream(logs11, FileMode.OpenOrCreate, FileAccess.Write);
            }
            responseBack responseBack = new responseBack();
            try
            {
                var dbEsignRecord = _e_SignRecordsAppService.GetAllE_Sign(2001);
                var find = dbEsignRecord.Where(x => x.FileName == filePath).FirstOrDefault();
                var accessToken = await ZohoESignGetAccessToken();
                if (find != null)
                {

                    // Replace with your Zoho Sign API endpoint for downloading a document
                    //string apiUrl = $"https://sign.zoho.in/api/v1/requests/48419000000032037/pdf";
                    string apiUrl = $"https://sign.zoho.in/api/v1/requests/{find.RequestId}/pdf";

                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                        // Make a POST request to download the document
                        HttpResponseMessage response = await client.GetAsync(apiUrl);

                        if (response.IsSuccessStatusCode)
                        {

                            StreamWriter writer1 = new StreamWriter(logs11, true);
                            writer1.WriteLine("Getting the latest pdf path");
                            writer1.Close();

                            // Read the PDF content
                            byte[] pdfBytes = await response.Content.ReadAsByteArrayAsync();

                            // call sgnin status metod here
                            StreamWriter writer3 = new StreamWriter(logs11, true);
                            writer3.WriteLine("siginging status Request");
                            writer3.Close();

                            responseBack = await signStatus(find.FileName);

                            StreamWriter writer2 = new StreamWriter(logs11, true);
                            writer2.WriteLine("siginging status response " + responseBack.signingStatus);
                            writer2.Close();

                            if (responseBack.signingStatus != "Unsigned")
                            {
                                StreamWriter writer9 = new StreamWriter(logs11, true);
                                writer9.WriteLine("_escrowFileHistoriesAppService data updated");
                                writer9.Close();
                                SaveDocumentToDisk(find.FileName, pdfBytes, find.FullFilePath);
                                CreateOrEditEscrowFileHistoryDto escrowFileHistory = new CreateOrEditEscrowFileHistoryDto();
                                escrowFileHistory.SrEscrowFileMasterId = srAssignedFileId;
                                escrowFileHistory.FileFullPath = filePath;
                                escrowFileHistory.UserId = AbpSession.UserId;
                                escrowFileHistory.Message = FileConstant.Sign_File_Download;
                                escrowFileHistory.ActionType = FileConstantAction.Sign_File;
                                await _escrowFileHistoriesAppService.CreateOrEdit(escrowFileHistory);

                                StreamWriter writer4 = new StreamWriter(logs11, true);
                                writer4.WriteLine("_escrowFileHistoriesAppService data updated");
                                writer4.Close();
                            }
                            Console.WriteLine("PDF downloaded successfully.");

                        }
                        else
                        {
                            string responseBody = await response.Content.ReadAsStringAsync();

                            StreamWriter writer6 = new StreamWriter(logs11, true);
                            writer6.WriteLine("error in file download " + responseBody);
                            writer6.Close();

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                StreamWriter writer7 = new StreamWriter(logs11, true);
                writer7.WriteLine("exception  in file download " + ex.Message);
                writer7.Close();
            }
            _hub.Clients.All.SendAsync("getFileUploadMessage", true);
            return responseBack;
        }

        public string SaveDocumentToDisk(string fileName, byte[] documentData, string fullPath)
        {

            // var pathWithGuid = @pdfDocumentsFolder + foldeName;


            var fullFilePath = fullPath;
            if (System.IO.File.Exists(fullFilePath))
            {
                System.IO.File.Delete(fullFilePath);
            }
            FileStream stream = new FileStream(fullFilePath, FileMode.CreateNew);
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(documentData, 0, documentData.Length);
            writer.Close();
            //Console.WriteLine("File Path : " + downloadpathWithGuid);
            //            Console.WriteLine("Folder Path : " + fullFilePath);
            //            return downloadpathWithGuid;
            return null;
        }

        //public async Task2<responseBack> signuser(string FileName)
        //{
        //    try
        //    {
        //         var dbSignedUser = srassignedfilesdetails.GetAllSigningStatus
        //    }
        //}


        public async Task<responseBack> signStatus(string FileName)
        {
            string logs11 = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");
            if (!System.IO.File.Exists(logs11))
            {
                FileStream fs1 = new FileStream(logs11, FileMode.OpenOrCreate, FileAccess.Write);
            }

            responseBack responseBack = new responseBack();
            try
            {

                var dbEsignRecord = _e_SignRecordsAppService.GetAllE_Sign(2001);
                var find = dbEsignRecord.Where(x => x.FileName == FileName).FirstOrDefault();
                var accessToken = await ZohoESignGetAccessToken();
                if (find != null)
                {

                    string apiUrl = $"https://sign.zoho.in/api/v1/requests/{find.RequestId}/fielddata";

                    // Replace {documentId} with the actual document ID you want to download

                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                        // Make a POST request to download the document
                        HttpResponseMessage response = await client.GetAsync(apiUrl);
                        int SignPercentageZoho = 0;
                        var signStatus = string.Empty;
                        double Signin_percentage = 0;

                        if (response.IsSuccessStatusCode)
                        {
                            StreamWriter writer1 = new StreamWriter(logs11, true);
                            writer1.WriteLine("getting the response after file signed ");
                            writer1.Close();
                            // Read the PDF content
                            //  byte[] pdfBytes = await response.Content.ReadAsByteArrayAsync();
                            var responseBody = await response.Content.ReadAsStringAsync();

                            ZohoSignatureStatus myDeserializedClass = JsonConvert.DeserializeObject<ZohoSignatureStatus>(responseBody);

                            var signPercentage = myDeserializedClass.document_form_data.actions;
                            if (signPercentage != null)
                            {
                                var filePerUser = _srAssignedFilesDetailRepository.GetAll().Where(x => x.FileName == FileName).ToList();
                                StreamWriter writer2 = new StreamWriter(logs11, true);
                                writer2.WriteLine("Looping the  All action  ");
                                writer2.Close();


                                foreach (var action in signPercentage)
                                {
                                    if (!string.IsNullOrWhiteSpace(action.signed_time))
                                    {
                                        StreamWriter writer3 = new StreamWriter(logs11, true);
                                        writer3.WriteLine("File status signed ");
                                        writer3.Close();

                                        var findUser = _userRepository.GetAll().Where(x => x.EmailAddress == action.recipient_email).FirstOrDefault();
                                        if (findUser != null)
                                        {
                                            StreamWriter writer4 = new StreamWriter(logs11, true);
                                            writer4.WriteLine("Found the user in db");
                                            writer4.Close();


                                            var findPerUser = filePerUser.Where(x => x.UserId == findUser.Id).FirstOrDefault();
                                            findPerUser.SigningStatus = "Signed";
                                            await _srAssignedFilesDetailRepository.UpdateAsync(findPerUser);
                                        }
                                        Signin_percentage++;
                                    }
                                }

                                var calulatingSignPer = 100.0 / signPercentage.Count();
                                var finalPercentale = Signin_percentage * calulatingSignPer;

                                if (finalPercentale < 100)
                                {
                                    signStatus = "Signed";
                                    Signin_percentage = Math.Round(finalPercentale, 2);
                                    responseBack.signingStatus = "Partially Signed";
                                }
                                else if (finalPercentale == 0)
                                {
                                    signStatus = "Unsigned";
                                    responseBack.signingStatus = "Unsigned";
                                }
                                else if (finalPercentale == 100)
                                {
                                    signStatus = "Signed";
                                    Signin_percentage = Math.Round(finalPercentale, 2);
                                    responseBack.signingStatus = "Signed";
                                }
                            }
                            else
                            {
                                signStatus = "Unsigned";
                                responseBack.signingStatus = "Unsigned";
                            }

                            await _e_SignRecordsAppService.UpdateEsignStatus(find.Id, signStatus, Signin_percentage.ToString());

                            //SaveDocumentToDisk(find.FileName, pdfBytes, find.FullFilePath);
                            // Save the PDF to a file or process it as needed
                            // For example, you can save it to a file as follows:
                            // File.WriteAllBytes("downloaded.pdf", pdfBytes);

                            Console.WriteLine("PDF downloaded successfully.");
                        }
                        else
                        {
                            Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                StreamWriter writer4 = new StreamWriter(logs11, true);
                writer4.WriteLine("Exception Message " + ex.Message);
                writer4.Close();
            }
            return responseBack;
        }


        // Function to validate the format of the signed time string
        static bool ValidateSignedTime(string signedTime)
        {
            // Define the expected date format
            string dateFormat = "MMM dd yyyy HH:mm zzz";

            // Attempt to parse the signedTime string using the specified format
            if (DateTime.TryParseExact(signedTime, dateFormat, null, System.Globalization.DateTimeStyles.None, out _))
            {
                // The format is valid
                return true;
            }
            else
            {

                // The format is not valid
                return false;
            }
        }

        public List<EscrowDetail> GetEscrowUserList(string EscrowId)
        {
            List<EscrowDetail> EscrowUserList = new List<EscrowDetail>();
            try
            {

                using (var unit = _unitOfWorkManager.Begin())
                {
                    EscrowUserList = _escrowDetailRepository.GetAll().Where(x => x.EscrowId == EscrowId).ToList();
                    unit.Complete();
                }
            }
            catch (Exception ex)
            {

            }
            return EscrowUserList;
        }


        public async Task<responseBack> ZohoESignUpdateDocument(string fileNamePath, string esignKey, string EscrowId)
        {
            responseBack res = new responseBack();

            List<EscrowDetail> EscrowUserList = new List<EscrowDetail>();
            try
            {
                var accessToken = await ZohoESignGetAccessToken();
                List<ZohoSigninUserMapping> zohoSigninUserMapping = new List<ZohoSigninUserMapping>();
                List<UserTypeList> userTypeList = new List<UserTypeList>();
                List<UserTypeList> userTypeListInitials = new List<UserTypeList>();
                string fileTokenList = string.Empty;
                List<string> extractedCodes = new List<string>();
                List<ActionData> actionsDataOld = new List<ActionData>();
                string requestId = string.Empty;

                using (var unit = _unitOfWorkManager.Begin())
                {
                    EscrowUserList = _escrowDetailRepository.GetAll().Where(x => x.EscrowId == EscrowId).ToList();
                    unit.Complete();

                }



                using (HttpClient httpClient = new HttpClient())
                {
                    string sn = conf["foxitPdfEditor:sn"].ToString();
                    string key = conf["foxitPdfEditor:Key"].ToString();
                    string pdfFilePath = "wwwroot\\Common\\Paperless\\" + esignKey + fileNamePath;


                    string pdfFilePathAfterConvert = "wwwroot\\Common\\replacedSign.pdf";
                    string createFileForZoho = "wwwroot\\Common\\convertToSign.pdf";

                    pdfFilePath = pdfFilePath.Replace("/", "\\");
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    foxit.common.ErrorCode error_code;

                    try
                    {
                        error_code = foxit.common.Library.Initialize(sn, key);
                    }
                    catch (Exception ex)
                    {
                        error_code = foxit.common.Library.Reinitialize();
                    }

                    if (error_code != foxit.common.ErrorCode.e_ErrSuccess)
                    {
                        string logs11 = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");
                        if (!System.IO.File.Exists(logs11))
                        {
                            FileStream fs1 = new FileStream(logs11, FileMode.OpenOrCreate, FileAccess.Write);
                        }
                        StreamWriter writer11 = new StreamWriter(logs11, true);
                        writer11.WriteLine("Error in oppening foxit token ");
                        writer11.Close();
                    }
                    using (var unit = _unitOfWorkManager.Begin())
                    {
                        var dbESignRecords = _e_SignRecordsAppService.GetAllE_Sign(2001).Where(x => x.FileName == fileNamePath).FirstOrDefault();
                        if (dbESignRecords != null)
                        {
                            requestId = dbESignRecords.RequestId;
                            actionsDataOld = JsonConvert.DeserializeObject<List<ActionData>>(dbESignRecords.ZohoAction);
                        }
                        unit.Complete();
                    }

                    try
                    {
                        FileInfo file = new FileInfo(pdfFilePath);

                        if (file.Exists)
                        {
                            FileInfo fileZoho = new FileInfo(createFileForZoho);
                            if (fileZoho.Exists)
                            {
                                FileInfo fileConverted = new FileInfo(createFileForZoho);
                                fileConverted.Delete();
                            }
                            //createFileForZoho
                            System.IO.File.Copy(pdfFilePath, createFileForZoho);

                        }
                        int signingOrder = 1;
                        if (pdfFilePath.Contains("~'~") || pdfFilePath.Contains("-'-") || pdfFilePath.Contains("_'_"))
                        {
                            if (pdfFilePath.Contains("~'~"))
                                fileTokenList = pdfFilePath.Substring(pdfFilePath.LastIndexOf("~'~") + 3);
                            if (pdfFilePath.Contains("-'-"))
                                fileTokenList = pdfFilePath.Substring(pdfFilePath.LastIndexOf("-'-") + 3);
                            if (pdfFilePath.Contains("_'_"))
                                fileTokenList = pdfFilePath.Substring(pdfFilePath.LastIndexOf("_'_") + 3);

                            string match = String.Empty;

                            MatchCollection matches = regexx.Matches(fileTokenList);
                            for (int i = 0; i < matches.Count; i++)
                            {
                                string rep = matches[i].Value.Replace("{", "").Replace("}", "");
                                int index = rep.IndexOf('-');
                                match = rep.Substring(index + 1);
                                if (match.Contains("S"))
                                {
                                    extractedCodes.Add(rep.Substring(0, index));
                                }
                            }

                            List<EscrowDetail> filteredEscrowUserList = new List<EscrowDetail>();

                            // Filter using foreach loop with specific checks
                            foreach (var user in EscrowUserList)
                            {
                                // Check if extractedCodes contains SRX or BRX
                                if (extractedCodes.Contains("SRX") && user.Usertype.StartsWith("SR") ||
                                    extractedCodes.Contains("BRX") && user.Usertype.StartsWith("BR"))
                                {
                                    filteredEscrowUserList.Add(user);
                                }
                                // Check for exact match
                                else if (extractedCodes.Contains(user.Usertype))
                                {
                                    filteredEscrowUserList.Add(user);
                                }
                            }
                            EscrowUserList = filteredEscrowUserList.Where(x => x.Usertype != "EOX").ToList();


                            foreach (var item in actionsDataOld)
                            {
                                var find = EscrowUserList.Where(x => x.Email == item.recipient_email).FirstOrDefault();
                                if (find != null)
                                {
                                    UserTypeList userTypeItem = new UserTypeList();
                                    userTypeItem.isReplaced = true;
                                    userTypeItem.signingOrder = item.signing_order;
                                    userTypeItem.userType = find.Usertype;
                                    userTypeList.Add(userTypeItem);
                                    signingOrder = item.signing_order;
                                    signingOrder++;
                                }
                            }



                            var findWord = new List<string>();
                            var replaceWord = new List<string>();
                            int  idx = userTypeList.Max(x => x.signingOrder)+1;
                             
                            string updatedUrl = "";
                            var fileData = await uploadFileToTextReplace(createFileForZoho);
                            List<AddTextToReplaceDto> addTextToReplaceList = new List<AddTextToReplaceDto>();
                            foreach (var escrowUser in EscrowUserList)
                            {
                                var find = userTypeList.Where(x => x.userType == escrowUser.Usertype).FirstOrDefault();
                                if(find != null)
                                {
                                    continue;
                                }
;

                                AddTextToReplaceDto addTextResponse = await findPdfText($"@{{{escrowUser.Usertype}:S:1}}", $"{{{{S:R{idx}*}}}}", fileData.Url, Path.GetFileName(createFileForZoho));
                                if (string.IsNullOrWhiteSpace(addTextResponse.Url))
                                {

                                    findWord.Add($"@{{{escrowUser.Usertype}:S:1}}");
                                    replaceWord.Add($"{{{{S:R{idx}*}}}}");

                                    var escrowDetails = EscrowUserList.Where(x => x.Usertype == escrowUser.Usertype).FirstOrDefault();
                                    if (escrowDetails != null)
                                    {
                                        var check = zohoSigninUserMapping.Where(x => x.recipientEmail == escrowDetails.Email).FirstOrDefault();
                                        if (check == null)
                                        {
                                            ZohoSigninUserMapping zohoSignin = new ZohoSigninUserMapping();
                                            zohoSignin.recipientName = escrowDetails.Name;
                                            zohoSignin.recipientEmail = escrowDetails.Email;
                                            zohoSignin.signingOrder = idx;
                                            zohoSigninUserMapping.Add(zohoSignin);
                                        }
                                    }
                                }
                                else
                                {
                                    addTextToReplaceList.Add(addTextResponse);
                                    fileData.Url = addTextResponse.Url;

                                    var escrowDetails = EscrowUserList.Where(x => x.Usertype == escrowUser.Usertype).FirstOrDefault();
                                    if (escrowDetails != null)
                                    {
                                        var check = zohoSigninUserMapping.Where(x => x.recipientEmail == escrowDetails.Email).FirstOrDefault();
                                        if (check == null)
                                        {
                                            ZohoSigninUserMapping zohoSignin = new ZohoSigninUserMapping();
                                            zohoSignin.recipientName = escrowDetails.Name;
                                            zohoSignin.recipientEmail = escrowDetails.Email;
                                            zohoSignin.signingOrder = idx;
                                            zohoSigninUserMapping.Add(zohoSignin);
                                        }
                                    }
                                }
                                idx++;
                            }

                            // If you need to convert them to arrays afterward:
                            string[] findWordArray = findWord.ToArray();
                            string[] replaceWordArray = replaceWord.ToArray();
                            FileUploadResponse fileUploadResponse = new FileUploadResponse();
                            if (findWord.Count == 0 && addTextToReplaceList.Count > 0)
                            {
                                var lastUrl = addTextToReplaceList.LastOrDefault().Url;
                                var index = 1;
                                bool isAsync = false;
                                foreach (var item in addTextToReplaceList)
                                {
                                    if (index == addTextToReplaceList.Count)
                                    {
                                        isAsync = true;
                                    }
                                    fileUploadResponse = await AddPdfText(item.replaceWord, lastUrl, item.left, item.top, Path.GetFileName(createFileForZoho), isAsync);
                                    if (!string.IsNullOrWhiteSpace(fileUploadResponse.Url))
                                    {
                                        lastUrl = fileUploadResponse.Url;
                                    }
                                    if (index == addTextToReplaceList.Count)
                                    {
                                        fileData.Url = fileUploadResponse.Url;
                                        fileData.jobId = fileUploadResponse.jobId;
                                    }
                                    index++;
                                }

                            }

                            if (findWord.Count > 0)
                            {
                                var lastUrl = addTextToReplaceList.LastOrDefault().Url;
                                var index = 1;
                                bool isAsync = false;
                                foreach (var item in addTextToReplaceList)
                                {

                                    fileUploadResponse = await AddPdfText(item.replaceWord, lastUrl, item.left, item.top, Path.GetFileName(createFileForZoho), isAsync);
                                    if (!string.IsNullOrWhiteSpace(fileUploadResponse.Url))
                                    {
                                        lastUrl = fileUploadResponse.Url;
                                    }
                                    if (index == addTextToReplaceList.Count)
                                    {
                                        fileData.Url = fileUploadResponse.Url;
                                    }
                                    index++;
                                }
                                var textFindAndReplaceResponse = await TextFindAndReplace(fileData.Url, findWordArray, replaceWordArray, Path.GetFileName(createFileForZoho));
                                if (!string.IsNullOrWhiteSpace(textFindAndReplaceResponse.jobId))
                                {
                                    fileData.jobId = textFindAndReplaceResponse.jobId;
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(fileData.Url))
                            {
                                var jobStatus = await TextFindAndReplaceFileJobStatus(fileData.jobId);
                                await DownloadFileAsync(jobStatus.url, pdfFilePath);
                            }




                        }

                        //using (PDFDoc doc = new PDFDoc(createFileForZoho))
                        //{
                        //    error_code = doc.Load(null);

                        //    if (error_code != foxit.common.ErrorCode.e_ErrSuccess)
                        //    {
                        //        Console.WriteLine("The PDFDoc " + pdfFilePath + " Error: " + error_code);
                        //        // return;
                        //    }

                        //    int pageCount = doc.GetPageCount();



                        //    using (TextSearch search = new TextSearch(doc, null, (int)TextPage.TextParseFlags.e_ParseTextNormal))
                        //    {

                        //        for (int i = 0; i < pageCount; i++)
                        //        {
                        //            using (var page = doc.GetPage(i))
                        //            {
                        //                // Parse page
                        //                page.StartParse((int)PDFPage.ParseFlags.e_ParsePageNormal, null, false);
                        //                // Get the text select object.


                        //                using (var text_select = new TextPage(page, (int)TextPage.TextParseFlags.e_ParseTextNormal))
                        //                {
                        //                    int count = text_select.GetCharCount();
                        //                    if (count > 0)
                        //                    {
                        //                        String chars = text_select.GetChars(0, count);
                        //                        string[] lines = chars.Split('\n', ' ');
                        //                        //string[] filter = lines.Where(x => x.Contains("{{Signature_")).Distinct().ToArray();
                        //                        //string[] filterInitial = lines.Where(x => x.Contains("{{Initial_")).Distinct().ToArray();
                        //                        string[] filter = lines.Where(x => x.Contains("@{")).Distinct().ToArray();
                        //                        string[] filterInitial = lines.Where(x => x.Contains("{{Initial_")).Distinct().ToArray();

                        //                        if (filter.Length == 0)
                        //                        {
                        //                            continue;
                        //                        }


                        //                        foreach (var txt in filter)
                        //                        {

                        //                            var usertext = txt;
                        //                            bool isTextCorrected = true;

                        //                            if (usertext.Contains("\r"))
                        //                            {
                        //                                //isTextCorrected = false;
                        //                                usertext = usertext.Replace("\r", "");
                        //                            }

                        //                            string trimmedInput = usertext.TrimStart('@').TrimStart('{').TrimEnd('}');

                        //                            // Split the string by the colon (":")
                        //                            string[] parts = trimmedInput.Split(':');

                        //                            // Extract the values
                        //                            string part1 = parts[0]; // BR1
                        //                            string part2 = parts[1]; // S
                        //                                                     //  string part3 = (signingOrder++).ToString(); // 1

                        //                            UserTypeList TypeList = new UserTypeList();
                        //                            TypeList.userType = part1;

                        //                            TypeList.sign_type = part2;
                        //                            if (isTextCorrected == true)
                        //                            {
                        //                                TypeList.searchPattren = "@{" + TypeList.userType + ":" + TypeList.sign_type + ":1" + "}";
                        //                            }
                        //                            else
                        //                            {
                        //                                TypeList.searchPattren = "@{" + TypeList.userType + ":" + TypeList.sign_type + ":1" + "}\r";
                        //                            }
                        //                            //if(actionsDataOld.Count > 0)
                        //                            //{
                        //                            //    var escrowDetailsItem = EscrowUserList.Where(x => x.Usertype == TypeList.userType).FirstOrDefault();
                        //                            //    if (escrowDetailsItem != null)
                        //                            //    {
                        //                            //        // check here if user already signed
                        //                            //        var actionData = actionsDataOld.Where(x => x.recipient_email == escrowDetailsItem.Email).FirstOrDefault();
                        //                            //        if(actionData != null)
                        //                            //        {
                        //                            //            TypeList.signingOrder = actionData.signing_order;
                        //                            //            userTypeList.Add(TypeList);
                        //                            //        }
                        //                            //    }
                        //                            //}

                        //                            var checkUserAlready = userTypeList.Where(x => x.userType == TypeList.userType).FirstOrDefault();
                        //                            if (checkUserAlready == null && (EscrowUserList.Where(x => x.Usertype == TypeList.userType).FirstOrDefault() != null))
                        //                            {
                        //                                TypeList.signingOrder = signingOrder++;
                        //                                userTypeList.Add(TypeList);
                        //                            }



                        //                        }


                        //                        foreach (var item in userTypeList)
                        //                        {
                        //                            using (TextSearchReplace searchreplace = new TextSearchReplace(doc))
                        //                            using (FindOption find_option = new FindOption(false, false))
                        //                            using (ReplaceCallbackImpl replace_callback = new ReplaceCallbackImpl())
                        //                            {
                        //                                searchreplace.SetReplaceCallback(replace_callback);


                        //                                if (item.isReplaced == false)
                        //                                {
                        //                                    // searchreplace.SetPattern("{{Signature_" + item.userType + ":", i, find_option);
                        //                                    searchreplace.SetPattern(item.searchPattren, i, find_option);


                        //                                    //  while (searchreplace.ReplaceNext($"{{Signature:Recipient{item.signingOrder}}}"))
                        //                                    var replaceText = "{{S:R" + item.signingOrder + "*}}";
                        //                                    while (searchreplace.ReplaceNext(replaceText))
                        //                                    {
                        //                                        try
                        //                                        {
                        //                                            var escrowDetails = EscrowUserList.Where(x => x.Usertype == item.userType).FirstOrDefault();
                        //                                            if (escrowDetails != null)
                        //                                            {
                        //                                                var check = zohoSigninUserMapping.Where(x => x.recipientEmail == escrowDetails.Email).FirstOrDefault();
                        //                                                if (check == null)
                        //                                                {
                        //                                                    ZohoSigninUserMapping zohoSignin = new ZohoSigninUserMapping();
                        //                                                    zohoSignin.recipientName = escrowDetails.Name;
                        //                                                    zohoSignin.recipientEmail = escrowDetails.Email;
                        //                                                    zohoSignin.signingOrder = item.signingOrder;
                        //                                                    zohoSigninUserMapping.Add(zohoSignin);
                        //                                                }
                        //                                            }
                        //                                        }
                        //                                        catch (Exception ex)
                        //                                        {
                        //                                            throw ex;
                        //                                        }

                        //                                    }
                        //                                }
                        //                                else
                        //                                {
                        //                                    var escrowDetails = EscrowUserList.Where(x => x.Usertype == item.userType).FirstOrDefault();
                        //                                    ZohoSigninUserMapping zohoSignin = new ZohoSigninUserMapping();
                        //                                    zohoSignin.recipientName = escrowDetails.Name;
                        //                                    zohoSignin.recipientEmail = escrowDetails.Email;
                        //                                    zohoSignin.signingOrder = item.signingOrder;
                        //                                    zohoSigninUserMapping.Add(zohoSignin);
                        //                                }

                        //                            }
                        //                            doc.SaveAs(pdfFilePathAfterConvert, 0);
                        //                        }


                        //                        // replace initials in pdf 

                        //                        foreach (var txt in filterInitial)
                        //                        {
                        //                            string[] parts = txt.TrimStart('{').TrimEnd('}').Split(':');

                        //                            var getUserType = parts[0].Replace("Initial_", "");
                        //                            var getRecipeint = parts[1].Replace("Recipient", "");
                        //                            // var getUserType = parts[0].Replace("@{", "");
                        //                            // var getRecipeint = parts[2].Replace("}", "");
                        //                            char getRecipeint1 = getRecipeint[0];

                        //                            //userTypeList
                        //                            UserTypeList TypeList = new UserTypeList();
                        //                            TypeList.userType = getUserType;
                        //                            TypeList.signingOrder = int.Parse(getRecipeint1.ToString());
                        //                            //TypeList.signingOrder = int.Parse(getRecipeint.ToString());

                        //                            userTypeListInitials.Add(TypeList);
                        //                        }

                        //                        foreach (var item in userTypeListInitials)
                        //                        {
                        //                            using (TextSearchReplace searchreplace = new TextSearchReplace(doc))
                        //                            using (FindOption find_option = new FindOption(false, false))
                        //                            using (ReplaceCallbackImpl replace_callback = new ReplaceCallbackImpl())
                        //                            {
                        //                                searchreplace.SetReplaceCallback(replace_callback);
                        //                                searchreplace.SetPattern("{{Initial_" + item.userType + ":", i, find_option);
                        //                                // searchreplace.SetPattern("@{" + item.userType + ":", i, find_option);

                        //                                while (searchreplace.ReplaceNext("{{Initial:"))
                        //                                {

                        //                                    //try
                        //                                    //{
                        //                                    //    var escrowDetails = EscrowUserList.Where(x => x.Usertype == item.userType).FirstOrDefault();
                        //                                    //    if (escrowDetails != null)
                        //                                    //    {
                        //                                    //        var check = zohoSigninUserMapping.Where(x => x.recipientEmail == escrowDetails.Email).FirstOrDefault();
                        //                                    //        if (check == null)
                        //                                    //        {


                        //                                    //            ZohoSigninUserMapping zohoSignin = new ZohoSigninUserMapping();
                        //                                    //            zohoSignin.recipientName = escrowDetails.Name;
                        //                                    //            zohoSignin.recipientEmail = escrowDetails.Email;
                        //                                    //            zohoSignin.signingOrder = item.signingOrder;
                        //                                    //            zohoSigninUserMapping.Add(zohoSignin);
                        //                                    //        }
                        //                                    //    }
                        //                                    //}
                        //                                    //catch (Exception ex)
                        //                                    //{

                        //                                    //    throw ex;
                        //                                    //}

                        //                                }
                        //                            }
                        //                            doc.SaveAs(pdfFilePathAfterConvert, 0);
                        //                        }


                        //                    }

                        //                    Console.WriteLine("Search Replace demo finished.");
                        //                }
                        //            }
                        //        }
                        //    }
                        //}
                    }
                    catch (System.Exception e)
                    {
                        string logs3 = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\FoxitPdf.txt");
                        if (!System.IO.File.Exists(logs3))
                        {
                            FileStream fs1 = new FileStream(logs3, FileMode.OpenOrCreate, FileAccess.Write);
                        }
                        StreamWriter writer3 = new StreamWriter(logs3, true);
                        writer3.WriteLine("test3");

                        writer3.WriteLine($" Error Message:   {e.Message} " + DateTime.Now.ToString());
                        writer3.Close();
                        foxit.common.Library.Reinitialize();
                    }


                    //if (System.IO.File.Exists(pdfFilePathAfterConvert))
                    //{
                    //    FileInfo file = new FileInfo(pdfFilePath);
                    //    file.Delete();
                    //    System.IO.File.Copy(pdfFilePathAfterConvert, pdfFilePath);

                    //    FileInfo fileNew = new FileInfo(pdfFilePathAfterConvert);
                    //    fileNew.Delete();
                    //    FileInfo fileConverted = new FileInfo(createFileForZoho);
                    //    fileConverted.Delete();

                    //}

                    await deleteZohoPdf(accessToken, requestId);


                    var multipartContent = new MultipartFormDataContent();

                    byte[] pdfBytes = System.IO.File.ReadAllBytes(pdfFilePath);
                    ByteArrayContent fileContent = new ByteArrayContent(pdfBytes);
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                    string fileName = "esignFile.pdf";
                    multipartContent.Add(fileContent, "file", fileName); // "file" is the form field name
                    string logs1 = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\FoxitPdf.txt");
                    if (!System.IO.File.Exists(logs1))
                    {
                        FileStream fs1 = new FileStream(logs1, FileMode.OpenOrCreate, FileAccess.Write);
                    }
                    StreamWriter writer1 = new StreamWriter(logs1, true);
                    writer1.WriteLine($" createing zoho doc " + DateTime.Now.ToString());
                    writer1.Close();


                    CreateDocumentRequest createDocumentRequest = new CreateDocumentRequest();
                    createDocumentRequest.requests = new Requests();
                    createDocumentRequest.requests.request_name = "EsignRequst";
                    createDocumentRequest.requests.expiration_days = "1";
                    createDocumentRequest.requests.is_sequential = false;
                    createDocumentRequest.requests.email_reminders = true;
                    createDocumentRequest.requests.reminder_period = 10;
                    createDocumentRequest.requests.folder_id = conf["zoho:FolderId"].ToString();
                    createDocumentRequest.requests.actions = new List<Models.ZohoESign.Action>();

                    foreach (var item in zohoSigninUserMapping)
                    {
                        createDocumentRequest.requests.actions.Add(new Models.ZohoESign.Action
                        {
                            action_type = "SIGN",
                            recipient_email = item.recipientEmail,
                            recipient_name = item.recipientName,
                            signing_order = item.signingOrder,
                            verify_recipient = false,
                            verification_type = "EMAIL",
                            verification_code = "",
                            private_notes = "Please get back to us for further queries",
                            is_embedded = true,
                            is_bulk = true,

                        });
                    }



                    var json = JsonConvert.SerializeObject(createDocumentRequest);
                    multipartContent.Add(new StringContent(json), "data");

                    HttpResponseMessage response = await httpClient.PostAsync($"https://sign.zoho.in/api/v1/requests?testing=true", multipartContent);



                    if (response.IsSuccessStatusCode)
                    {

                        string responseBody = await response.Content.ReadAsStringAsync();
                        CreateDocumentResponse CreateDocumentResponse = JsonConvert.DeserializeObject<CreateDocumentResponse>(responseBody);

                        string logs2 = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\FoxitPdf.txt");
                        if (!System.IO.File.Exists(logs2))
                        {

                            FileStream fs1 = new FileStream(logs2, FileMode.OpenOrCreate, FileAccess.Write);
                        }
                        StreamWriter writer2 = new StreamWriter(logs1, true);
                        writer2.WriteLine($" response success " + DateTime.Now.ToString());
                        writer2.Close();



                        var dbEsignRecord = _esignRepository.GetAll().Where(x => x.RequestId == requestId).FirstOrDefault();
                        if (dbEsignRecord != null)
                        {
                            var requests = CreateDocumentResponse.requests;
                            dbEsignRecord.ZohoAction = JsonConvert.SerializeObject(requests.actions);
                            dbEsignRecord.RequestId = requests.request_id;
                            if (!string.IsNullOrWhiteSpace(requests.document_ids.FirstOrDefault().document_id))
                            {
                                dbEsignRecord.DocumentId = long.Parse(requests.document_ids.FirstOrDefault().document_id);
                            }

                        }
                        var dbChanges = _esignRepository.InsertOrUpdate(dbEsignRecord);

                        string logs5 = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\FoxitPdf.txt");
                        if (!System.IO.File.Exists(logs2))
                        {

                            FileStream fs1 = new FileStream(logs5, FileMode.OpenOrCreate, FileAccess.Write);
                        }
                        StreamWriter writer5 = new StreamWriter(logs5, true);
                        writer5.WriteLine($" updated zoho file in db success " + DateTime.Now.ToString());
                        writer5.Close();

                    }
                    else
                    {
                        string logs8 = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\FoxitPdf.txt");
                        if (!System.IO.File.Exists(logs8))
                        {
                            FileStream fs1 = new FileStream(logs8, FileMode.OpenOrCreate, FileAccess.Write);
                        }

                        string responseBody = await response.Content.ReadAsStringAsync();
                        StreamWriter writer2 = new StreamWriter(logs8, true);
                        writer2.WriteLine($" response unsuccess " + responseBody);
                        writer2.WriteLine($"folder_id " + "70994000000031001");
                        writer2.WriteLine("from dto" + json);
                        writer2.Close();

                        CreateOrEditE_SignRecordDto std = new CreateOrEditE_SignRecordDto();

                        std.FolderId = 0;
                        std.FolderName = "Not valid file";
                        //std.FileName = esignKey;
                        std.Status = "Unsigned";
                        std.RequestId = "0";
                        std.DocumentId = 0;
                        std.FullFilePath = pdfFilePath;
                        std.ZohoAction = "";



                        std.EsignCompanyCode = 2001;
                        // std.CompanyId = requests.owner_id;
                        std.EmailId = "";
                        //  std.EmbeddedToken = sign.embeddedToken;
                        //std.EmbeddedURL = sign.embeddedSessionURL;
                        var i = await _e_SignRecordsAppService.CreateOrEdit(std);

                    }

                }
            }
            catch (Exception ex)
            {

            }
            return res;
        }



        public async Task<responseBack> deleteZohoPdf(string accessToken, string RequestId)
        {
            try
            {

                string apiUrl = $"https://sign.zoho.in/api/v1/requests/{RequestId}/delete";
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                    HttpResponseMessage response = await client.PutAsync(apiUrl, null);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                    }
                }

            }
            catch (Exception ex)
            {

            }
            return null;
        }


        public void LogMessage(string methodName, string errorMessage)
        {

            string logFilePath = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");
            string directoryPath = Path.GetDirectoryName(logFilePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine($"FunctionName : {methodName} - Message: {errorMessage} - Timestamp: {DateTime.Now}");
            }
        }


        // Text Find and replace Api Integration
        static async Task<FileUploadResponse> uploadFileToTextReplace(string filePath)
        {
            FileUploadResponse responseData = new FileUploadResponse();
            string apiUrl = "https://api.pdf.co/v1/file/upload";
            string apiKey = "bloodhollow01@gmail.com_YOWMUbSfp9BYXmkdmjbiQ0VTAEF14RqmbmHui489hKFciApvKGtCLcy56ULcrxSi";

            if (!System.IO.File.Exists(filePath))
            {
                Console.WriteLine("File not found: " + filePath);
                return responseData;
            }

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("x-api-key", apiKey);

                using (var content = new MultipartFormDataContent())
                {
                    var fileContent = new ByteArrayContent(System.IO.File.ReadAllBytes(filePath));
                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");
                    content.Add(fileContent, "file", Path.GetFileName(filePath));
                    content.Add(new StringContent($"{Path.GetFileName(filePath)}"), "name");

                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        responseData = JsonConvert.DeserializeObject<FileUploadResponse>(result);
                        return responseData;
                    }
                    else
                    {
                        Console.WriteLine("Failed to upload file. Status Code: " + response.StatusCode);
                        Console.WriteLine("Response: " + await response.Content.ReadAsStringAsync());
                    }
                }
            }
            return responseData;
        }


        //static async Task<FileUploadResponse> uploadFileToTextReplace(string filePath)
        //{
        //    FileUploadResponse responseData = new FileUploadResponse();
        //    string apiUrl = "https://api.pdf.co/v1/file/upload";
        //    string apiKey = "puneet@mandavconsultancy.com_nrLCUdnweUjPh3OK4rXaFwOoYWmLRXXhUGV6wAF3SLW6ovv4mKnzxlrJvWHDLYif";

        //    if (!System.IO.File.Exists(filePath))
        //    {
        //        Console.WriteLine("File not found: " + filePath);
        //        return responseData;
        //    }

        //    string fileExtension = Path.GetExtension(filePath).ToLower();

        //    // Determine the appropriate Content-Type
        //    string contentType = fileExtension switch
        //    {
        //        ".pdf" => "application/pdf",
        //        ".doc" => "application/msword",
        //        ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        //        _ => throw new NotSupportedException($"File extension '{fileExtension}' is not supported.")
        //    };

        //    using (HttpClient client = new HttpClient())
        //    {
        //        client.DefaultRequestHeaders.Add("x-api-key", apiKey);

        //        using (var content = new MultipartFormDataContent())
        //        {
        //            var fileContent = new ByteArrayContent(System.IO.File.ReadAllBytes(filePath));
        //            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
        //            content.Add(fileContent, "file", Path.GetFileName(filePath));
        //            content.Add(new StringContent($"{Path.GetFileName(filePath)}"), "name");

        //            HttpResponseMessage response = await client.PostAsync(apiUrl, content);

        //            if (response.IsSuccessStatusCode)
        //            {
        //                string result = await response.Content.ReadAsStringAsync();
        //                responseData = JsonConvert.DeserializeObject<FileUploadResponse>(result);
        //                return responseData;
        //            }
        //            else
        //            {
        //                Console.WriteLine("Failed to upload file. Status Code: " + response.StatusCode);
        //                Console.WriteLine("Response: " + await response.Content.ReadAsStringAsync());
        //            }
        //        }
        //    }
        //    return responseData;
        //}


        static async Task<FileUploadResponse> TextFindAndReplace(string url, string[] searchStrings, string[] replaceStrings, string fileName)
        {
            FileUploadResponse responseData = new FileUploadResponse();
            string API_KEY = "bloodhollow01@gmail.com_YOWMUbSfp9BYXmkdmjbiQ0VTAEF14RqmbmHui489hKFciApvKGtCLcy56ULcrxSi";
            const string ApiUrl = "https://api.pdf.co/v1/pdf/edit/replace-text";
            var jsonBody = new
            {
                url = url,
                searchStrings = searchStrings,
                replaceStrings = replaceStrings,
                caseSensitive = true,
                replacementLimit = 1,
                pages = "",
                password = "",
                name = fileName,
                async = true
            };

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("x-api-key", API_KEY);

                HttpResponseMessage response = await client.PostAsync(
                    ApiUrl,
                    new StringContent(JsonConvert.SerializeObject(jsonBody), Encoding.UTF8, "application/json")
                );

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    responseData = JsonConvert.DeserializeObject<FileUploadResponse>(result);
                    return responseData;

                }
                else
                {
                    string errorDetails = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Error: " + response.StatusCode);
                    Console.WriteLine("Details: " + errorDetails);
                }
            }
            return responseData;
        }

        static async Task<JobStatusResponse> TextFindAndReplaceFileJobStatus(string jobid)
        {
            JobStatusResponse responseData = new JobStatusResponse();
            string API_KEY = "bloodhollow01@gmail.com_YOWMUbSfp9BYXmkdmjbiQ0VTAEF14RqmbmHui489hKFciApvKGtCLcy56ULcrxSi";
            const string ApiUrl = "https://api.pdf.co/v1/job/check";
            var jsonBody = new
            {
                jobid = jobid
            };

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("x-api-key", API_KEY);

                HttpResponseMessage response = await client.PostAsync(
                    ApiUrl,
                    new StringContent(JsonConvert.SerializeObject(jsonBody), Encoding.UTF8, "application/json")
                );

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    responseData = JsonConvert.DeserializeObject<JobStatusResponse>(result);
                    return responseData;

                }
                else
                {
                    string errorDetails = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Error: " + response.StatusCode);
                    Console.WriteLine("Details: " + errorDetails);
                }
            }
            return responseData;
        }

        static async Task DownloadFileAsync(string fileUrl, string destinationPath)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(fileUrl);

                if (response.IsSuccessStatusCode)
                {
                    using (var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await response.Content.CopyToAsync(fileStream);
                    }
                }
                else
                {
                    throw new Exception($"Failed to download file. Status Code: {response.StatusCode}");
                }
            }
        }



        static async Task<AddTextToReplaceDto> findPdfText(string SearchText, string replaceWord, string url, string fileName)
        {

            AddTextToReplaceDto addTextToReplaceDto = new AddTextToReplaceDto();
            string API_KEY = "bloodhollow01@gmail.com_YOWMUbSfp9BYXmkdmjbiQ0VTAEF14RqmbmHui489hKFciApvKGtCLcy56ULcrxSi";
            const string ApiUrl = "https://api.pdf.co/v1/pdf/find";
            var data = new
            {
                async = false,
                url = url,
                searchString = $"____________________________________________{SearchText}",
                regexSearch = true,
                name = "output",
                pages = "0-",
                inline = true,
                wordMatchingMode = "",
                password = ""
            };

            using (HttpClient client = new HttpClient())
            {
                try
                {

                    client.DefaultRequestHeaders.Add("x-api-key", API_KEY);

                    HttpResponseMessage response = await client.PostAsync(
                        ApiUrl,
                        new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
                    );

                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        var pdfTextPositins = JsonConvert.DeserializeObject<PdfTextPositins>(result);
                        if (pdfTextPositins.body.Count > 0)
                        {
                            // delete Api
                            var DeleteSuccessFully = await DeletePdfText(SearchText, url, fileName);
                            if (!string.IsNullOrWhiteSpace(DeleteSuccessFully.Url))
                            {
                                addTextToReplaceDto.replaceWord = replaceWord;
                                addTextToReplaceDto.left = pdfTextPositins.body.FirstOrDefault().left;
                                addTextToReplaceDto.top = pdfTextPositins.body.FirstOrDefault().top;
                                addTextToReplaceDto.Url = DeleteSuccessFully.Url;
                                return addTextToReplaceDto;
                            }

                        }

                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception occurred: {ex.Message}");
                }
                return addTextToReplaceDto;
            }
        }

        static async Task<FileUploadResponse> DeletePdfText(string deleteText, string url, string fileName)
        {
            FileUploadResponse responseData = new FileUploadResponse();
            string API_KEY = "bloodhollow01@gmail.com_YOWMUbSfp9BYXmkdmjbiQ0VTAEF14RqmbmHui489hKFciApvKGtCLcy56ULcrxSi";
            const string ApiUrl = "https://api.pdf.co/v1/pdf/edit/delete-text";
            var jsonBody = new
            {
                url = url,
                name = fileName,
                caseSensitive = "false",
                searchString = deleteText,
                replacementLimit = 0,
                async = false
            };

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("x-api-key", API_KEY);

                HttpResponseMessage response = await client.PostAsync(
                    ApiUrl,
                    new StringContent(JsonConvert.SerializeObject(jsonBody), Encoding.UTF8, "application/json")
                );
                string result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    responseData = JsonConvert.DeserializeObject<FileUploadResponse>(result);


                    return responseData;

                }

            }
            return responseData;
        }


        static async Task<FileUploadResponse> AddPdfText(string AddText, string url, double Xpositions, double Ypositions, string fileName, bool isAsync)
        {

            FileUploadResponse responseData = new FileUploadResponse();
            string API_KEY = "bloodhollow01@gmail.com_YOWMUbSfp9BYXmkdmjbiQ0VTAEF14RqmbmHui489hKFciApvKGtCLcy56ULcrxSi";
            const string ApiUrl = "https://api.pdf.co/v1/pdf/edit/add";
            var jsonBody = new
            {
                async = isAsync,
                inline = true,
                name = fileName,
                url = url,
                annotations = new[]
            {
                new
                {
                    text = AddText,
                    x = Xpositions +50,
                    y = Ypositions-6,
                    size = 8,
                    color = System.Drawing.Color.White,
                    pages = "0-"
                }
            }
            };

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("x-api-key", API_KEY);

                HttpResponseMessage response = await client.PostAsync(
                    ApiUrl,
                    new StringContent(JsonConvert.SerializeObject(jsonBody), Encoding.UTF8, "application/json")
                );
                string result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    responseData = JsonConvert.DeserializeObject<FileUploadResponse>(result);
                    return responseData;
                }

            }
            return responseData;
        }


        private async Task<FileUploadResponse> UploadFileToPdfCo(string filePath)
        {
            string apiUrl = "https://api.pdf.co/v1/file/upload";
            string apiKey = "bloodhollow01@gmail.com_YOWMUbSfp9BYXmkdmjbiQ0VTAEF14RqmbmHui489hKFciApvKGtCLcy56ULcrxSi";
            FileUploadResponse response = new FileUploadResponse();

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("x-api-key", apiKey);

                using (var content = new MultipartFormDataContent())
                {
                    var fileContent = new ByteArrayContent(System.IO.File.ReadAllBytes(filePath));
                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
                    content.Add(fileContent, "file", Path.GetFileName(filePath));

                    HttpResponseMessage httpResponse = await client.PostAsync(apiUrl, content);

                    if (httpResponse.IsSuccessStatusCode)
                    {
                        var result = await httpResponse.Content.ReadAsStringAsync();
                        response = JsonConvert.DeserializeObject<FileUploadResponse>(result);
                    }
                    else
                    {
                        response.Error = false;
                        response.Name = "Upload failed: " + await httpResponse.Content.ReadAsStringAsync();
                    }
                }
            }

            return response;
        }

        #endregion


        private async Task<ConvertResponse> ConvertDocToPdf(string fileUrl)
        {
            string apiUrl = "https://api.pdf.co/v1/pdf/convert/from/doc";
            string apiKey = "bloodhollow01@gmail.com_YOWMUbSfp9BYXmkdmjbiQ0VTAEF14RqmbmHui489hKFciApvKGtCLcy56ULcrxSi";
            ConvertResponse response = new ConvertResponse();

            var requestBody = new
            {
                url = fileUrl,
                name = "converted.pdf",
                async = false
            };

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("x-api-key", apiKey);
                var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

                HttpResponseMessage httpResponse = await client.PostAsync(apiUrl, jsonContent);

                if (httpResponse.IsSuccessStatusCode)
                {
                    var result = await httpResponse.Content.ReadAsStringAsync();
                    response = JsonConvert.DeserializeObject<ConvertResponse>(result);
                    response.success = true;
                }
                else
                {
                    response.success = false;
                    response.error = "Conversion failed: " + await httpResponse.Content.ReadAsStringAsync();
                }
            }

            return response;
        }


    }

    /// <summary>
    /// ReplaceCallback for foxit
    /// </summary>
    class ReplaceCallbackImpl : ReplaceCallback
    {
        public ReplaceCallbackImpl()
        {
        }
        public int count = 0;
        public override void Dispose()
        {
        }
        public override bool NeedToReplace(string search_text, string replace_text, int current_page_index, foxit.common.fxcrt.RectFArray text_rect_array)
        {
            count++;
            if (count <= 10) return true;
            else return false;
        }
        public override void Release() { }
    }

    public class ZohoSignWebhookPayload
    {
        public string EventType { get; set; } // Event type, e.g., "document.completed"
        public string DocumentId { get; set; } // ID of the signed document
        public DateTime EventTime { get; set; } // Timestamp of the event
                                                // Other properties you might need based on the webhook data
    }


    ///<Summary>f
    /// Class UserCompany
    ///</Summary>
    public class UserCompany
    {
        ///<Summary>
        /// Parameter escrowId
        ///</Summary>
        public string escrowId { get; set; }

        ///<Summary>
        /// Parameter type
        ///</Summary>
        public string type { get; set; }

        ///<Summary>
        /// Parameter company
        ///</Summary>
        public string company { get; set; }

        ///<Summary>
        /// Parameter subCompany
        ///</Summary>
        public string subCompany { get; set; }

        ///<Summary>
        /// Parameter address
        ///</Summary>
        public string address { get; set; }

        ///<Summary>
        /// Parameter buyer
        ///</Summary>
        public string buyer { get; set; }

        ///<Summary>
        /// Parameter seller
        ///</Summary>
        public string seller { get; set; }
    }

    ///<Summary>
    /// Class user data
    ///</Summary>
    public class UserData
    {
        ///<Summary>
        /// Parameter fromEmail
        ///</Summary>
        public string fromEmail { get; set; }

        ///<Summary>
        /// Parameter name
        ///</Summary>
        public string name { get; set; }

        ///<Summary>
        /// Parameter company
        ///</Summary>
        public string company { get; set; }

        ///<Summary>
        /// Parameter invitee
        ///</Summary>
        public string invitee { get; set; }

        ///<Summary>
        /// Parameter escro
        ///</Summary>
        public string escro { get; set; }

        ///<Summary>
        /// Parameter type
        ///</Summary>
        public string type { get; set; }
    }

    ///<Summary>
    /// Class enterprise new
    ///</Summary>
    public class enterpriseNew
    {
        ///<Summary>
        /// Parameter CompanyName
        ///</Summary>
        public string CompanyName { get; set; }

        ///<Summary>
        /// Parameter SubCompanyName
        ///</Summary>
        public string SubCompanyName { get; set; }

        ///<Summary>
        /// Parameter Escrow
        ///</Summary>
        public string Escrow { get; set; }

        ///<Summary>
        /// Parameter PropertyAddress
        ///</Summary>
        public string PropertyAddress { get; set; }

        ///<Summary>
        /// Parameter EscrowOfficerName
        ///</Summary>
        public string EscrowOfficerName { get; set; }

        ///<Summary>
        /// Parameter EOEmail
        ///</Summary>
        public string EOEmail { get; set; }

        ///<Summary>
        /// Parameter EOPhone
        ///</Summary>
        public string EOPhone { get; set; }

        ///<Summary>
        /// Parameter EoPhoneExt
        ///</Summary>
        public string EoPhoneExt { get; set; }

        ///<Summary>
        /// Parameter EoPhoneCell
        ///</Summary>
        public string EoPhoneCell { get; set; }
    }

    ///<Summary>
    /// Class E-sign
    ///</Summary>
    public class Esign
    {
        ///<Summary>
        /// SubClass EmbeddedSigningSession
        ///</Summary>
        public class EmbeddedSigningSession
        {
            ///<Summary>
            /// Parameter emailIdOfSigner
            ///</Summary>
            public string emailIdOfSigner { get; set; }

            ///<Summary>
            /// Parameter embeddedToken
            ///</Summary>
            public string embeddedToken { get; set; }

            ///<Summary>
            /// Parameter embeddedSessinURL
            ///</Summary>
            public string embeddedSessionURL { get; set; }
        }

        ///<Summary>
        /// SubClass Documentslist
        ///</Summary>
        public class DocumentsList
        {
            ///<Summary>
            /// Parameter documentId
            ///</Summary>
            public int documentId { get; set; }

            ///<Summary>
            /// Parameter contractId
            ///</Summary>
            public int contractId { get; set; }

            ///<Summary>
            /// Parameter companyId
            ///</Summary>
            public int companyId { get; set; }

            ///<Summary>
            /// Parameter contractCreatedBy
            ///</Summary>
            public int contractCreatedBy { get; set; }

            ///<Summary>
            /// Parameter contractCreatedOn
            ///</Summary>
            public object contractCreatedOn { get; set; }

            ///<Summary>
            /// Parameter contractType
            ///</Summary>
            public object contractType { get; set; }

            ///<Summary>
            /// Parameter contractStatus
            ///</Summary>
            public string contractStatus { get; set; }





            ///<Summary>
            /// Parameter editable
            ///</Summary>
            public bool editable { get; set; }

            ///<Summary>
            /// Parameter contractVersionId
            ///</Summary>
            public int contractVersionId { get; set; }

            ///<Summary>
            /// Parameter contractVersionName
            ///</Summary>
            public string contractVersionName { get; set; }

            ///<Summary>
            /// Parameter contractVersionDesc
            ///</Summary>
            public object contractVersionDesc { get; set; }

            ///<Summary>
            /// Parameter versionCreatedby
            ///</Summary>
            public int versionCreatedby { get; set; }

            ///<Summary>
            /// Parameter versionCreatedOn
            ///</Summary>
            public object versionCreatedOn { get; set; }

            ///<Summary>
            /// Parameter contractVersionNumber
            ///</Summary>
            public int contractVersionNumber { get; set; }

            ///<Summary>
            /// Parameter contractTransactionSource
            ///</Summary>
            public string contractTransactionSource { get; set; }
        }

        ///<Summary>
        /// SubClass PartyDetails
        ///</Summary>
        public class PartyDetails
        {
            ///<Summary>
            /// Parameter partyId
            ///</Summary>
            public int partyId { get; set; }

            ///<Summary>
            /// Parameter firstName
            ///</Summary>
            public string firstName { get; set; }

            ///<Summary>
            /// Parameter lastName
            ///</Summary>
            public string lastName { get; set; }

            ///<Summary>
            /// Parameter emailId
            ///</Summary>
            public string emailId { get; set; }

            ///<Summary>
            /// Parameter placeholder
            ///</Summary>
            public bool placeholder { get; set; }

            ///<Summary>
            /// Parameter address
            ///</Summary>
            public object address { get; set; }

            ///<Summary>
            /// Parameter dateCreated
            ///</Summary>
            public object dateCreated { get; set; }

            ///<Summary>
            /// Parameter optOutEmails
            ///</Summary>
            ///
            public bool optOutE0mails { get; set; }

            ///<Summary>
            /// Parameter authenticationLevel
            ///<0/Summa000ry>
            public string authenticationLevel { get; set; }

            ///<Summary>
            /// Parameter companyId
            ///</Summary>
            public int companyId { get; set; }

            ///<Summary>
            /// Parameter subcontractorDescription
            ///</Summary>
            public object subcontractorDescription { get; set; }

            ///<Summary>
            /// Parameter subcontractorOrganization
            ///</Summary>
            public object subcontractorOrganization { get; set; }

            ///<Summary>
            /// Parameter subcAddedDate
            ///</Summary>
            public long? subcAddedDate { get; set; }

            ///<Summary>
            /// Parameter dialingCode
            ///</Summary>
            public string dialingCode { get; set; }

            ///<Summary>
            /// Parameter mobileNumber
            ///</Summary>
            public object mobileNumber { get; set; }
        }

        ///<Summary>
        /// SubClass FolderRecipientParty
        ///</Summary>
        public class FolderRecipientParty
        {
            ///<Summary>
            /// Parameter partyId
            ///</Summary>
            public int partyId { get; set; }

            ///<Summary>
            /// Parameter partyDetails
            ///</Summary>
            public PartyDetails partyDetails { get; set; }

            ///<Summary>
            /// Parameter dialingCode
            ///</Summary>
            public string dialingCode { get; set; }

            ///<Summary>
            /// Parameter mobileNumber
            ///</Summary>
            public object mobileNumber { get; set; }

            ///<Summary>
            /// Parameter contractPermissions
            ///</Summary>
            public string contractPermissions { get; set; }

            ///<Summary>
            /// Parameter partySequence
            ///</Summary>
            public int partySequence { get; set; }

            ///<Summary>
            /// Parameter workflowSignSequence
            ///</Summary>
            public int workflowSignSequence { get; set; }

            ///<Summary>
            /// Parameter envelopeId
            ///</Summary>
            public int envelopeId { get; set; }

            ///<Summary>
            /// Parameter sharingMode
            ///</Summary>
            public string sharingMode { get; set; }

            ///<Summary>
            /// Parameter folderAccessURL
            ///</Summary>
            public string folderAccessURL { get; set; }

            ///<Summary>
            /// Parameter securityMode
            ///</Summary>
            public string securityMode { get; set; }

            ///<Summary>
            /// Parameter extraComments
            ///</Summary>
            public object extraComments { get; set; }

            ///<Summary>
            /// Parameter allowNameChange
            ///</Summary>
            public bool allowNameChange { get; set; }

            ///<Summary>
            /// Parameter signerNameUpdated
            ///</Summary>
            public bool signerNameUpdated { get; set; }

            ///<Summary>
            /// Parameter signerAuthenticationLevel
            ///</Summary>
            public string signerAuthenticationLevel { get; set; }

            ///<Summary>
            /// Parameter signatureId
            ///</Summary>
            public object signatureId { get; set; }

            ///<Summary>
            /// Parameter partyRole
            ///</Summary>
            public object partyRole { get; set; }
        }

        ///<Summary>
        /// SubClass EnvelopePartyPermission
        ///</Summary>
        public class EnvelopePartyPermission
        {
            ///<Summary>
            /// Parameter partyId
            ///</Summary>
            public int partyId { get; set; }

            ///<Summary>
            /// Parameter partyDetails
            ///</Summary>
            public PartyDetails partyDetails { get; set; }

            ///<Summary>
            /// Parameter dialingCode
            ///</Summary>
            public string dialingCode { get; set; }

            ///<Summary>
            /// Parameter mobileNumber
            ///</Summary>
            public object mobileNumber { get; set; }

            ///<Summary>
            /// Parameter contractPermissions
            ///</Summary>
            public string contractPermissions { get; set; }

            ///<Summary>
            /// Parameter partySequence
            ///</Summary>
            public int partySequence { get; set; }

            ///<Summary>
            /// Parameter workflowSignSequence
            ///</Summary>
            public int workflowSignSequence { get; set; }

            ///<Summary>
            /// Parameter envelopeId
            ///</Summary>
            public int envelopeId { get; set; }

            ///<Summary>
            /// Parameter sharingMode
            ///</Summary>
            public string sharingMode { get; set; }

            ///<Summary>
            /// Parameter folderAccessURL
            ///</Summary>
            public string folderAccessURL { get; set; }

            ///<Summary>
            /// Parameter securityMode
            ///</Summary>
            public string securityMode { get; set; }

            ///<Summary>
            /// Parameter extraComments
            ///</Summary>
            public object extraComments { get; set; }

            ///<Summary>
            /// Parameter allowNameChange
            ///</Summary>
            public bool allowNameChange { get; set; }

            ///<Summary>
            /// Parameter signerNameUpdated
            ///</Summary>
            public bool signerNameUpdated { get; set; }

            ///<Summary>
            /// Parameter signerAuthenticationLevel
            ///</Summary>
            public string signerAuthenticationLevel { get; set; }

            ///<Summary>
            /// Parameter signatureId
            ///</Summary>
            public object signatureId { get; set; }

            ///<Summary>
            /// Parameter partyRole
            ///</Summary>
            public object partyRole { get; set; }
        }

        ///<Summary>
        /// SubClass Folder
        ///</Summary>
        public class Folder
        {
            ///<Summary>
            /// Parameter folderId
            ///</Summary>
            public int folderId { get; set; }

            ///<Summary>
            /// Parameter folderName
            ///</Summary>
            public string folderName { get; set; }

            ///<Summary>
            /// Parameter folderCustomName
            ///</Summary>
            public string folderCustomName { get; set; }

            ///<Summary>
            /// Parameter folderPassword
            ///</Summary>
            public object folderPassword { get; set; }

            ///<Summary>
            /// Parameter folderAuthorId
            ///</Summary>
            public int folderAuthorId { get; set; }

            ///<Summary>
            /// Parameter foderAuthorFirstName
            ///</Summary>
            public object folderAuthorFirstName { get; set; }

            ///<Summary>
            /// Parameter folderAuthorLastName
            ///</Summary>
            public object folderAuthorLastName { get; set; }

            ///<Summary>
            /// Parameter folderAuthorEmail
            ///</Summary>
            public object folderAuthorEmail { get; set; }

            ///<Summary>
            /// Parameter folderAuthorRole
            ///</Summary>
            public object folderAuthorRole { get; set; }

            ///<Summary>
            /// Parameter folderCompanyId
            ///</Summary>
            public int folderCompanyId { get; set; }

            ///<Summary>
            /// Parameter folderCreationDate
            ///</Summary>
            public long folderCreationDate { get; set; }

            ///<Summary>
            /// Parameter folderSentDate
            ///</Summary>
            public long folderSentDate { get; set; }

            ///<Summary>
            /// Parameter folderStatus
            ///</Summary>
            public string folderStatus { get; set; }

            ///<Summary>
            /// Parameter custom_field1
            ///</Summary>
            public object custom_field1 { get; set; }

            ///<Summary>
            /// Parameter custom_field2
            ///</Summary>
            public object custom_field2 { get; set; }

            ///<Summary>
            /// Parameter folderDocumentIds
            ///</Summary>
            public List<int> folderDocumentIds { get; set; }

            ///<Summary>
            /// Parameter documentsList
            ///</Summary>
            public List<DocumentsList> documentsList { get; set; }

            ///<Summary>
            /// Parameter folderRecipientParties
            ///</Summary>
            public List<FolderRecipientParty> folderRecipientParties { get; set; }

            ///<Summary>
            /// Parameter folderAccessURLForAuthor
            ///</Summary>
            public object folderAccessURLForAuthor { get; set; }

            ///<Summary>
            /// Parameter boardRoomSign
            ///</Summary>
            public bool boardRoomSign { get; set; }

            ///<Summary>
            /// Parameter bulkId
            ///</Summary>
            public int bulkId { get; set; }

            ///<Summary>
            /// Parameter enforceSignWorkflow
            ///</Summary>
            public bool enforceSignWorkflow { get; set; }

            ///<Summary>
            /// Parameter currentWorkflowStep
            ///</Summary>
            public int currentWorkflowStep { get; set; }

            ///<Summary>
            /// Parameter transactionSource
            ///</Summary>
            public string transactionSource { get; set; }

            ///<Summary>
            /// Parameter editable
            ///</Summary>
            public bool editable { get; set; }

            ///<Summary>
            /// Parameter inPersonSignable
            ///</Summary>
            public bool inPersonSignable { get; set; }

            ///<Summary>
            /// Parameter overrideAccountReminders
            ///</Summary>
            public bool overrideAccountReminders { get; set; }

            ///<Summary>
            /// Parameter overrideAccountRecipientDelegation
            ///</Summary>
            public bool overrideAccountRecipientDelegation { get; set; }

            ///<Summary>
            /// Parameter allowRecipientsToDelegate
            ///</Summary>
            public bool allowRecipientsToDelegate { get; set; }

            ///<Summary>
            /// Parameter envelopeId
            ///</Summary>
            public int envelopeId { get; set; }

            ///<Summary>
            /// Parameter envelopeName
            ///</Summary>
            public string envelopeName { get; set; }

            ///<Summary>
            /// Parameter envelopeOriginatorId
            ///</Summary>
            public int envelopeOriginatorId { get; set; }

            ///<Summary>
            /// Parameter envelopeCompanyId
            ///</Summary>
            public int envelopeCompanyId { get; set; }

            ///<Summary>
            /// Parameter envelopeDate
            ///</Summary>
            public long envelopeDate { get; set; }

            ///<Summary>
            /// Parameter envelopeShareDate
            ///</Summary>
            public long envelopeSharedDate { get; set; }

            ///<Summary>
            /// Parameter envelopeStatus
            ///</Summary>
            public string envelopeStatus { get; set; }

            ///<Summary>
            /// Parameter envelopeContractIds
            ///</Summary>
            public List<int> envelopeContractIds { get; set; }

            ///<Summary>
            /// Parameter envelopePartyPermissions
            ///</Summary>
            public List<EnvelopePartyPermission> envelopePartyPermissions { get; set; }

            ///<Summary>
            /// Parameter envelopeAuthenticationLevel
            ///</Summary>
            public string envelopeAuthenticationLevel { get; set; }

            ///<Summary>
            /// Parameter allowSingleSignerInBulk
            ///</Summary>
            public bool allowSingleSignerInBulk { get; set; }
        }

        ///<Summary>
        /// SubClass Root
        ///</Summary>
        public class Root
        {
            ///<Summary>
            /// Parameter embeddedSigningSessions
            ///</Summary>
            public List<EmbeddedSigningSession> embeddedSigningSessions { get; set; }

            ///<Summary>
            /// Parameter folder
            ///</Summary>
            public Folder folder { get; set; }

            ///<Summary>
            /// Parameter result
            ///</Summary>
            public string result { get; set; }

            ///<Summary>
            /// Parameter message
            ///</Summary>
            public string message { get; set; }
        }
    }

    ///<Summary>
    /// Class template json
    ///</Summary>
    public class templateJson
    {
        ///<Summary>
        /// SubClass Field
        ///</Summary>
        public class Field
        {
            ///<Summary>
            /// Parameter type
            ///</Summary>
            public string type { get; set; }

            ///<Summary>
            /// Parameter x
            ///</Summary>
            public int x { get; set; }

            ///<Summary>
            /// Parameter y
            ///</Summary>
            public int y { get; set; }

            ///<Summary>
            /// Parameter width
            ///</Summary>
            public int width { get; set; }

            ///<Summary>
            /// Parameter height
            ///</Summary>
            public int height { get; set; }

            ///<Summary>
            /// Parameter pageNumber
            ///</Summary>
            public int pageNumber { get; set; }

            ///<Summary>
            /// Parameter tabOrder
            ///</Summary>
            public int tabOrder { get; set; }

            ///<Summary>
            /// Parameter party
            ///</Summary>
            public int party { get; set; }

            ///<Summary>
            /// Parameter name
            ///</Summary>
            public string name { get; set; }

            ///<Summary>
            /// Parameter tooltip
            ///</Summary>
            public string tooltip { get; set; }

            ///<Summary>
            /// Parameter value
            ///</Summary>
            public string value { get; set; }

            ///<Summary>
            /// Parameter required
            ///</Summary>
            public bool required { get; set; }

            ///<Summary>
            /// Parameter characterLimit
            ///</Summary>
            public int characterLimit { get; set; }

            ///<Summary>
            /// Parameter fontSize
            ///</Summary>
            public int fontSize { get; set; }

            ///<Summary>
            /// Parameter fontColor
            ///</Summary>
            public string fontColor { get; set; }

            ///<Summary>
            /// Parameter validation
            ///</Summary>
            public string validation { get; set; }

            ///<Summary>
            /// Parameter options
            ///</Summary>
            public List<string> options { get; set; }
        }

        ///<Summary>
        /// SubClass Root
        ///</Summary>
        public class Root
        {
            ///<Summary>
            /// Parameter templateUrl
            ///</Summary>
            public string templateUrl { get; set; }

            ///<Summary>
            /// Parameter templateName
            ///</Summary>
            public string templateName { get; set; }

            ///<Summary>
            /// Parameter processTextTags
            ///</Summary>
            public bool processTextTags { get; set; }

            ///<Summary>
            /// Parameter processAcroFields
            ///</Summary>
            public bool processAcroFields { get; set; }

            ///<Summary>
            /// Parameter numberOfParties
            ///</Summary>
            public int numberOfParties { get; set; }

            ///<Summary>
            /// Parameter fields
            ///</Summary>
            public List<Field> fields { get; set; }
        }
    }

    ///<Summary>
    /// Class create folder for E-sign
    ///</Summary>
    public class createFolderForEsign
    {
        ///<Summary>
        /// SubClass Party
        ///</Summary>
        public class Party
        {
            ///<Summary>
            /// Parameter firstName
            ///</Summary>
            public string firstName { get; set; }

            ///<Summary>
            /// Parameter lastName
            ///</Summary>
            public string lastName { get; set; }

            ///<Summary>
            /// Parameter emailId
            ///</Summary>
            public string emailId { get; set; }

            ///<Summary>
            /// Parameter permission
            ///</Summary>
            public string permission { get; set; }

            ///<Summary>
            /// Parameter sequence
            ///</Summary>
            public int sequence { get; set; }

            ///<Summary>
            /// Parameter dialingCode
            ///</Summary>
            public string dialingCode { get; set; }

            ///<Summary>
            /// Parameter mobileNumber
            ///</Summary>
            public string mobileNumber { get; set; }

            ///<Summary>
            /// Parameter signerAuthLevel
            ///</Summary>
            public string signerAuthLevel { get; set; }
        }

        ///<Summary>
        /// SubClass Field
        ///</Summary>
        public class Field
        {
            ///<Summary>
            /// Parameter type
            ///</Summary>
            public string type { get; set; }

            ///<Summary>
            /// Parameter x
            ///</Summary>
            public int x { get; set; }

            ///<Summary>
            /// Parameter y
            ///</Summary>
            public int y { get; set; }

            ///<Summary>
            /// Parameter width
            ///</Summary>
            public int width { get; set; }

            ///<Summary>
            /// Parameter height
            ///</Summary>
            public int height { get; set; }

            ///<Summary>
            /// Parameter documentNumber
            ///</Summary>
            public int documentNumber { get; set; }

            ///<Summary>
            /// Parameter pageNumber
            ///</Summary>
            public int pageNumber { get; set; }

            ///<Summary>
            /// Parameter tabOrder
            ///</Summary>
            public int tabOrder { get; set; }

            ///<Summary>
            /// Parameter party
            ///</Summary>
            public int party { get; set; }

            ///<Summary>
            /// Parameter name
            ///</Summary>
            public string name { get; set; }

            ///<Summary>
            /// Parameter tooltip
            ///</Summary>
            public string tooltip { get; set; }

            ///<Summary>
            /// Parameter value
            ///</Summary>
            public string value { get; set; }

            ///<Summary>
            /// Parameter required
            ///</Summary>
            public bool required { get; set; }

            ///<Summary>
            /// Parameter characterLimit
            ///</Summary>
            public int characterLimit { get; set; }

            ///<Summary>
            /// Parameter fontSize
            ///</Summary>
            public int fontSize { get; set; }

            ///<Summary>
            /// Parameter fontColor
            ///</Summary>
            public string fontColor { get; set; }

            ///<Summary>
            /// Parameter validation
            ///</Summary>
            public string validation { get; set; }

            ///<Summary>
            /// Parameter options
            ///</Summary>
            public List<string> options { get; set; }
        }

        ///<Summary>
        /// Parameter Root
        ///</Summary>
        public class Root
        {
            ///<Summary>
            /// Parameter folderName
            ///</Summary>
            public string folderName { get; set; }

            ///<Summary>
            /// Parameter fileUrls
            ///</Summary>
            public List<string> fileUrls { get; set; }

            ///<Summary>
            /// Parameter fileNames
            ///</Summary>
            public List<string> fileNames { get; set; }

            ///<Summary>
            /// Parameter processTextTags
            ///</Summary>
            public bool processTextTags { get; set; }

            ///<Summary>
            /// Parameter processAcroFields
            ///</Summary>
            public bool processAcroFields { get; set; }

            ///<Summary>
            /// Parameter signInSequence
            ///</Summary>
            public bool signInSequence { get; set; }

            ///<Summary>
            /// Parameter sendNow
            ///</Summary>
            public bool sendNow { get; set; }

            ///<Summary>
            /// Parameter createEmbeddedSendingSession
            ///</Summary>
            public bool createEmbeddedSendingSession { get; set; }

            ///<Summary>
            /// Parameter fixRecipientParties
            ///</Summary>
            public bool fixRecipientParties { get; set; }

            ///<Summary>
            /// Parameter fixDocuments
            ///</Summary>
            public bool fixDocuments { get; set; }

            ///<Summary>
            /// Parameter sendSuccessUrl
            ///</Summary>
            public string sendSuccessUrl { get; set; }

            ///<Summary>
            /// Parameter sendErrorUrl
            ///</Summary>
            public string sendErrorUrl { get; set; }

            ///<Summary>
            /// Parameter createEmbeddedSigningSession
            ///</Summary>
            public bool createEmbeddedSigningSession { get; set; }

            ///<Summary>
            /// Parameter createEmbeddedSigningSessionForAllParties
            ///</Summary>
            public bool createEmbeddedSigningSessionForAllParties { get; set; }

            ///<Summary>
            /// Parameter signSuccessUrl
            ///</Summary>
            public string signSuccessUrl { get; set; }

            ///<Summary>
            /// Parameter signDeclineUrl
            ///</Summary>
            public string signDeclineUrl { get; set; }

            ///<Summary>
            /// Parameter signLaterUrl
            ///</Summary>
            public string signLaterUrl { get; set; }

            ///<Summary>
            /// Parameter signErrorUrl
            ///</Summary>
            public string signErrorUrl { get; set; }

            ///<Summary>
            /// Parameter themeColor
            ///</Summary>
            public string themeColor { get; set; }

            ///<Summary>
            /// Parameter allowSendNowAndEmbeddedSigningSession
            ///</Summary>
            public bool allowSendNowAndEmbeddedSigningSession { get; set; }

            ///<Summary>
            /// Parameter parties
            ///</Summary>  
            public List<Party> parties { get; set; }

            ///<Summary>
            /// Parameter fields
            ///</Summary>
            public List<Field> fields { get; set; }
        }
    }

    ///<Summary>
    /// Class response data
    ///</Summary>
    public class responseData
    {
        ///<Summary>
        /// Parameter message
        ///</Summary>
        public string message { get; set; }

        ///<Summary>
        /// Parameter Name
        ///</Summary>
        public string Name { get; set; }

        ///<Summary>
        /// Parameter status    Code
        ///</Summary>
        public int statusCode { get; set; }

        ///<Summary>
        /// Parameter totalRecord
        ///</Summary>
        public int totalRecord { get; set; }

        ///<Summary>
        /// Parameter updatedRecord
        ///</Summary>
        public int updatedRecord { get; set; }

        ///<Summary>
        /// Parameter faildRecord
        ///</Summary>
        public int faildRecord { get; set; }

        ///<Summary>
        /// Parameter addedRecord
        ///</Summary>
        public int addedRecord { get; set; }
    }

    ///<Summary>
    /// Class file path
    ///</Summary>
    public class FilePath
    {
        ///<Summary>
        /// Parameter path
        ///</Summary>
        public string path { get; set; }
    }

    ///<Summary>
    /// Class enterprise create
    ///</Summary>
    public class enterpriseCreate
    {
        ///<Summary>
        /// Parameter EnterpriseName
        ///</Summary>
        public string EnterpriseName { get; set; }

        ///<Summary>
        /// Parameter EnterpriseExt
        ///</Summary>
        public string EnterpriseExt { get; set; }

        ///<Summary>
        /// Parameter EnterpriseExtFlag
        ///</Summary>
        public string EnterpriseExtFlag { get; set; }

        ///<Summary>
        /// Parameter Phone
        ///</Summary>
        public string Phone { get; set; }

        ///<Summary>
        /// Parameter PrimaryContact
        ///</Summary>
        public string PrimaryContact { get; set; }

        ///<Summary>
        /// Parameter PrimaryContactCellNo
        ///</Summary>
        public string PrimaryContactCellNo { get; set; }

        ///<Summary>
        /// Parameter AlternateEnterpriseName
        ///</Summary>
        public string AlternateEnterpriseName { get; set; }

        ///<Summary>
        /// Parameter BrokerName
        ///</Summary>
        public string BrokerName { get; set; }

        ///<Summary>
        /// Parameter CorporateName
        ///</Summary>
        public string CorporateName { get; set; }

        ///<Summary>
        /// Parameter OfficePhone
        ///</Summary>
        public string OfficePhone { get; set; }

        ///<Summary>
        /// Parameter OfficeFax
        ///</Summary>
        public string OfficeFax { get; set; }

        ///<Summary>
        /// Parameter SecondaryEnterpriseEmail
        ///</Summary>
        public string SecondaryEnterpriseEmail { get; set; }

        ///<Summary>
        /// Parameter DisclosureVerbage
        ///</Summary>
        public string DisclosureVerbage { get; set; }

        ///<Summary>
        /// Parameter LicenseVerbiage
        ///</Summary>
        public string LicenseVerbiage { get; set; }

        ///<Summary>
        /// Parameter DefaultRealtor
        ///</Summary>
        public string DefaultRealtor { get; set; }

        ///<Summary>
        /// Parameter DefaultMbroker
        ///</Summary>
        public string DefaultMbroker { get; set; }

        ///<Summary>
        /// Parameter DefaultTitle
        ///</Summary>
        public string DefaultTitle { get; set; }

        ///<Summary>
        /// Parameter DefaultRefi
        ///</Summary>
        public string DefaultRefi { get; set; }

        ///<Summary>
        /// Parameter TaxPayerID
        ///</Summary>
        public string TaxPayerID { get; set; }

        ///<Summary>
        /// Parameter LicenesNo
        ///</Summary>
        public string LicenesNo { get; set; }

        ///<Summary>
        /// Parameter Subcompany
        ///</Summary>
        public string Subcompany { get; set; }

        ///<Summary>
        /// Parameter Logo
        ///</Summary>
        public string Logo { get; set; }
    }

    ///<Summary>
    /// Class Myholder
    ///</Summary>
    public class MyHolder
    {
        ///<Summary>
        /// Parameter A
        ///</Summary>
        public string A;

        ///<Summary>
        /// Function MyHolder(parameter a)
        ///</Summary>
        public MyHolder(string a)
        {
            A = a;
        }
    }
    ///<Summary>
    /// Rename Filename
    ///</Summary>
    public class Filenames
    {
        public string filenameold { get; set; }
        public string filenamenew { get; set; }
    }


    public class EmbeddedZohoResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public string sign_url { get; set; }
        public string status { get; set; }
    }

    public class RefreshTokenResponse
    {
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }
        public DateTime? AccessTokenTime { get; set; }


    }
    public class ZohoSigninUserMapping
    {
        public string recipientEmail { get; set; }
        public string recipientName { get; set; }
        public int signingOrder { get; set; }
        public bool isReplaced { get; set; }

    }
    public class UserTypeList
    {
        public string userType { get; set; }
        public int signingOrder { get; set; }
        public string sign_type { get; set; }
        public string searchPattren { get; set; }
        public bool isReplaced { get; set; }

    }

    public class UserEmail
    {
        public string EmailAddress { get; set; }
    }// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Action
    {
        public bool verify_recipient { get; set; }
        public string recipient_countrycode_iso { get; set; }
        public string action_type { get; set; }
        public string private_notes { get; set; }
        public string cloud_provider_name { get; set; }
        public bool has_payment { get; set; }
        public string recipient_email { get; set; }
        public bool send_completed_document { get; set; }
        public string verification_type { get; set; }
        public bool allow_signing { get; set; }
        public string recipient_phonenumber { get; set; }
        public bool is_bulk { get; set; }
        public string action_id { get; set; }
        public bool is_revoked { get; set; }
        public bool is_embedded { get; set; }
        public int cloud_provider_id { get; set; }
        public int signing_order { get; set; }
        public List<Field> fields { get; set; }
        public string recipient_name { get; set; }
        public string delivery_mode { get; set; }
        public string action_status { get; set; }
        public string recipient_countrycode { get; set; }
    }

    public class DocumentField
    {
        public string document_id { get; set; }
        public List<object> fields { get; set; }
    }

    public class DocumentId
    {
        public string image_string { get; set; }
        public string document_name { get; set; }
        public List<Page> pages { get; set; }
        public int document_size { get; set; }
        public string document_order { get; set; }
        public bool is_editable { get; set; }
        public int total_pages { get; set; }
        public string document_id { get; set; }
    }

    public class Field
    {
        public string field_id { get; set; }
        public int x_coord { get; set; }
        public string field_type_id { get; set; }
        public int abs_height { get; set; }
        public string field_category { get; set; }
        public string field_label { get; set; }
        public bool is_mandatory { get; set; }
        public int page_no { get; set; }
        public string document_id { get; set; }
        public bool is_draggable { get; set; }
        public string field_name { get; set; }
        public double y_value { get; set; }
        public int abs_width { get; set; }
        public string action_id { get; set; }
        public double width { get; set; }
        public int y_coord { get; set; }
        public string field_type_name { get; set; }
        public string description_tooltip { get; set; }
        public double x_value { get; set; }
        public bool is_resizable { get; set; }
        public double height { get; set; }
    }

    public class Page
    {
        public string image_string { get; set; }
        public int page { get; set; }
        public bool is_thumbnail { get; set; }
    }

    public class Requestss
    {
        public string request_status { get; set; }
        public string notes { get; set; }
        public List<object> attachments { get; set; }
        public int reminder_period { get; set; }
        public string owner_id { get; set; }
        public string description { get; set; }
        public string request_name { get; set; }
        public long modified_time { get; set; }
        public long action_time { get; set; }
        public bool is_deleted { get; set; }
        public int expiration_days { get; set; }
        public bool is_sequential { get; set; }
        public long sign_submitted_time { get; set; }
        public List<object> templates_used { get; set; }
        public string owner_first_name { get; set; }
        public double sign_percentage { get; set; }
        public long expire_by { get; set; }
        public bool is_expiring { get; set; }
        public string owner_email { get; set; }
        public long created_time { get; set; }
        public bool email_reminders { get; set; }
        public List<DocumentId> document_ids { get; set; }
        public bool self_sign { get; set; }
        public List<DocumentField> document_fields { get; set; }
        public string folder_name { get; set; }
        public bool in_process { get; set; }
        public int validity { get; set; }
        public string request_type_name { get; set; }
        public VisibleSignSettings visible_sign_settings { get; set; }
        public string folder_id { get; set; }
        public string request_id { get; set; }
        public string zsdocumentid { get; set; }
        public string request_type_id { get; set; }
        public string owner_last_name { get; set; }
        public List<Action> actions { get; set; }
        public int attachment_size { get; set; }
    }

    public class Roots
    {
        public int code { get; set; }
        public Requestss requestss { get; set; }
        public string message { get; set; }
        public string status { get; set; }

    }

    public class VisibleSignSettings
    {
        public bool visible_sign { get; set; }
        public bool allow_reason_visible_sign { get; set; }
    }


    public class FileUploadResponse
    {
        public string Url { get; set; }
        public string OutputLinkValidTill { get; set; }
        public bool Error { get; set; }
        public int Status { get; set; }
        public string Name { get; set; }
        public int Credits { get; set; }
        public int RemainingCredits { get; set; }
        public int Duration { get; set; }
        public string jobId { get; set; }
    }

    public class JobStatusResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public int pageCount { get; set; }
        public string url { get; set; }
        public DateTime outputLinkValidTill { get; set; }
        public string jobId { get; set; }
        public int credits { get; set; }
        public int remainingCredits { get; set; }
        public int jobDuration { get; set; }
        public int duration { get; set; }
    }





    public class Body
    {
        public string text { get; set; }
        public double left { get; set; }
        public double top { get; set; }
        public double width { get; set; }
        public double height { get; set; }
        public int pageIndex { get; set; }
        public Bounds bounds { get; set; }
        public int elementCount { get; set; }
        public List<Element> elements { get; set; }
    }

    public class Bounds
    {
        public Location location { get; set; }
        public string size { get; set; }
        public double x { get; set; }
        public double y { get; set; }
        public double width { get; set; }
        public double height { get; set; }
        public double left { get; set; }
        public double top { get; set; }
        public double right { get; set; }
        public double bottom { get; set; }
        public bool isEmpty { get; set; }
    }

    public class Element
    {
        public int index { get; set; }
        public double left { get; set; }
        public double top { get; set; }
        public double width { get; set; }
        public double height { get; set; }
        public double angle { get; set; }
        public string text { get; set; }
        public bool isNewLine { get; set; }
        public bool fontIsBold { get; set; }
        public bool fontIsItalic { get; set; }
        public string fontName { get; set; }
        public int fontSize { get; set; }
        public string fontColor { get; set; }
        public int fontColorAsOleColor { get; set; }
        public string fontColorAsHtmlColor { get; set; }
        public Bounds bounds { get; set; }
    }

    public class Location
    {
        public bool isEmpty { get; set; }
        public double x { get; set; }
        public double y { get; set; }
    }

    public class PdfTextPositins
    {
        public List<Body> body { get; set; }
        public int pageCount { get; set; }
        public bool error { get; set; }
        public int status { get; set; }
        public string name { get; set; }
        public int credits { get; set; }
        public int remainingCredits { get; set; }
        public int duration { get; set; }
    }

    //replaceWord, DeleteSuccessFully.Url, pdfTextPositins.body.FirstOrDefault().left, pdfTextPositins.body.FirstOrDefault().top, fileName

    public class AddTextToReplaceDto
    {
        public string replaceWord { get; set; }
        public double left { get; set; }
        public double top { get; set; }
        public string Url { get; set; }
    }


    public class ConvertResponse
    {
        public bool success { get; set; }
        public string url { get; set; }
        public string error { get; set; }
    }

}

