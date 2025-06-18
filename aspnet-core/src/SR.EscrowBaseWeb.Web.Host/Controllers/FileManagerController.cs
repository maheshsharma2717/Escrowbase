using Abp;
using Abp.Domain.Repositories;
using Abp.Notifications;
using DevExtreme.AspNet.Mvc.FileManagement;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using MimeKit;
using MySqlConnector;
using Newtonsoft.Json;
using Spire.Doc;
using SR.EscrowBaseWeb.Authorization.Users;
using SR.EscrowBaseWeb.Authorization.Users.Dto;
using SR.EscrowBaseWeb.E_SignRecords;
using SR.EscrowBaseWeb.EscrowDetails;
using SR.EscrowBaseWeb.EscrowFileMaster;
using SR.EscrowBaseWeb.EscrowFileTag;
using SR.EscrowBaseWeb.SrAssignedFilesDetails;
using SR.EscrowBaseWeb.SREnterprise;
using SR.EscrowBaseWeb.SREscrowFileHistory;
using SR.EscrowBaseWeb.SREscrowFileHistory.Dtos;
using SR.EscrowBaseWeb.SrEscrows;
using SR.EscrowBaseWeb.SRFileMapping;
using SR.EscrowBaseWeb.SRFileMapping.Dtos;
using SR.EscrowBaseWeb.SrInvitationRecords;
using SR.EscrowBaseWeb.SrInvitationRecords.Dtos;
using SR.EscrowBaseWeb.TagsAndFileMapping;
using SR.EscrowBaseWeb.Web.Models;
using SR.EscrowBaseWeb.Web.Models.ZohoESign;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
//using NPOI.HWPF;
//using FreeSpire.Doc;
//using NPOI.HWPF.UserModel;

namespace SR.EscrowBaseWeb.Web.Controllers
{
    ///<Summary>
    /// FileManager controller
    ///</Summary>
    ///
    public class FileManagerController : EscrowBaseWebControllerBase
    {
        ///<Summary>
        /// Get IAbpSession
        ///</Summary>
        ///

        private readonly UserAppService _IUserAppService;
        private readonly IRepository<SrFileMapping> _srfilemapRepository;
        private readonly IRepository<Enterprise> _enterpriseRepository;
        private readonly IRepository<SrInvitationRecord, long> _srinvitationrecordRepository;
        private readonly SrInvitationRecordsAppService _srInvitationRecordsAppService;
        private readonly SrFileMappingsAppService _srFileMappingsAppService;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<E_SignRecord, long> _esignRepository;
        private readonly E_SignRecordsAppService _e_SignRecordsAppService;
        private readonly IRepository<EscrowDetail, long> _escrowDetailRepository;
        private readonly IRepository<SrEscrow> _ISrEscrowRepository;
        private readonly IRepository<SrAssignedFilesDetail, long> _srAssignedFilesDetailRepository;
        private readonly SrAssignedFilesDetailsAppService _srAssignedFilesDetailsAppService;
        private readonly IEscrowFileHistoriesAppService _escrowFileHistoriesAppService;
        static readonly string SampleImagesRelativePath = Path.Combine(@"Common", "Paperless");
        IWebHostEnvironment _hostingEnvironment;
        private readonly IRepository<EscrowFileHistory, long> _escrowFileHistoryRepository;
        private readonly IRepository<SREscrowFileMaster, long> _srEscrowFileMasterRepository;
        private readonly IRepository<EscrowFileTags> _escrowFileTagsRepository;
        private readonly IRepository<TagsAndFileMappings> _tagsAndFileMappingsRepository;


        ///<Summary>
        /// Get UrlReferrer
        ///</Summary>
        public Uri UrlReferrer { get; }

        ///<Summary>
        /// Static parameter approve
        ///</Summary>
        public static string approve = "approve";

        ///<Summary>
        /// Get ISrFileMappingsAppService
        ///</Summary>
        public readonly ISrFileMappingsAppService _ISrFileMappingsAppService;
        private readonly INotificationPublisher _notificationPublisher;
        static IConfiguration conf = (new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build());

        Regex regexx = new Regex(@"\{.*?\}");
        ///<Summary>
        /// Overview
        ///</Summary>
        public IActionResult Overview()
        {
            return View();
        }

        ///<Summary>
        /// Get all required data for FileManager controller
        ///</Summary>
        public FileManagerController(IWebHostEnvironment hostingEnvironment,
            SrInvitationRecordsAppService srInvitationRecordsAppService,
               UserAppService IUserAppService,
            ISrFileMappingsAppService ISrFileMappingsAppService,
            IRepository<Enterprise> enterpriseRepository,
            IRepository<SrFileMapping> srfilemapRepository,
            SrFileMappingsAppService srFileMappingsAppService,
            IRepository<SrInvitationRecord, long> srInvitationRecord,
            IRepository<User, long> userRepository,
            IRepository<E_SignRecord, long> esignRepository,
            E_SignRecordsAppService e_SignRecordsAppService,
            IRepository<EscrowDetail, long> escrowDetailRepository,
            IRepository<SrEscrow> ISrEscrowRepository,
            IRepository<SrAssignedFilesDetail, long> srAssignedFilesDetailRepository,
            SrAssignedFilesDetailsAppService srAssignedFilesDetailsAppService,
            INotificationPublisher notificationPublisher,
            IEscrowFileHistoriesAppService escrowFileHistoriesAppService,
            IRepository<EscrowFileHistory, long> escrowFileHistoryRepository,
            IRepository<SREscrowFileMaster, long> srEscrowFileMasterRepository,
            IRepository<EscrowFileTags> escrowFileTagsRepository,
            IRepository<TagsAndFileMappings> tagsAndFileMappingsRepository
            )
        {
            _ISrFileMappingsAppService = ISrFileMappingsAppService;
            _enterpriseRepository = enterpriseRepository;
            _srfilemapRepository = srfilemapRepository;
            _hostingEnvironment = hostingEnvironment;
            _srFileMappingsAppService = srFileMappingsAppService;
            _srinvitationrecordRepository = srInvitationRecord;
            _userRepository = userRepository;
            _srInvitationRecordsAppService = srInvitationRecordsAppService;
            _esignRepository = esignRepository;
            _e_SignRecordsAppService = e_SignRecordsAppService;
            _escrowDetailRepository = escrowDetailRepository;
            _ISrEscrowRepository = ISrEscrowRepository;
            _notificationPublisher = notificationPublisher;
            _srAssignedFilesDetailRepository = srAssignedFilesDetailRepository;
            _srAssignedFilesDetailsAppService = srAssignedFilesDetailsAppService;
            _escrowFileHistoriesAppService = escrowFileHistoriesAppService;
            _escrowFileHistoryRepository = escrowFileHistoryRepository;
            _srEscrowFileMasterRepository = srEscrowFileMasterRepository;
            _escrowFileTagsRepository = escrowFileTagsRepository;
            _tagsAndFileMappingsRepository = tagsAndFileMappingsRepository;
        }

        ///<Summary>
        /// Get HostingEnvironment
        ///</Summary>
        public IWebHostEnvironment HostingEnvironment { get; }

        ///<Summary>
        /// Download files
        ///</Summary>
        public async Task<IActionResult> DownloadFile(string path, string key, long srAssignedFileId, long userId)
        {
            try
            {
                path = path.Replace("%23", "#");
                key = key.Replace("%23", "#");
                var folderName = Path.Combine(@"Common/Paperless/" + path);
                folderName = folderName.Substring(0, folderName.LastIndexOf('/'));
                folderName = Path.Combine(folderName + "/" + key);
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);


                WebClient webClient = new WebClient();
                string newpath = folderName.Replace("/", "\\");
                string file = Path.Combine(_hostingEnvironment.WebRootPath + "\\" + newpath);
                var memory = new MemoryStream();
                using (var stream = new FileStream(file, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                var ext = Path.GetExtension(file).ToLowerInvariant();
                CreateOrEditEscrowFileHistoryDto escrowFileHistory = new CreateOrEditEscrowFileHistoryDto();
                escrowFileHistory.SrEscrowFileMasterId = srAssignedFileId;
                string fileName = Path.GetFileName(pathToSave);
                escrowFileHistory.FileFullPath = fileName;
                escrowFileHistory.UserId = userId;//await _escrowFileHistoriesAppService.GetUserIdFromSession() != null ? long.Parse(_escrowFileHistoriesAppService.GetUserIdFromSession().ToString()) : 0;
                escrowFileHistory.Message = FileConstant.Download_File;
                escrowFileHistory.ActionType = FileConstantAction.Download_File;
                await _escrowFileHistoriesAppService.CreateOrEdit(escrowFileHistory);
                var mimeType = GetMimeTypes().GetValueOrDefault(ext, "application/octet-stream");
                return File(memory, mimeType, Path.GetFileName(file));
            }
            catch (Exception ex)
            {
                string logs = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");
                if (!System.IO.File.Exists(logs))
                {
                    FileStream fs1 = new FileStream(logs, FileMode.OpenOrCreate, FileAccess.Write);
                }
                StreamWriter writer = new StreamWriter(logs, true);
                writer.WriteLine("Error in DownloadFile method for -: error=" + ex.ToString() + DateTime.Now.ToString());
                writer.Close();
                return NotFound();
            }
        }

        public async Task<IActionResult> ConvertFileToBase64(string path, string key)
        {
            try
            {
                path = path.Replace("%23", "#");
                key = key.Replace("%23", "#");
                var folderName = Path.Combine(@"Common/Paperless/" + path);
                folderName = folderName.Substring(0, folderName.LastIndexOf('/'));
                folderName = Path.Combine(folderName + "/" + key);
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                WebClient webClient = new WebClient();
                string newpath = folderName.Replace("/", "\\");
                string file = Path.Combine(_hostingEnvironment.WebRootPath + "\\" + newpath);

                if (Path.GetExtension(file).ToLower() == ".doc")
                {
                    try
                    {
                        if (!System.IO.File.Exists(file))
                        {
                            return NotFound(new { error = "File not found" });
                        }
                        Document document = new Document();
                        document.LoadFromFile(file);
                        string textContent = document.GetText();
                        string evaluationWarning = "Evaluation Warning: The document was created with Spire.Doc for .NET.";
                        textContent = textContent.Replace(evaluationWarning, string.Empty).Trim();
                        //  Convert the text content to Base64
                        byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(textContent);
                        string base64String = Convert.ToBase64String(textBytes);
                        return Ok(new { Base64 = base64String, fileType = "doc" });
                    }
                    catch (Exception ex)
                    {
                        // Log the error (or return it for debugging purposes)
                        //Console.WriteLine($"Error: {ex.Message}");
                        //return StatusCode(500, new { error = ex.Message });
                        string logs = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");
                        if (!System.IO.File.Exists(logs))
                        {
                            FileStream fs1 = new FileStream(logs, FileMode.OpenOrCreate, FileAccess.Write);
                        }
                        StreamWriter writer = new StreamWriter(logs, true);
                        writer.WriteLine("Error in ConvertFileToBase64 method for -: error=" + ex.ToString() + " " + DateTime.Now.ToString());
                        writer.Close();
                        return NotFound();
                    }
                }

                if (Path.GetExtension(file).Equals(".eml", StringComparison.OrdinalIgnoreCase))
                {
                    using var stream = new FileStream(file, FileMode.Open, FileAccess.Read);
                    var message = await MimeMessage.LoadAsync(stream);

                    // Get the HTML or plain text body
                    string emailContent = message.HtmlBody ?? message.TextBody;

                    // Return the content
                    return Ok(new { Base64 = emailContent, fileType = "eml" });
                }

                else if (Path.GetExtension(file).ToLower() == ".rtf")
                {
                    // Handle .rtf files (you can use Aspose.Words or other libraries)
                    // Assuming you have an existing method for this
                    var rtfContent = await System.IO.File.ReadAllTextAsync(file);
                    byte[] rtfBytes = System.Text.Encoding.UTF8.GetBytes(rtfContent);
                    string base64String = Convert.ToBase64String(rtfBytes);
                    return Ok(new { Base64 = base64String, fileType = "rtf" });
                }

                if (Path.GetExtension(file).ToLower() == ".pdf")
                {
                    // Handle .pdf files (assuming they are already working)
                    using var pdfStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                    using var memory = new MemoryStream();
                    await pdfStream.CopyToAsync(memory);
                    string base64String = Convert.ToBase64String(memory.ToArray());
                    return Ok(new { Base64 = base64String, fileType = "pdf" });
                }
                if (Path.GetExtension(file).ToLower() == ".docx")
                {
                    // Handle .docx files (already working)
                    using var docxStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                    using var memory = new MemoryStream();
                    await docxStream.CopyToAsync(memory);
                    string base64String = Convert.ToBase64String(memory.ToArray());
                    return Ok(new { Base64 = base64String, fileType = "docx" });
                }

                else if (Path.GetExtension(file).ToLower() == ".txt")
                {
                    // Handle .txt files
                    var textContent = await System.IO.File.ReadAllTextAsync(file);
                    byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(textContent);
                    string base64String = Convert.ToBase64String(textBytes);
                    return Ok(new { Base64 = base64String, fileType = "docx" });
                }


                else if (Path.GetExtension(file).ToLower() == ".msg")
                {                 
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    using var fileStream = System.IO.File.OpenRead(file);
                    var reader = new MsgReader.Outlook.Storage.Message(fileStream);

                    // Prefer HTML body for links and images
                    string emailContent = reader.BodyHtml ?? reader.BodyText ?? "No content available";
                    byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(emailContent);
                    string base64String = Convert.ToBase64String(textBytes);
                    return Ok(new { Base64 = base64String, fileType = "msg" });
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
                writer.WriteLine("Error in ConvertFileToBase64 method for -: error=" + ex.ToString() + " " + DateTime.Now.ToString());
                writer.Close();
                return NotFound();
            }
            return NotFound();
        }
        private string ConvertHtmlToRtf(string html)
        {
            StringBuilder rtfBuilder = new StringBuilder();
            rtfBuilder.Append(@"{\rtf1\ansi\ansicpg1252\deff0\nouicompat{\fonttbl{\f0\fnil\fcharset0 Calibri;}}
    {\*\generator Riched20 10.0.18362;}viewkind4\uc1 \pard ");

            // Simple replacements for newlines and HTML entities
            html = html.Replace("\n", "\\par ");
            html = html.Replace("&nbsp;", " "); // Convert HTML entities as needed
                                                // Add more conversions as necessary...

            rtfBuilder.Append(html);
            rtfBuilder.Append(@"\par }");
            return rtfBuilder.ToString();
        }



        private string ExtractTextFromHtml(string html)
        {
            // Load HTML into HtmlDocument
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            // Extract text using inner text of the root node
            return htmlDoc.DocumentNode.InnerText;
        }

         
        public async Task<IActionResult> SaveDocument([FromBody] SaveDocumentRequest request)
        {
            if (string.IsNullOrEmpty(request.Base64Content))
                return BadRequest("File content is empty");

            // Get the file name from the path
            string fileName = Path.GetFileName(request.FilePath);

            // Build the directory path from the file path
            var folderName = Path.Combine("wwwroot", "Common", "Paperless", Path.GetDirectoryName(request.FilePath));
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            // Ensure the directory exists
            if (!Directory.Exists(pathToSave))
                Directory.CreateDirectory(pathToSave);

            // Full path for the new file
            var filePath = Path.Combine(pathToSave, fileName);

            // Remove the old file if it exists
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            byte[] fileBytes = Convert.FromBase64String(request.Base64Content);

            await System.IO.File.WriteAllBytesAsync(filePath, fileBytes);

            return Ok(new { filePath });
        }

        public class SaveDocumentRequest
        {
            public string FileName { get; set; }
            public string Base64Content { get; set; }
            public string FilePath { get; set; }
        }

        ///<Summary>
        /// Delete files
        ///</Summary>
        public responseBack DeleteFile(string path, string key)
        {
            try
            {
                responseBack res = new responseBack();
                path = path.Replace("%23", "#");
                key = key.Replace("%23", "#");
                var folderName = Path.Combine(@"Common/Paperless/" + path);
                folderName = folderName.Substring(0, folderName.LastIndexOf('/'));
                folderName = Path.Combine(folderName + "/" + key);
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                var httpConnectionFeatures = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
                //var localAddress = httpConnectionFeatures?.LocalIpAddress;
                WebClient webClient = new WebClient();
                string newpath = folderName.Replace("/", "\\");
                string file = Path.Combine(_hostingEnvironment.WebRootPath + "\\" + newpath);
                if (System.IO.File.Exists(file))
                {
                    var filed = _srfilemapRepository.GetAll().Where(x => x.FileName == file).ToList();
                    if (filed != null)
                    {
                        foreach (var id in filed)
                        {
                            _srfilemapRepository.Delete(id);
                        }
                        System.IO.File.Delete(file);

                    }
                    // Deleting from EsignMapping table files

                    var esignFile = _esignRepository.GetAll().Where(x => file.Contains(x.FullFilePath)).ToList();
                    if (esignFile.Count > 0)
                    {
                        foreach (var item in esignFile)
                        {
                            _esignRepository.Delete(item);
                        }
                    }

                    res.message = "File deleted successfully";
                }
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
                writer.WriteLine("Error in DeleteFile method for -: error=" + ex.ToString() + DateTime.Now.ToString());
                writer.Close();
                return null;
            }
        }



        public static string ValidFileName(string Vfn)
        {
            // First replace invalid Charcter symbols for file names with normal ascii set with similar acceptable charcters.
            Vfn = Strings.Replace(Vfn, "<", "(");
            Vfn = Strings.Replace(Vfn, ">", ")");
            Vfn = Strings.Replace(Vfn, ":", ";");
            Vfn = Strings.Replace(Vfn, "*", "'");
            // Vfn = Strings.Replace(Vfn, "/", "-");
            Vfn = Strings.Replace(Vfn, @"\", "=");
            Vfn = Strings.Replace(Vfn, "|", "_");
            Vfn = Strings.Replace(Vfn, "?", "+");
            Vfn = Strings.Replace(Vfn, "*", ".");
            // Vfn = Strings.Replace(Vfn, ".", "DOT");
            char str = Vfn[Vfn.Length - 1];
            if (str.ToString() == ".")
            {
                Vfn = Vfn.Substring(0, Vfn.LastIndexOf('.'));
            }
            //int length = Vfn.Length - Vfn.IndexOf("1") - 1;

            //string sub = Vfn.Substring(0, length);

            return Vfn;
        }



        ///<Summary>
        /// Get file in memory stream
        ///</Summary>
        public async Task<responseBack> Edit(string path, long srAssignedFileId)
        {
            try
            {
                var file = Request.Form.Files[0];
                responseBack res = new responseBack();
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                fileName = fileName.Replace("%23", "#");
                fileName = fileName.Substring(0, fileName.IndexOf('?'));
                var pt = path.Substring(path.IndexOf("/Paperless/"));
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Common" + pt + "/" + fileName);
                //var fullPath = "C:\\Host\\serverhost\\wwwroot\\Common\\Paperless\\Escrow nInc\\Escrow nInc\\07-6549\\My File Test Email.-#10222.ZAS_~'~{SR1-READS}{BR1-READS}.doc05-28-2021(Mary Jones) (5) (1).pdf";
                //var fileName = "My File Test Email.-#10222.ZAS_~'~{SR1-READS}{BR1-READS}.doc05-28-2021(Mary Jones) (5) (1).pdf";
                if (System.IO.File.Exists(fullPath))
                {
                    var memoryStream = new MemoryStream();
                    await file.CopyToAsync(memoryStream);
                    var Content = memoryStream.ToArray();
                    System.IO.File.WriteAllBytes(fullPath, Content);
                    fullPath = ValidFileName(fullPath);
                    var full = fullPath.Replace("=", "\\").Replace(".\\", "\\").Replace(";", ":").Replace("/", "\\");
                    string[] subs = full.Split('\\');
                    var ids = $"{subs[8]}";
                    var usrid = _escrowDetailRepository.GetAll().Where(x => x.EscrowId == ids).ToList();
                    fullPath = ValidFileName(fullPath);
                    var full1 = fullPath.Replace("=", "\\").Replace(".\\", "\\").Replace(";", ":").Replace("/", "\\");
                    var fileInfo1 = _srfilemapRepository.GetAll().Where(x => x.FileName == full1).FirstOrDefault();
                    var EscrowIds = fileInfo1.EscrowiId;
                    var detail = _ISrEscrowRepository.GetAll().Where(x => x.EscrowNo == EscrowIds).FirstOrDefault();
                    var emailing = detail.EOEmail;
                    EscrowDetail item = new EscrowDetail();
                    item.Email = emailing;
                    usrid.Add(item);
                    foreach (var usr in usrid)
                    {

                        string match = String.Empty;
                        MatchCollection matches = regexx.Matches(fileName);
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
                            var dbUser = _userRepository.GetAll().Where(x => x.EmailAddress == usr.Email).FirstOrDefault();
                            if (dbUser != null)
                            {
                                if (dbUser.IsEmailConfirmed == true)
                                {
                                    isInviteAccepted = true;
                                }
                                else
                                {
                                    isInviteAccepted = false;

                                }
                            }

                            if ((match.Contains("A") && isInviteAccepted == true && (rep.Contains(usr.Usertype)) || usr.Usertype == ""))
                            {

                                MailMessage mail = new MailMessage();
                                mail.From = new MailAddress("Noreply@EscrowBasePortal.com");
                                mail.To.Add(usr.Email);
                                mail.Subject = "File Edit";
                                string referer = conf["App:ClientRootAddress"].ToString();

                                var escrow = _escrowDetailRepository.GetAll().Where(x => x.EscrowId == EscrowIds).FirstOrDefault();
                                var enterprises = _enterpriseRepository.GetAll().Where(x => x.EnterpriseName == escrow.Company).FirstOrDefault();
                                var Company = enterprises.EnterpriseName;
                                var userName = escrow.Name;
                                var logo = enterprises.Logo;
                                if (logo == "" || logo == null) { logo = "https://ayushkamiya.com/Escrow-logo.png"; }
                                var Message = "A doc has been edited on the secure web portal regarding your escrow " + EscrowIds + " that needs your attention. Please log in to review, this reminder was sent by escrowbaseweb powered by software reality. File Name is " + "'" + fileName + "'";
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
                                        cmd.Parameters.AddWithValue("@functionName", "Edit");

                                        var dr = cmd.ExecuteReader();
                                        var dataTable = new DataTable();
                                        dataTable.Load(dr);
                                        string JSONString = string.Empty;
                                        JSONString = JsonConvert.SerializeObject(dataTable);
                                        result = JsonConvert.DeserializeObject<List<ExpandoObject>>(JSONString);
                                    }
                                }

                                if (result.Count > 0)
                                {
                                    string pno = "";
                                    string phoneno = "7148121408";
                                    //var user = _userRepository.GetAll().Where(x => x.Id == usr.UserId).FirstOrDefault();
                                    //string phoneno = user.PhoneNumber;
                                    //if(phoneno != null) { }
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
                                    messageOptions.Body = "A doc has been edited on the secure web portal regarding your escrow " + EscrowIds + " that needs your attention. Please log in to review, this reminder was sent by escrowbaseweb powered by software reality. File Name is " + "'" + fileName + "'";
                                    messageOptions.From = SenderNumber;
                                    User.PhoneNumber = pno;
                                    GetUsersInput input = new GetUsersInput();
                                    //input.Filter = un;
                                    input.MaxResultCount = 1000;

                                    var messages = MessageResource.Create(messageOptions);
                                    //res = message.Status.ToString();



                                }
                                else {
                                }
                            }
                        }
                    }


                    CreateOrEditEscrowFileHistoryDto escrowFileHistory = new CreateOrEditEscrowFileHistoryDto();
                    escrowFileHistory.SrEscrowFileMasterId = srAssignedFileId;
                    escrowFileHistory.FileFullPath = path;
                    escrowFileHistory.UserId = AbpSession.UserId;
                    escrowFileHistory.Message = FileConstant.Edit_File;
                    escrowFileHistory.ActionType = FileConstantAction.Edit_File;
                    await _escrowFileHistoriesAppService.CreateOrEdit(escrowFileHistory);



                    // }
                    //else
                    //{
                    //  //  res.message = "Something went wrong";
                    //}
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }




        ///<Summary>
        /// Get mime for files
        ///</Summary>
        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
            {".pdf","application/pdf"},
            {".txt","application/text"},
            {".msg","application/vnd.ms-outlook"},
            {".png","image/png"},
            {".jpeg","image/jpeg"},
            {".rar","application/vnd.rar"},
            {".jpg","image/jpeg"},
                {".xls","application/vnd.ms-excel" },
            {".svg","image/svg+xml"},
            {".tif","image/tif"},
            {".tiff","image/tiff"},
            {".bmp","image/bmp"},
            {".gif","image/gif"},
            {".doc","application/msword" },
            {".docx","application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
                {".xlsx","application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                {".dot","application/msword" },
            {".dotx","application/vnd.openxmlformats-officedocument.wordprocessingml.template" },
                {".docm","application/vnd.ms-word.document.macroEnabled.12" },
                {".dotm","application/vnd.ms-word.template.macroEnabled.12" },
            {".xlt","application/vnd.ms-excel" },
            {".xla","application/vnd.ms-excel" },
                {".xltx","application/vnd.openxmlformats-officedocument.spreadsheetml.template" },
                {".xltm","application/vnd.ms-excel.template.macroEnabled.12" },
            {".xlsm","application/vnd.ms-excel.sheet.macroEnabled.12" },
                {".xlam","application/vnd.ms-excel.addin.macroEnabled.12" },
                {".xlsb","application/vnd.ms-excel.sheet.binary.macroEnabled.12" },
                {".ppt","application/vnd.ms-powerpoint" },
                {".pot","application/vnd.ms-powerpoint" },
            {".pps","application/vnd.ms-powerpoint" },
                {".ppa","application/vnd.ms-powerpoint" },
                {".pptx","application/vnd.openxmlformats-officedocument.presentationml.presentation" },
                {".potx","application/vnd.openxmlformats-officedocument.presentationml.template" },
                {".ppsx","application/vnd.openxmlformats-officedocument.presentationml.slideshow" },
            {".ppam","application/vnd.ms-powerpoint.addin.macroEnabled.12" },
                {".pptm","application/vnd.ms-powerpoint.presentation.macroEnabled.12" },
                {".potm","application/vnd.ms-powerpoint.template.macroEnabled.12" },
                {".ppsm","application/vnd.ms-powerpoint.slideshow.macroEnabled.12" },
                {".mdb","application/vnd.ms-access" }
            };
        }

        ///<Summary>
        /// Get Signing file view
        ///</Summary>
        public responseBack E_SignView(string path, string key, string user)
        {

            try
            {
                responseBack res = new responseBack();
                string private_key_file_path = String.Empty, keyfile_password = String.Empty;
                path = path.Replace("%23", "#");
                string host = conf["App:ServerRootAddress"].ToString();
                var folderName = Path.Combine(@"Common/Paperless/" + path);
                var folderName2 = Path.Combine(folderName + "/" + key);
                string newpath = folderName2.Replace("/", "\\");
                string urlpath = folderName2.Replace("\\", "/");
                string file = Path.Combine(_hostingEnvironment.WebRootPath + "\\" + newpath.Replace("%23", "#"));
                var file_Id = _srfilemapRepository.GetAll().Where(x => x.FileName == file).FirstOrDefault();
                var usr = _userRepository.GetAll().Where(x => x.Id == Convert.ToInt32(user)).FirstOrDefault();
                var sign = _esignRepository.GetAll().Where(x => x.FileName == key && x.EmailId != usr.UserName).FirstOrDefault();
                if (sign.Status == "Signed")
                {
                    res.firstPara = sign.Status;
                    res.secondPara = sign.EmbeddedToken;
                }
                else
                {
                    res.firstPara = sign.EmbeddedURL;
                    res.secondPara = sign.EmbeddedToken;
                }

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
                writer.WriteLine("Error in E_Sign method for -: error=" + ex.ToString() + DateTime.Now.ToString());
                writer.Close();
                return null;
            }
        }

        ///<Summary>
        /// Download signed document
        ///</Summary>
        public async Task<responseBack> E_SignDocDownload(string mail, string fileName)
        {
            try
            {
                string log = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");
                if (!System.IO.File.Exists(log))
                {
                    FileStream fs1 = new FileStream(log, FileMode.OpenOrCreate, FileAccess.Write);
                }
                StreamWriter writer = new StreamWriter(log);
                writer.WriteLine("Nitin -: error=" + mail.ToString() + DateTime.Now.ToString());
                string client_id = conf["ESign:client_id"].ToString();
                string client_secret = conf["ESign:client_secret"].ToString();
                string base64Decoded;
                //fileName = Convert.ToBase64String(Encoding.UTF8.GetBytes(fileName));
                byte[] d = System.Convert.FromBase64String(fileName.Replace(" ", "+"));
                base64Decoded = System.Text.ASCIIEncoding.ASCII.GetString(d);
                var filename = base64Decoded.Replace("%23", "#");
                var esignData = _esignRepository.GetAll().Where(x => x.EmailId == mail && x.FileName == filename).FirstOrDefault();
                var id = _userRepository.GetAll().Where(x => x.UserName == mail).FirstOrDefault();
                var fileInfo = _srfilemapRepository.GetAll().Where(x => x.UserId == id.Id && x.FileName.Contains(filename) && x.Action.Contains("S")).FirstOrDefault();

                await Publish_Signing_Notification(base64Decoded, id.FullName);
                long folderid = 0, docNumber = 1;
                if (esignData != null)
                {
                    esignData.Status = "Signed";
                    folderid = esignData.FolderId;
                    docNumber = esignData.DocumentId;
                    _esignRepository.InsertOrUpdate(esignData);
                }
                var request = (HttpWebRequest)WebRequest.Create("https://www.esigngenie.com/esign/api/oauth2/access_token");
                var postData = "grant_type=client_credentials" + "&client_id=" + client_id + "& client_secret=" + client_secret + "& scope=read-write";
                var data = Encoding.ASCII.GetBytes(postData);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                //var response = (HttpWebResponse)request.GetResponse();
                //string accessToken = "";
                //using (var streamReader = new StreamReader(response.GetResponseStream()))
                //{
                //    var responseString = streamReader.ReadToEnd();
                //    AccessToken myDeserializedClass = JsonConvert.DeserializeObject<AccessToken>(responseString);
                //    accessToken = myDeserializedClass.access_token;

                //}

                string Access_Token = conf["ESign:Access_Token"].ToString();
                var webAddr = "https://www.esigngenie.com/esign/api/folders/document/download?folderId=" + folderid + "&docNumber=1&access_token=" + Access_Token;

                using (var client = new WebClient())
                {
                    // client.DownloadFile(webAddr, "C:/codebase dec 2021/aspnet-core/src/SR.EscrowBaseWeb.Web.Host/wwwroot/Common/Paperless/EscrowInc/EscrowInc/07-6549/weeeeekkkkehyet-#10222.ZAS_~'~{BR1-READS}{SR1-READS}{SR2-READS}.doc05-28-2021(Mary Jones).pdf");
                    client.DownloadFile(webAddr, fileInfo.FileName);
                }

                byte[] e = System.Convert.FromBase64String(fileName.Replace(" ", "+"));
                base64Decoded = System.Text.ASCIIEncoding.ASCII.GetString(e);
                var filename1 = base64Decoded.Replace("%23", "#");

                var fileInfo1 = _srfilemapRepository.GetAll().Where(x => x.FileName.Contains(filename1)).FirstOrDefault();


                var EscrowIds = fileInfo1.EscrowiId;
                var usrid = _escrowDetailRepository.GetAll().Where(x => x.EscrowId == EscrowIds).ToList();

                var detail = _ISrEscrowRepository.GetAll().Where(x => x.EscrowNo == EscrowIds).FirstOrDefault();
                var emailing = detail.EOEmail;
                //var emaillist = mail + ',' + emails;
                //var list = emaillist.Split(',');
                EscrowDetail item = new EscrowDetail();
                item.Email = emailing;
                usrid.Add(item);
                foreach (var itms in usrid)
                {

                    string match = String.Empty;
                    MatchCollection matches = regexx.Matches(filename1);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string mails = String.Empty;
                        string rep = matches[i].Value.Replace("{", "").Replace("}", "");
                        int index = rep.IndexOf('-');
                        match = rep.Substring(index + 1);
                        //var v = _escrowDetailRepository.GetAll().Where(x => x.Email == itms.Email).ToList();
                        //foreach(var itm in v) {
                        if (itms.Usertype == null)
                        {
                            itms.Usertype = "";
                        }
                        if ((match.Contains("A") && rep.Contains(itms.Usertype)) || itms.Usertype == "")
                        {

                            string file = fileInfo1.FileName;

                            MailMessage mailw = new MailMessage();
                            mailw.From = new MailAddress("Noreply@EscrowBasePortal.com");
                            mailw.To.Add(itms.Email);
                            mailw.Subject = "File Signed";
                            //Attachment datas = new Attachment(file, MediaTypeNames.Application.Octet);
                            //mailw.Attachments.Add(datas);
                            string referer = conf["App:ClientRootAddress"].ToString();
                            var escrow = _escrowDetailRepository.GetAll().Where(x => x.EscrowId == EscrowIds).FirstOrDefault();
                            var enterprises = _enterpriseRepository.GetAll().Where(x => x.EnterpriseName == escrow.Company).FirstOrDefault();
                            var Company = enterprises.EnterpriseName;
                            var userName = escrow.Name;
                            var logo = enterprises.Logo;
                            if (logo == "" || logo == null) { logo = "https://ayushkamiya.com/Escrow-logo.png"; }
                            var Message = "A doc has been esigned By " + mail + " on the secure web portal regarding your escrow " + EscrowIds + " that needs your attention. Please log in to review, this reminder was sent by escrowbaseweb powered by software reality. File Name is " + "'" + filename1 + "'";
                            string texts = "";
                            using (StreamReader reader = System.IO.File.OpenText("wwwroot\\notification.html")) // Path to your Email format
                            {
                                texts = reader.ReadToEnd();
                                texts = texts.Replace("$$Company$$", Company).Replace("$$Logo$$", logo).Replace("$$Message$$", Message).Replace("$$userName$$", userName);
                            }
                            Random rnd = new Random();
                            mailw.IsBodyHtml = true;
                            mailw.Body = texts;
                            SmtpClient SmtpServer = new SmtpClient();
                            SmtpServer.Port = 587;
                            SmtpServer.Credentials = new System.Net.NetworkCredential("office@mandavconsultancy.com", "aouownmhogfobzbc");
                            SmtpServer.Host = "smtp.gmail.com";
                            SmtpServer.EnableSsl = true;
                            SmtpServer.Send(mailw);

                            List<ExpandoObject> result = new List<ExpandoObject>();
                            var userid = escrow.UserId;

                            string cs = conf["ConnectionStrings:Default"].ToString();
                            {
                                using var con = new MySqlConnection(cs);
                                con.Open();
                                var sql = $"Select * from usersmsprefrence where UserId=@UserId AND functionName=@functionName";
                                using (var cmd = new MySqlCommand(sql, con))
                                {
                                    cmd.Parameters.AddWithValue("@UserId", userid);
                                    cmd.Parameters.AddWithValue("@functionName", "Sign");

                                    var dr = cmd.ExecuteReader();
                                    var dataTable = new DataTable();
                                    dataTable.Load(dr);
                                    string JSONString = string.Empty;
                                    JSONString = JsonConvert.SerializeObject(dataTable);
                                    result = JsonConvert.DeserializeObject<List<ExpandoObject>>(JSONString);
                                }
                            }

                            if (result.Count > 0)
                            {
                                string pno = "";
                                string phoneno = "7148121408";
                                //var user = _userRepository.GetAll().Where(x => x.Id == itms.UserId).FirstOrDefault();
                                //string phoneno = user.PhoneNumber;
                                //if(phoneno != null) { }
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
                                messageOptions.Body = "A doc has been esigned By " + mail + " on the secure web portal regarding your escrow " + EscrowIds + " that needs your attention. Please log in to review, this reminder was sent by escrowbaseweb powered by software reality. File Name is " + "'" + filename1 + "'";
                                messageOptions.From = SenderNumber;
                                User.PhoneNumber = pno;
                                GetUsersInput input = new GetUsersInput();
                                //input.Filter = un;
                                input.MaxResultCount = 1000;

                                var messages = MessageResource.Create(messageOptions);
                                //res = message.Status.ToString();
                            }
                            else { }

                        }
                        else { }
                        // }

                    }

                }
                return null;
            }
            catch (Exception ex)
            {
                string logs = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");
                if (!System.IO.File.Exists(logs))
                {
                    FileStream fs1 = new FileStream(logs, FileMode.OpenOrCreate, FileAccess.Write);
                }
                StreamWriter writer = new StreamWriter(logs, true);
                writer.WriteLine("Error in E_SignDocDownload method for -: error=" + ex.ToString() + DateTime.Now.ToString());
                writer.Close();
                return null;
            }
        }


        ///<Summary>
        /// Send Notification to the users
        ///</Summary>
        public async Task Publish_Signing_Notification(string filename, string name)
        {
            var fileName = filename.Replace("%23", "#");
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
                                  where p.FileName == fileName
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


                    //var currentUser = (await this.GetCurrentUserAsync()).ToUserIdentifier();

                    await _notificationPublisher.PublishAsync("NotificationTest", new MessageNotificationData(message), severity: NotificationSeverity.Info, userIds: new[] { ghh });
                }
            }
            catch (Exception ex)
            {

            }
        }

        ///<Summary>
        /// Update document status
        ///</Summary>
        public void DocUpdate(string message, string filename, long userId)
        {
            var update = _srAssignedFilesDetailRepository.GetAll().Where(x => x.UserId == userId && x.FileName == filename.Replace("%23", "#")).FirstOrDefault();
            if (update != null)
            {
                if (message == "Read")
                {
                    update.ReadStatus = message;
                }
                if (message == "Input")
                {
                    update.InputStatus = "Input Completed";
                }
                if (message == "Sign")
                {
                    update.SigningStatus = "Signed";
                }
                update.UpdatedOn = DateTime.Now;
                var upd = _srAssignedFilesDetailRepository.InsertOrUpdate(update);

            }
        }

        ///<Summary>
        /// Get signing file status details
        ///</Summary>
        public List<EsignNameStatus> GetSignDetailsFile(string type, string filename)
        {
            List<EsignNameStatus> zohosignInPopup = new List<EsignNameStatus>();


            var get = _esignRepository.GetAll().Where(x => x.FileName == filename).ToList();
            var JoinResult = (from p in get.AsEnumerable()
                              select new EsignNameStatus()
                              {
                                  Status = p.Status,
                                  Signin_percentage = p.Signin_percentage

                              }).ToList();
            return JoinResult;
        }

        ///<Summary>
        /// Get signing file status details
        ///</Summary>
        public async Task<List<EsignNameStatus>> GetSignDetails(string type, string filename, string Escrow)
        {
            List<EsignNameStatus> zohosignInPopup = new List<EsignNameStatus>();

            var get = _esignRepository.GetAll().Where(x => x.FileName == filename).ToList();
            var JoinResult = (from p in get.AsEnumerable()
                              select new EsignNameStatus()
                              {
                                  Status = p.Status,
                                  Signin_percentage = p.Signin_percentage

                              }).ToList();

            var dbEsignRecord = _e_SignRecordsAppService.GetAllE_Sign(2001);
            var find = dbEsignRecord.Where(x => x.FileName == filename).FirstOrDefault();
            if (find != null)
            {
                var EscrowDetails = _escrowDetailRepository.GetAll().Where(x => x.EscrowId == Escrow).ToList();

                var myDeserializedClass = await getSignStatus(find.RequestId);
                var action = find.ZohoAction;
                var ZohoAction = JsonConvert.DeserializeObject<List<Action>>(find.ZohoAction);
                if (myDeserializedClass.document_form_data != null && myDeserializedClass.document_form_data.actions != null)
                {
                    var signPercentage = myDeserializedClass.document_form_data.actions;
                    foreach (var item in ZohoAction)
                    {

                        EsignNameStatus zohosignIn = new EsignNameStatus();
                        zohosignIn.Email = item.recipient_email;
                        zohosignIn.Name = item.recipient_name;
                        zohosignIn.UserType = EscrowDetails.Where(x => x.Email == zohosignIn.Email).FirstOrDefault().Usertype;
                        zohosignIn.signing_order = item.signing_order;
                        zohosignIn.TotalSignatureCount = item.fields.Where(x => x.field_type_name == "Signature").ToList().Count();
                        zohosignIn.TotalinitialsCount = item.fields.Where(x => x.field_type_name == "Initial").ToList().Count();
                        zohosignIn.TotalMandatorySignatureCount = item.fields.Where(x => x.field_type_name == "Signature" && x.is_mandatory == true).ToList().Count();
                        zohosignIn.TotalMandatoryInitialsCount = item.fields.Where(x => x.field_type_name == "Initial" && x.is_mandatory == true).ToList().Count();
                        zohosignIn.TotalOptinalSignatureCount = zohosignIn.TotalSignatureCount - zohosignIn.TotalMandatorySignatureCount;
                        zohosignIn.TotalOptinalInitialsCount = zohosignIn.TotalinitialsCount - zohosignIn.TotalMandatoryInitialsCount;


                        var signData = signPercentage.Where(x => x.recipient_email == item.recipient_email).FirstOrDefault();
                        if (signData != null)
                        {
                            if (!string.IsNullOrWhiteSpace(signData.signed_time))
                            {
                                zohosignIn.ZohoSignSignature = true;
                            }
                            else
                            {
                                zohosignIn.ZohoSignSignature = false;
                            }
                        }
                        else
                        {
                            zohosignIn.ZohoSignSignature = false;
                        }
                        zohosignInPopup.Add(zohosignIn);


                    }
                }
                else
                {
                    foreach (var item in ZohoAction)
                    {

                        EsignNameStatus zohosignIn = new EsignNameStatus();
                        zohosignIn.Email = item.recipient_email;
                        zohosignIn.Name = item.recipient_name;
                        zohosignIn.UserType = EscrowDetails.Where(x => x.Email == zohosignIn.Email).FirstOrDefault().Usertype;
                        zohosignIn.signing_order = item.signing_order;
                        zohosignIn.TotalSignatureCount = item.fields.Where(x => x.field_type_name == "Signature").ToList().Count();
                        zohosignIn.TotalinitialsCount = item.fields.Where(x => x.field_type_name == "Initial").ToList().Count();
                        zohosignIn.TotalMandatorySignatureCount = item.fields.Where(x => x.field_type_name == "Signature" && x.is_mandatory == true).ToList().Count();
                        zohosignIn.TotalMandatoryInitialsCount = item.fields.Where(x => x.field_type_name == "Initial" && x.is_mandatory == true).ToList().Count();
                        zohosignIn.TotalOptinalSignatureCount = zohosignIn.TotalSignatureCount - zohosignIn.TotalMandatorySignatureCount;
                        zohosignIn.TotalOptinalInitialsCount = zohosignIn.TotalinitialsCount - zohosignIn.TotalMandatoryInitialsCount;
                        zohosignIn.ZohoSignSignature = false;
                        zohosignInPopup.Add(zohosignIn);

                    }
                }
            }


            return zohosignInPopup.ToList();
        }

        public async Task<ZohoSignatureStatus> getSignStatus(string requestId)
        {
            ZohoSignatureStatus myDeserializedClass = new ZohoSignatureStatus();
            if (!string.IsNullOrWhiteSpace(requestId))
            {
                var accessToken = await ZohoESignGetAccessToken();
                string apiUrl = $"https://sign.zoho.in/api/v1/requests/{requestId}/fielddata";

                // Replace {documentId} with the actual document ID you want to download


                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                    // Make a POST request to download the document
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        myDeserializedClass = JsonConvert.DeserializeObject<ZohoSignatureStatus>(responseBody);
                    }

                }

            }
            return myDeserializedClass;
        }

        public async Task<string> ZohoESignGetAccessToken()
        {
            responseBack res = new responseBack();
            string refreshToken = string.Empty;
            string cs = conf["ConnectionStrings:Default"].ToString();
            {
                using var con = new MySqlConnection(cs);
                con.Open();
                var sql = $"Select RefreshToken from   srescrowdev2.e_signcompany  where SystemCode=@SystemCode ";
                using (var cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@SystemCode", 2001);
                    var dr = cmd.ExecuteReader();
                    var dataTable = new DataTable();
                    dataTable.Load(dr);
                    string JSONString = string.Empty;
                    JSONString = JsonConvert.SerializeObject(dataTable);
                    List<RefreshTokenResponse> _refreshToken = JsonConvert.DeserializeObject<List<RefreshTokenResponse>>(JSONString);
                    refreshToken = _refreshToken.FirstOrDefault().RefreshToken;
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
                            ZohotokenApiResponse zohotokenApiResponse = JsonConvert.DeserializeObject<ZohotokenApiResponse>(responseBody);
                            //ZohoESignGetAccessToken(zohotokenApiResponse.access_token);
                            return zohotokenApiResponse.access_token;
                        }
                    }
                    else
                    {
                        // Handle the error response.
                        Console.WriteLine($"Error: {response1.StatusCode} - {response1.ReasonPhrase}");
                    }
                }

                #endregion

            }
            catch (Exception ex)
            {

            }
            return "";
        }



        ///<Summary>
        /// Get status history of file
        ///</Summary>
        public List<StatusFiles> fileHistoryView(string userId, string type, string filename)
        {
            var get = _srAssignedFilesDetailRepository.GetAll().ToList();
            var set = _userRepository.GetAll().ToList();
            var JoinResult = (from p in get.AsEnumerable()
                              join t in set.AsEnumerable()
                              on p.UserId equals t.Id
                              where p.FileName == filename
                              select new StatusFiles()
                              {
                                  Name = t.Name,
                                  ReadStatus = p.ReadStatus,
                                  InputStatus = p.InputStatus,
                                  SignStatus = p.SigningStatus,
                                  UpdatedOn = DateTime.Now,

                              }).ToList();
            return JoinResult.ToList();
        }

        public responseBack ProcessRequest(string path, string userId)
        {
            responseBack res = new responseBack();

            try
            {
                if (Request.Form.Files.Count == 0)
                {
                    res.message = "No file found in request.";
                    res.statusCode = 400;
                    return res;
                }

                var file = Request.Form.Files[0];
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

                var folderPath = Path.Combine("wwwroot", "Common", "Paperless", path.Replace("/", "\\"));
                var fullSavePath = Path.Combine(Directory.GetCurrentDirectory(), folderPath);
                var fullFilePath = Path.Combine(fullSavePath, fileName);

                // Ensure directory exists
                if (!Directory.Exists(fullSavePath))
                {
                    Directory.CreateDirectory(fullSavePath);
                }

                if (System.IO.File.Exists(fullFilePath))
                {
                    res.message = "!Oops same name file already exists, please change file name first.";
                    res.statusCode = 500;
                    return res;
                }

                using (var stream = new FileStream(fullFilePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                if (userId != "1")
                {
                    var splitPath = path.Replace("/", "\\").Split("\\");
                    if (splitPath.Length < 3)
                    {
                        res.message = "Invalid path format.";
                        res.statusCode = 400;
                        return res;
                    }

                    var escrowId = splitPath[2];
                    var coesfm = new CreateOrEditSrFileMappingDto
                    {
                        UserId = Convert.ToInt32(userId),
                        FileName = fullFilePath,
                        IsActive = true,
                        EscrowiId = escrowId,
                        Action = "READ"
                    };

                    var existingMapping = _srfilemapRepository.GetAll()
                        .FirstOrDefault(x => x.UserId == coesfm.UserId);

                    var filemap = _ISrFileMappingsAppService.CreateOrEdit(coesfm);
                }

                res.statusCode = 200;
                res.message = "File uploaded successfully.";
                return res;
            }
            catch (Exception ex)
            {
                string logPath = Path.Combine(_hostingEnvironment.WebRootPath, "Logs", "Logs.txt");
                Directory.CreateDirectory(Path.GetDirectoryName(logPath)); // ensure Logs folder exists
               // File.AppendAllText(logPath, $"[{DateTime.Now}] Error in ProcessRequest: {ex}\n");

                res.message = ex.Message;
                res.statusCode = 500;
                return res;
            }
        }
        ///<Summary>
        /// Files and directories shown for escrow documents
        ///</Summary>
        public object FileSystem(string company, string subCompany, string escrow, string userId, FileSystemCommand command, string arguments, string usertype, string usersname)
        {
            string test = approve;

            string Logs = @"wwwroot\\Logs";
            if (!Directory.Exists(Logs))
            {
                System.IO.Directory.CreateDirectory(Logs);
            }
            string filename = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");
            if (!System.IO.File.Exists(filename))
            {
                FileStream fs1 = new FileStream(filename, FileMode.Create, FileAccess.Write);
            }

            string lastchracter = company.Substring(company.Length - 1);
            if (lastchracter == ".")
            {
                company = company.Remove(company.Length - 1, 1);

            };
            string subCompanylst = subCompany.Substring(subCompany.Length - 1);
            if (subCompanylst == ".")
            {
                subCompany = subCompany.Remove(subCompany.Length - 1, 1);

            };


            var currentUserId = userId;
            GetAllSrFileMappingsInput obj = new GetAllSrFileMappingsInput();
            obj.Filter = userId;
            var checkpath = Path.Combine(_hostingEnvironment.WebRootPath, SampleImagesRelativePath);
            var config = new FileSystemConfiguration
            {
                Request = Request,
                FileSystemProvider = new PhysicalFileSystemProvider(
                    Path.Combine(_hostingEnvironment.WebRootPath, SampleImagesRelativePath),
                (fileSystemItem, clientItem) =>
                {
                    if (!clientItem.IsDirectory)
                        clientItem.CustomFields["url"] = GetFileItemUrl(fileSystemItem);
                }
                ),
                AllowCopy = false,
                AllowMove = false,
                AllowDelete = false,
                AllowRename = true,
                AllowUpload = false,
                AllowDownload = true,
                AllowedFileExtensions = new string[] { ".pdf", ".txt" }
            };
            var processor = new FileSystemCommandProcessor(config);
            var result = processor.Execute(command, arguments);


            //var checkPermission = _ISrFileMappingsAppService.GetAll(obj);
            //int stor = Convert.ToInt32(userId);
            var checkPermission = _srfilemapRepository.GetAll();
            var usrdetail = _userRepository.GetAll().Where(x => x.Id == Convert.ToInt64(userId)).FirstOrDefault();
            var jsonConvert = JsonConvert.SerializeObject(result.GetClientCommandResult());
            Root temp = JsonConvert.DeserializeObject<Root>(jsonConvert);

            if (usersname == "admin")
            {
                return temp.result;
            }
            else
            {
                if (checkPermission != null && usertype.Trim() != "EO")
                {
                    List<Result> newFile = new List<Result>();
                    Root filterFile = new Root();
                    var results = temp.result.Where(x => x.key == company || x.key.Contains("\\")).ToList();


                    foreach (var lst in results)
                    {
                        if (lst.key.Contains("\\Other"))
                        {
                            continue;
                        }
                        if (lst.name == "(0)0000aaaSeller Opening Documents.txt")
                        {
                            if (usertype == "SR1" || usertype == "SR2" || usertype == "SR3" || usertype == "SR4" || usertype == "SR5" || usertype == "SR6" || usertype == "SR7" || usertype == "SR8" || usertype == "SR9" || usertype == "SR10" || usertype == "SRX")
                            {
                                Result res = new Result();
                                var data = GetSignDetailsFile(usrdetail.EmailAddress, lst.name.TrimEnd());
                                res = lst;
                                res.key = lst.name;
                                res.action = "Fill out Completely";
                                res.status = "Sent 12/10/20 Completed";
                                res.name = "SELLER OPENING INFORMATION";
                                newFile.Add(res);
                            }
                        }
                        else if (lst.name == "(0)0000aaaBuyer Opening Documents.txt")
                        {
                            if (usertype == "BR1" || usertype == "BR2" || usertype == "BR3" || usertype == "BR4" || usertype == "BR5" || usertype == "BR6" || usertype == "BR7" || usertype == "BR8" || usertype == "BR9" || usertype == "BR10" || usertype == "BRX")
                            {
                                Result res = new Result();
                                var data = GetSignDetailsFile(usrdetail.EmailAddress, lst.name.TrimEnd());
                                res = lst;
                                res.key = lst.name;
                                res.action = "Fill out Completely";
                                res.status = "Completed";
                                res.name = "BUYER OPENING INFORMATION";
                                newFile.Add(res);
                            }
                        }
                        else
                        {

                            string strBunch = company + "\\" + subCompany + "\\" + escrow + "\\";
                            try
                            {
                                var find = checkPermission.ToList().Where(x => x.FileName.ToString().Replace(" ", "").Contains(strBunch, StringComparison.OrdinalIgnoreCase)).ToList();


                                if (find != null)
                                {
                                }
                            }
                            catch (Exception ex)
                            {

                            }


                            var temp1 = checkPermission.Where(x => x.FileName.Contains(strBunch) && x.FileName.Contains(lst.name) && x.Action.Contains(usertype)
                            // && x.UserId == Convert.ToInt32(userId)

                            ).FirstOrDefault();

                            if (temp1 == null)
                            {
                                if (usertype.StartsWith("SR") == true)
                                {
                                    temp1 = checkPermission.Where(x => x.FileName.Contains(strBunch) && x.FileName.Contains(lst.name) && x.Action.Contains("SRX")).FirstOrDefault();
                                }
                                else if (usertype.StartsWith("BR") == true)
                                {
                                    temp1 = checkPermission.Where(x => x.FileName.Contains(strBunch) && x.FileName.Contains(lst.name) && x.Action.Contains("BRX")).FirstOrDefault();
                                }
                                else if (usertype.StartsWith("TC") == true)
                                {
                                    temp1 = checkPermission.Where(x => x.FileName.Contains(strBunch) && x.FileName.Contains(lst.name) && x.Action.Contains("TCX")).FirstOrDefault();
                                }
                                if (temp1 == null)
                                {
                                    temp1 = checkPermission.Where(x => x.FileName.Contains(company) && x.FileName.Contains(subCompany) && x.FileName.Contains(escrow) && x.Action.Contains(usertype)).FirstOrDefault();
                                }

                            }


                            var comp = lst.name.Contains(usertype);
                            string extension = Path.GetExtension(lst.name);
                            bool isValidFileForUserType = false;
                            if (String.IsNullOrEmpty(extension) || extension != ".pdf")
                            {
                                isValidFileForUserType = true;
                            }
                            else
                            {

                                comp = lst.name.Contains(usertype);
                                if (comp == true)
                                {
                                    isValidFileForUserType = true;
                                }

                                else
                                {
                                    if (usertype.Contains("BR") && lst.name.Contains("BRX"))
                                    {
                                        isValidFileForUserType = true;
                                    }
                                    else if (usertype.Contains("SR") && lst.name.Contains("SRX"))
                                    {
                                        isValidFileForUserType = true;
                                    }
                                    else if (usertype.Contains("TC") && lst.name.Contains("TCX"))
                                    {
                                        isValidFileForUserType = true;
                                    }
                                    else
                                    {
                                        isValidFileForUserType = false;
                                    }
                                }
                            }

                            if (temp1 != null && isValidFileForUserType)
                            {
                                Result res = new Result();

                                res = lst;
                                res.key = lst.name;
                                int index = lst.name.IndexOf("~");
                                if (index != -1)
                                {
                                    var userType = "";
                                    var userTypeQuery = _srfilemapRepository.GetAll().Where(x => x.UserId == int.Parse(userId) && x.Action.Contains(usertype) && x.FileName.Contains(lst.name.ToString())).FirstOrDefault();

                                    if (userTypeQuery == null)
                                    {
                                        if (usertype.StartsWith("SR") == true)
                                        {
                                            userTypeQuery = _srfilemapRepository.GetAll().Where(x => x.UserId == int.Parse(userId) && x.Action.Contains("SRX") && x.FileName.Contains(lst.name.ToString())).FirstOrDefault();
                                        }
                                        else if (usertype.StartsWith("BR") == true)
                                        {
                                            userTypeQuery = _srfilemapRepository.GetAll().Where(x => x.UserId == int.Parse(userId) && x.Action.Contains("BRX") && x.FileName.Contains(lst.name.ToString())).FirstOrDefault();
                                        }
                                        else if (usertype.StartsWith("TC") == true)
                                        {
                                            userTypeQuery = _srfilemapRepository.GetAll().Where(x => x.UserId == int.Parse(userId) && x.Action.Contains("TCX") && x.FileName.Contains(lst.name.ToString())).FirstOrDefault();
                                        }
                                    }

                                    if (userTypeQuery != null)
                                    {
                                        userType = userTypeQuery.Action;
                                    }
                                    int index1 = userType.IndexOf("-");

                                    if (index1 > 0)
                                    {
                                        userType = userType.Substring(0, index1);

                                    }

                                    // new  code 

                                    // Fetch a single record for the logged-in user and file
                                    var st = _srAssignedFilesDetailRepository
                                        .GetAll()
                                        .FirstOrDefault(x => x.FileName == lst.name.TrimEnd() && x.UserId == usrdetail.Id);

                                    // If there's no record for this user, you can initialize default values here
                                    if (st == null)
                                    {
                                        // Set default or empty values when no record exists
                                        res.srAssignedFileId = 0;
                                        res.signing = "No record found";
                                        res.status = "Not applicable";
                                        return res; // or continue based on your logic
                                    }
                                    var filefound = _srfilemapRepository.GetAll().ToList();
                                    var selectedfile = filefound.Where(x => x.FileName.Contains(st.FileName)).FirstOrDefault();
                                    var SRFileMasterId = _srEscrowFileMasterRepository.GetAll().Where(x => x.FileShortName == st.FileName).FirstOrDefault();
                                    res.OtherAction = SRFileMasterId.OtherAction == true ? true : false;
                                    res.OtherActionNote = SRFileMasterId?.OtherActionNote;
                                    res.srAssignedFileId = SRFileMasterId.Id;
                                    res.signStatus = st.SigningStatus;

                                    var lastUpdated = _escrowFileHistoryRepository.GetAll().Where(x => x.SrEscrowFileMasterId == SRFileMasterId.Id && x.ActionType != FileConstantAction.Download_File).OrderByDescending(x => x.CreatedAt).FirstOrDefault();
                                    if (lastUpdated != null)
                                    {
                                       // res.updateOn = Convert.ToString(lastUpdated.CreatedAt);
                                        res.updateOn = lastUpdated.CreatedAt.ToString("MM/dd/yy HH:mm");
                                    }
                                    else
                                    {
                                        res.updateOn = st.UpdatedOn.ToString("MM/dd/yy HH:mm");
                                    }


                                    // Get signing details based on email and file name
                                    var data = GetSignDetailsFile(usrdetail.EmailAddress, lst.name.TrimEnd());
                                    var Signin_percentage = string.Empty;
                                    bool signed = false, unsigned = false, partialSigned = false;

                                    if (data.Any())
                                    {
                                        signed = data.Any(d => d.Status == "Signed");
                                        unsigned = data.Any(d => d.Status == "Unsigned");
                                        partialSigned = data.Any(d => d.Status == "Partially Signed");

                                        // Get the signing percentage from the relevant status
                                        var status = data.FirstOrDefault(d => d.Status == "Signed" || d.Status == "Partially Signed");
                                        if (status != null)
                                        {
                                            Signin_percentage = status.Signin_percentage;
                                        }
                                    }
                                    else
                                    {
                                        unsigned = true; // If no data is found, mark it as unsigned
                                    }

                                    // Determine the signing status
                                    if (signed)
                                    {
                                        if (res.signStatus == "Unsigned")
                                        {
                                            res.signing = $"{(Signin_percentage == "100" ? "Signed" : "Partially Signed")} - {Signin_percentage} %";
                                        }
                                        else
                                        {
                                            res.signing = $"✓  {(Signin_percentage == "100" ? "Signed" : "Partially Signed")} - {Signin_percentage} %";
                                        }


                                    }
                                    else if (unsigned)
                                    {
                                        res.signing = "Unsigned";
                                    }
                                    else
                                    {
                                        res.signing = string.Empty;
                                    }

                                    // Access and action logic based on the access type (userType)
                                    if (lst.key.Contains(userType) || (lst.key.Contains("BRX") || lst.key.Contains("SRX")))
                                    {
                                        if (lst.key.Contains("BRX") && userType.Contains("BR"))
                                        {
                                            var accessKey1 = lst.key;
                                            var idx1 = accessKey1.IndexOf('{');
                                            var idx2 = accessKey1.IndexOf('}');

                                            // Extracting the content between { and } 
                                            string tempAccess1 = idx1 >= 0 && idx2 > idx1 ? accessKey1.Substring(idx1 + 1, idx2 - idx1 - 1) : string.Empty;

                                            // Further split by "-" and get the second part (READS)
                                            if (!string.IsNullOrEmpty(tempAccess1) && tempAccess1.Contains("-"))
                                            {
                                                tempAccess1 = tempAccess1.Split('-')[1].Trim();
                                            }

                                            res.access = tempAccess1; // Expected to get "READS"
                                        }
                                        else if (lst.key.Contains("SRX") && userType.Contains("SR"))
                                        {
                                            var accessKey1 = lst.key;
                                            var idx1 = accessKey1.IndexOf('{');
                                            var idx2 = accessKey1.IndexOf('}');

                                            // Extracting the content between { and } 
                                            string tempAccess1 = idx1 >= 0 && idx2 > idx1 ? accessKey1.Substring(idx1 + 1, idx2 - idx1 - 1) : string.Empty;

                                            // Further split by "-" and get the second part (READS)
                                            if (!string.IsNullOrEmpty(tempAccess1) && tempAccess1.Contains("-"))
                                            {
                                                tempAccess1 = tempAccess1.Split('-')[1].Trim();
                                            }

                                            res.access = tempAccess1; // Expected to get "READS"
                                        }
                                        else
                                        {
                                            var accessKey = lst.key;
                                            var idx = accessKey.IndexOf('{' + usertype);
                                            string tempAccess = idx > 0 ? accessKey.Substring(idx + 5, 6).Replace('}', ' ').Trim() : string.Empty;

                                            res.access = tempAccess.Replace("-", "");
                                        }

                                        // Determine action and status based on the signing status
                                        if (res.access.Contains("S"))
                                        {
                                            res.action = usertype == "EOX"
                                            ? "No Action Required"
                                            : (st.SigningStatus == "Signed"
                                                ? "Completed"
                                                : "Fill out and Electronically Sign");
                                            res.status = st.SigningStatus == "Signed" ? "Signed Fully" : "Nobody signed yet";
                                        }
                                        else if (res.access.Contains("E"))
                                        {
                                            res.status = st.InputStatus;
                                            res.action = st.InputStatus == "Input Completed" ? "Completed" : "Fill out";
                                        }
                                        else
                                        {
                                            res.action = "No action required";
                                            res.status = st.ReadStatus == "Read" ? "Read by all" : "Read by none";
                                        }
                                    }

                                    res.name = lst.name.Substring(0, lst.name.IndexOf("~"));


                                    ///

                                }

                                if (lst.name.Contains("Other"))
                                {
                                    res.name = "Except";
                                }
                                else
                                {
                                    res.name = lst.name;
                                }
                                if (res.name != "Except")
                                    newFile.Add(res);
                            }
                        }
                    }

                    filterFile.result = newFile;

                    return filterFile.result;
                }
                else
                {
                    List<Result> newFile = new List<Result>();
                    Root filterFile = new Root();
                    List<int> myu = new List<int>();
                    var userType = _srfilemapRepository.GetAll().Where(x => x.UserId == int.Parse(userId)).FirstOrDefault().Action;
                    int index1 = userType.IndexOf("-");
                    if (index1 > 0)
                    {
                        userType = userType.Substring(0, index1);
                    }



                    GetAllSrInvitationRecordsInput oj = new GetAllSrInvitationRecordsInput();
                    oj.EscrowOfficerFilter = usersname;

                    var invi = _srInvitationRecordsAppService.GetAll(oj);
                    foreach (var useri in invi.Result.Items)
                    {
                        myu.Add((int)useri.SrInvitationRecord.UserId);
                    }
                    foreach (var lst in temp.result)
                    {
                        myu = myu.Distinct().ToList();
                        var userProfiles = _srfilemapRepository.GetAll().Where(t => myu.Contains(t.UserId)).ToList();
                        var cheermission = _ISrFileMappingsAppService.GetAll(obj);
                        var temp1 = (dynamic)null;



                        temp1 = userProfiles.Where(x => x.FileName.Contains(lst.name) && x.Action != "READ" && (myu.Contains(x.UserId))).ToList();

                        if (temp1 == null)
                        {
                            if (usertype.StartsWith("SR") == true)
                            {
                                temp1 = checkPermission.Where(x => x.FileName.Contains(lst.name) && x.Action != "READ").FirstOrDefault();
                            }
                            else if (usertype.StartsWith("BR") == true)
                            {
                                temp1 = checkPermission.Where(x => x.FileName.Contains(lst.name) && x.Action != "READ").FirstOrDefault();
                            }
                            else if (usertype.StartsWith("TC") == true)
                            {
                                temp1 = checkPermission.Where(x => x.FileName.Contains(lst.name) && x.Action != "READ").FirstOrDefault();
                            }

                        }


                        var comp = lst.name.Contains(usertype);
                        if (temp1.Count > 0)
                        {
                            Result res = new Result();
                            res = lst;
                            res.key = lst.name;
                            //}
                            int index = -1;
                            if (lst.name.Contains("~"))
                            {
                                index = lst.name.IndexOf("~");
                            }
                            else if (lst.name.Contains("-'-"))
                            {
                                index = lst.name.IndexOf("-'-");
                            }
                            else if (lst.name.Contains("_'_"))
                            {
                                index = lst.name.IndexOf("_'_");
                            }
                            if (index != -1)
                            {
                                var acesstype = lst.key.Contains(userType);
                                var stringComp = acesstype.ToString().IndexOf(userType);
                                var newaceess = lst.key;
                                var idx = newaceess.IndexOf(usertype);
                                string tempqw = newaceess.Substring(idx + 4, 6);
                                res.access = "READ";

                                res.name = lst.name.Substring(0, index);
                            }
                            newFile.Add(res);
                        }

                    }
                    filterFile.result = newFile;
                    return filterFile.result;
                }
            }
        }


        ///<Summary>
        /// files and directories shown  for other documents
        ///</Summary>
        public object FileSystem1(string company, string subCompany, string escrow, string userId, string arguments, FileSystemCommand command)
        {
            var currentUserId = userId;
            GetAllSrFileMappingsInput obj = new GetAllSrFileMappingsInput();
            obj.Filter = userId;

            var config = new FileSystemConfiguration
            {
                Request = Request,
                FileSystemProvider = new PhysicalFileSystemProvider(
                    Path.Combine(_hostingEnvironment.WebRootPath, SampleImagesRelativePath),
                (fileSystemItem, clientItem) =>
                {
                    if (!clientItem.IsDirectory)
                        clientItem.CustomFields["url"] = GetFileItemUrl(fileSystemItem);
                }
                ),
                AllowDownload = true
            };

            var processor = new FileSystemCommandProcessor(config);
            var result = processor.Execute(command, arguments);
            var checkPermission = _srfilemapRepository.GetAll();

            if (config.Request.Method == "POST")
            {
                return result.GetClientCommandResult();
            }
            else
            {
                var jsonConvert = JsonConvert.SerializeObject(result.GetClientCommandResult());
                Root temp = JsonConvert.DeserializeObject<Root>(jsonConvert);

                if (userId == "8")
                {
                    return temp.result;
                }
                else
                {
                    temp.result = temp.result.Where(x => x.key == company || x.key.Contains("\\")).ToList();
                    if (checkPermission != null)
                    {
                        List<Result> newFile = new List<Result>();
                        Root filterFile = new Root();

                        foreach (var lst in temp.result)
                        {
                            string strBunch = company + "\\" + subCompany + "\\" + escrow + "\\" + "Other";

                            var temp1 = checkPermission.Where(x => x.FileName.Contains(strBunch)
                            && x.FileName.Contains(lst.name)
                            && x.FileName.Contains("Other")

                            && x.Action == "READ").FirstOrDefault();
                            //     && x.UserId == Convert.ToInt32(userId)).FirstOrDefault();
                            Result res = new Result();

                            if (temp1 != null)
                            {
                                if (!string.IsNullOrWhiteSpace(temp1.FileName))
                                {
                                    var newString = temp1.FileName.Substring(temp1.FileName.IndexOf("\\Paperless") + 11);
                                    if (newString.Contains(strBunch) && newString.Contains(lst.name))
                                    {
                                        res = lst;
                                        res.key = lst.name;
                                        var fileExtension = Path.GetExtension(newString).ToLowerInvariant();

                                        if (fileExtension == ".pdf")
                                        {
                                            res.fileType = "/Images/pdf-file.png"; 
                                        }
                                        else if (fileExtension == ".docx")
                                        {
                                            res.fileType = "/Images/docx-file.png";
                                        }
                                        else if (fileExtension == ".doc")
                                        {
                                            res.fileType = "/Images/doc-file.png";
                                        }
                                        else if (fileExtension == ".rtf")
                                        {
                                            res.fileType = "/Images/rtf-file.png";
                                        }
                                        else if (fileExtension == ".txt")
                                        {
                                            res.fileType = "/Images/txt-file.png";
                                        }
                                        else if (fileExtension == ".eml")
                                        {
                                            res.fileType = "/Images/eml-file.png";
                                        }
                                        else if (fileExtension == ".msg")
                                        {
                                            res.fileType = "/Images/msg-file.png";
                                        }

                                        res = lst;
                                        res.key = lst.name;
                                        res.access = "READ";

                                        int index = lst.name.IndexOf("~");
                                        if (index != -1)
                                        {
                                            res.name = lst.name.Substring(0, index);
                                        }
                                        else
                                        {
                                            res.name = lst.name;
                                        }

                                        var findMappingTag = _tagsAndFileMappingsRepository
                                            .GetAll()
                                            .Where(x => x.FileName == res.name)
                                            .ToList();

                                        var tagIds = findMappingTag.Select(x => x.TagId).ToList();

                                        //var findTag = _escrowFileTagsRepository
                                        //    .GetAll()
                                        //    .Where(x => tagIds.Contains(x.Id) 
                                        //    && x.CreatedBy == long.Parse(currentUserId)).ToList();

                                        var findTag = _escrowFileTagsRepository
       .GetAll()
       .Where(x => tagIds.Contains(x.Id))
       .ToList();

                                        if (findTag.Count() > 0)
                                        {
                                            res.escrowFileTags = findTag;
                                        }
                                        newFile.Add(res);
                                    }

                                }
                            }
                            else
                            {
                                temp1 = checkPermission.Where(x => x.FileName.Contains(company)
                       //&& x.FileName.Contains(lst.name)
                       && x.FileName.Contains("Other")

                       && x.Action == "READ").FirstOrDefault();
                                //     && x.UserId == Convert.ToInt32(userId)).FirstOrDefault();


                                if (temp1 != null)
                                {
                                    if (!string.IsNullOrWhiteSpace(temp1.FileName))
                                    {
                                        var newString = temp1.FileName.Substring(temp1.FileName.IndexOf("\\Paperless") + 11);
                                        if (newString.Contains(strBunch) && newString.Contains(lst.name) || newString.Contains(company))
                                        {
                                            res = lst;
                                            res.key = lst.name;
                                            res.access = "READ";

                                            int index = lst.name.IndexOf("~");
                                            if (index != -1)
                                            {
                                                res.name = lst.name.Substring(0, index);

                                            }
                                            else
                                            {
                                                res.name = lst.name;
                                                res.fileType = $"";
                                            }
                                            newFile.Add(res);
                                            //  res.signing = $"✓  {(Signin_percentage == "100" ? "Signed" : "Partially Signed")} - {Signin_percentage} %";
                                        }

                                    }
                                }

                            }
                        }
                        filterFile.result = newFile;
                        return filterFile.result;
                    }
                }
            }
            return null;
        }

        ///<Summary>
        /// Get locations
        ///</Summary>
        string GetFileItemUrl(FileSystemInfo fileSystemItem)
        {
            if (Path.GetExtension(Convert.ToString(fileSystemItem)) == ".txt") { }
            else
            {
                var relativeUrl = fileSystemItem.FullName
                    .Replace(_hostingEnvironment.WebRootPath, "")
                    .Replace(Path.DirectorySeparatorChar, '/');
                return $"{Request.Scheme}://{Request.Host}{Request.PathBase}{relativeUrl}";
            }
            return null;
        }

        //public async Task<IActionResult> ConvertDocToDocxAsync([FromForm] string inputFilePath)
        //{
        //    string apiUrl = "https://api.pdf.co/v1/docx/convert/from/doc";
        //    string apiKey = "narender@mandavconsultancy.com_z56MDLfD2tahF0ECxC3M6VChszYeWQoD1z0S8ISTv8VG70AYhkIBFEidLzuKehsw";

        //    using (var client = new HttpClient())
        //    {
        //        client.DefaultRequestHeaders.Add("x-api-key", apiKey);

        //        var formData = new MultipartFormDataContent();
        //        formData.Add(new StringContent("application/vnd.openxmlformats-officedocument.wordprocessingml.document"), "file");

        //        byte[] fileBytes = System.IO.File.ReadAllBytes(inputFilePath);
        //        var fileContent = new ByteArrayContent(fileBytes);
        //        formData.Add(fileContent, "file", Path.GetFileName(inputFilePath));

        //        var response = await client.PostAsync(apiUrl, formData);
        //        response.EnsureSuccessStatusCode();

        //        byte[] resultFileBytes = await response.Content.ReadAsByteArrayAsync();

        //        string outputFilePath = Path.ChangeExtension(inputFilePath, ".docx");

        //        await System.IO.File.WriteAllBytesAsync(outputFilePath, resultFileBytes);

        //        return File(resultFileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "converted.docx");
        //    }
        //}

    }





    ///<Summary>
    /// Class EsignNameStatus
    ///</Summary>
    public class EsignNameStatus
    {
        ///<Summary>
        /// Parameter UserId
        ///</Summary>
        public long UserId { get; set; }

        ///<Summary>
        /// Parameter Name
        ///</Summary>
        public string Name { get; set; }

        ///<Summary>
        /// Parameter Status
        ///</Summary>
        public string Status { get; set; }

        public bool ZohoSignSignature { get; set; }

        ///<Summary>
        /// Parameter Signin_percentage
        ///</Summary>
        public string Signin_percentage { get; set; }

        ///<Summary>
        /// Parameter Email
        ///</Summary>
        public string Email { get; set; }

        ///<Summary>
        /// Parameter UserType
        ///</Summary>
        public string UserType { get; set; }

        ///<Summary>
        /// Parameter signing_order
        ///</Summary>
        public int? signing_order { get; set; }

        ///<Summary>
        /// Parameter TotalSignatureCount
        ///</Summary>
        public int? TotalSignatureCount { get; set; }

        ///<Summary>
        /// Parameter TotalinitialsCount
        ///</Summary>
        public int? TotalinitialsCount { get; set; }

        ///<Summary>
        /// Parameter TotalMandatorySignatureCount
        ///</Summary>
        public int? TotalMandatorySignatureCount { get; set; }

        ///<Summary>
        /// Parameter TotalMandatoryInitialsCount
        ///</Summary>
        public int? TotalMandatoryInitialsCount { get; set; }

        ///<Summary>
        /// Parameter TotalOptinalSignatureCount
        ///</Summary>
        public int? TotalOptinalSignatureCount { get; set; }

        ///<Summary>
        /// Parameter TotalOptinalInitialsCount
        ///</Summary>
        public int? TotalOptinalInitialsCount { get; set; }

    }

    ///<Summary>
    /// Class StatusFiles
    ///</Summary>
    public class StatusFiles
    {
        ///<Summary>
        /// Parameter Name
        ///</Summary>
        public string Name { get; set; }

        ///<Summary>
        /// Parameter ReadStatus
        ///</Summary>
        public string ReadStatus { get; set; }

        ///<Summary>
        /// Parameter InputStatus
        ///</Summary>
        public string InputStatus { get; set; }

        ///<Summary>
        /// Parameter SignStatus
        ///</Summary>
        public string SignStatus { get; set; }

        ///<Summary>
        /// Parameter UpdatedOn
        ///</Summary>
        public DateTime UpdatedOn { get; set; }
    }

    ///<Summary>
    /// Class Result
    ///</Summary>
    public class Result
    {
        ///<Summary>
        /// Parameter key
        ///</Summary>
        public string key { get; set; }

        ///<Summary>
        /// Parameter name
        ///</Summary>
        public string name { get; set; }

        ///<Summary>
        /// Parameter dateModified
        ///</Summary>
        public DateTime dateModified { get; set; }

        ///<Summary>
        /// Parameter isDirectory
        ///</Summary>
        public bool isDirectory { get; set; }

        ///<Summary>
        /// Parameter size
        ///</Summary>
        public int size { get; set; }

        ///<Summary>
        /// Parameter hasSubDiretories
        ///</Summary>
        public bool hasSubDirectories { get; set; }

        ///<Summary>
        /// Parameter access
        ///</Summary>
        public string access { get; set; }

        ///<Summary>
        /// Parameter status
        ///</Summary>
        public string status { get; set; }

        ///<Summary>
        /// Parameter signing
        ///</Summary>
        public string signing { get; set; }

        ///<Summary>
        /// Parameter link
        ///</Summary>
        public string link { get; set; }

        ///<Summary>
        /// Parameter action
        ///</Summary>
        public string action { get; set; }

        ///<Summary>
        /// Parameter update
        ///</Summary>
        public string updateOn { get; set; }
        /// <summary>
        /// Parameter srAssignedFileId 
        /// </summary>
        public long srAssignedFileId { get; set; }

        public string signStatus { get; set; }

        public string fileType { get; set; }

        public List<EscrowFileTags>? escrowFileTags { get; set; }

        public bool? OtherAction { get; set; }

        public string OtherActionNote { get; set; }

    }

    ///<Summary>
    /// Class responseBack
    ///</Summary>
    public class responseBack
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
        /// Parameter statusCode
        ///</Summary>
        public int statusCode { get; set; }

        ///<Summary>
        /// Parameter firstPara
        ///</Summary>
        public string firstPara { get; set; }

        ///<Summary>
        /// Parameter secondPara
        ///</Summary>
        public string secondPara { get; set; }

        ///<Summary>
        /// Parameter thirdPara
        ///</Summary>
        public string thirdPara { get; set; }

        ///<Summary>
        /// Parameter fourthPara
        ///</Summary>
        public string fourthPara { get; set; }

        public string signingStatus { get; set; }

        public bool Success { get; internal set; }
    }

    ///<Summary>
    /// Class Root
    ///</Summary>
    public class Root
    {
        ///<Summary>
        /// Parameter success
        ///</Summary>
        public bool success { get; set; }

        ///<Summary>
        /// Parameter errorId
        ///</Summary>
        public object errorId { get; set; }

        ///<Summary>
        ///
        /// Parameter result
        ///</Summary>
        public List<Result> result { get; set; }
    }

    ///<Summary>
    /// Class DestinationPathInfo
    ///</Summary>
    public class DestinationPathInfo
    {
        ///<Summary>
        /// Parameter key
        ///</Summary>
        public string key { get; set; }

        ///<Summary>
        /// Parameter name
        ///</Summary>
        public string name { get; set; }
    }

    ///<Summary>
    /// Class ChunkMetadata
    ///</Summary>
    public class ChunkMetadata
    {
        ///<Summary>
        /// Parameter UploadId
        ///</Summary>
        public string UploadId { get; set; }

        ///<Summary>
        /// Parameter FileName
        ///</Summary>
        public string FileName { get; set; }

        ///<Summary>
        /// Parameter Index
        ///</Summary>
        public int Index { get; set; }

        ///<Summary>
        /// Parameter TotalCount
        ///</Summary>
        public int TotalCount { get; set; }

        ///<Summary>
        /// Parameter FileSize
        ///</Summary>
        public int FileSize { get; set; }
    }

    ///<Summary>
    /// Class myNewRoot
    ///</Summary>
    public class myNewRoot
    {
        ///<Summary>
        /// Parameter destinationPathInfo
        ///</Summary>
        public List<DestinationPathInfo> destinationPathInfo { get; set; }

        ///<Summary>
        /// Parameter chunkMetadata
        ///</Summary>
        public ChunkMetadata chunkMetadata { get; set; }
    }

    ///<Summary>
    /// Class Tempratures
    ///</Summary>
    public partial class Temperatures
    {
        ///<Summary>
        /// Parameter PathInfoList
        ///</Summary>
        public PathInfoList[][] PathInfoList { get; set; }
    }

    ///<Summary>
    /// Class PathInfoList
    ///</Summary>
    public partial class PathInfoList
    {
        ///<Summary>
        /// Parameter key
        ///</Summary>
        public string Key { get; set; }

        ///<Summary>
        /// Parameter Name
        ///</Summary>
        public string Name { get; set; }
    }

    ///<Summary>
    /// Class MyArray
    ///</Summary>
    public class MyArray
    {
        ///<Summary>
        /// Parameter Email
        ///</Summary>
        public string Email { get; set; }

        ///<Summary>
        /// Parameter DomainAccessInstance
        ///</Summary>
        public object DomainAccessInstance { get; set; }

        ///<Summary>
        /// Parameter EscrowCompany
        ///</Summary>
        public string EscrowCompany { get; set; }

        ///<Summary>
        /// Parameter EscrowOfficer
        ///</Summary>
        public string EscrowOfficer { get; set; }

        ///<Summary>
        /// Parameter EscrowContactEmail
        ///</Summary>
        public object EscrowContactEmail { get; set; }

        ///<Summary>
        /// Parameter EscrowNumber
        ///</Summary>
        public string EscrowNumber { get; set; }

        ///<Summary>
        /// Parameter Usertype
        ///</Summary>
        public string Usertype { get; set; }

        ///<Summary>
        /// Parameter EscrowOfficerPhoneNumber
        ///</Summary>
        public object EscrowOfficerPhoneNumber { get; set; }

        ///<Summary>
        /// Parameter UserId
        ///</Summary>
        public int UserId { get; set; }

        ///<Summary>
        /// Parameter UserFk
        ///</Summary>
        public object UserFk { get; set; }

        ///<Summary>
        /// Parameter Id
        ///</Summary>
        public int Id { get; set; }
    }

    ///<Summary>
    /// Class Rootx
    ///</Summary>
    public class Rootx
    {
        ///<Summary>
        /// Parameter myArray
        ///</Summary>
        public List<MyArray> myArray { get; set; }
    }

    ///<Summary>
    /// Class AccessToken
    ///</Summary>
    public class AccessToken
    {
        ///<Summary>
        /// Parameter access_token
        ///</Summary>
        public string access_token { get; set; }

        ///<Summary>
        /// Parameter token_type
        ///</Summary>
        public string token_type { get; set; }

        ///<Summary> 
        /// Parameter expires_in
        ///</Summary>
        public int expires_in { get; set; }
    }

    //public class TagCreationResult
    //{
    //    ///<Summary> 
    //    /// Parameter Success
    //    ///</Summary>
    //    public bool Success { get; set; }

    //    ///<Summary> 
    //    /// Parameter Message
    //    ///</Summary>
    //    public string Message { get; set; }
    //}

}