using Abp;
using Abp.Domain.Repositories;
using Abp.Notifications;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using MimeKit;
using MySqlConnector;
using Microsoft.AspNetCore.SignalR;
using SR.EscrowBaseWeb.Web.Chat.SignalR;
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
using SR.EscrowBaseWeb.Web.FilePermission;
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
using System.IO.Compression;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
//using NPOI.HWPF;
//using FreeSpire.Doc;
//using NPOI.HWPF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;

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
        private readonly IFilePermissionService _filePermissionService;


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
        private readonly IHubContext<ChatHub> _chatHub;
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
            IRepository<TagsAndFileMappings> tagsAndFileMappingsRepository,
            IHubContext<ChatHub> chatHub,
            IFilePermissionService filePermissionService
            )
        {
            _chatHub = chatHub;
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
            _filePermissionService = filePermissionService;
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

        [HttpPost]
        public async Task<IActionResult> DownloadZip([FromBody] DownloadZipInput input)
        {
            try
            {
                if (input == null || input.Files == null || !input.Files.Any())
                {
                    return BadRequest("No files selected.");
                }

                using (var memoryStream = new MemoryStream())
                {
                    using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var fileItem in input.Files)
                        {
                            try
                            {
                                string path = input.ParentPath.Replace("%23", "#");
                                string key = fileItem.Key.Replace("%23", "#");
                                // Logic to construct file path similar to DownloadFile
                                var folderName = Path.Combine(@"Common/Paperless/" + path);
                                folderName = folderName.Substring(0, folderName.LastIndexOf('/'));
                                folderName = Path.Combine(folderName + "/" + key);
                                
                                string newpath = folderName.Replace("/", "\\");
                                string filePath = Path.Combine(_hostingEnvironment.WebRootPath + "\\" + newpath);

                                if (System.IO.File.Exists(filePath))
                                {
                                    var entry = archive.CreateEntry(fileItem.Name); // Use the provided name for the entry
                                    using (var entryStream = entry.Open())
                                    using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                                    {
                                        await fileStream.CopyToAsync(entryStream);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                // Log error for individual file but continue zipping others
                                string logs = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");
                                if (!System.IO.File.Exists(logs))
                                {
                                    using (FileStream fs1 = new FileStream(logs, FileMode.OpenOrCreate, FileAccess.Write)) { }
                                }
                                using (StreamWriter writer = new StreamWriter(logs, true))
                                {
                                    writer.WriteLine($"Error zipping file {fileItem.Name}: {ex.ToString()} {DateTime.Now}");
                                }
                            }
                        }
                    }

                    memoryStream.Position = 0;
                    return File(memoryStream.ToArray(), "application/zip", "FailedDownload-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".zip");
                }
            }
            catch (Exception ex)
            {
                string logs = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");
                if (!System.IO.File.Exists(logs))
                {
                    using (FileStream fs1 = new FileStream(logs, FileMode.OpenOrCreate, FileAccess.Write)) { }
                }
                using (StreamWriter writer = new StreamWriter(logs, true))
                {
                    writer.WriteLine("Error in DownloadZip method: " + ex.ToString() + DateTime.Now.ToString());
                }
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public IActionResult DownloadDragDropExeFile()
        {
            var filePath = Path.Combine( _hostingEnvironment.WebRootPath,"Common", "Downloads", "EscrowDragSetup.exe" );
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            // Use PhysicalFile for efficient streaming directly from disk
            return PhysicalFile(filePath, "application/octet-stream", "EscrowDragSetup.exe");
        }

        public class DownloadZipInput
        {
            public string ParentPath { get; set; }
            public List<ZipFileItem> Files { get; set; }
            public long UserId { get; set; }
        }

        public class ZipFileItem
        {
            public string Key { get; set; }
            public string Name { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> ConvertFileToBase64(string path, string key)
        {
            try
            {
                path = path.Replace("%23", "#");
                key = key.Replace("%23", "#");
                string cleanPath = path.Replace("/", Path.DirectorySeparatorChar.ToString()).TrimEnd(Path.DirectorySeparatorChar);
                string cleanKey = key.Replace("/", Path.DirectorySeparatorChar.ToString());
                string relativePath = Path.Combine("Common", "Paperless", cleanPath, cleanKey);
                string file = Path.Combine(_hostingEnvironment.WebRootPath, relativePath);

                if (Path.GetExtension(file).ToLower() == ".doc" || Path.GetExtension(file).ToLower() == ".docx" || Path.GetExtension(file).ToLower() == ".rtf")
                {
                    try
                    {
                        if (!System.IO.File.Exists(file))
                        {
                            return NotFound(new { error = "File not found" });
                        }
                        Document document = new Document();
                        document.LoadFromFile(file);
                        document.HtmlExportOptions.ImageEmbedded = true;
                        document.HtmlExportOptions.CssStyleSheetType = CssStyleSheetType.Internal;

                        string tempHtml = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".html");
                        try 
                        {
                            document.SaveToFile(tempHtml, FileFormat.Html);
                            string htmlContent = await System.IO.File.ReadAllTextAsync(tempHtml);
                            
                            string evaluationWarning = "Evaluation Warning: The document was created with Spire.Doc for .NET.";
                            htmlContent = htmlContent.Replace(evaluationWarning, string.Empty);

                            byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(htmlContent);
                            string base64String = Convert.ToBase64String(textBytes);
                            return Ok(new { Base64 = base64String, fileType = "html_edit" });
                        }
                        finally 
                        {
                            if (System.IO.File.Exists(tempHtml)) System.IO.File.Delete(tempHtml);
                        }
                    }
                    catch (Exception ex)
                    {
                        string logs = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");
                        try {
                            if (!Directory.Exists(Path.GetDirectoryName(logs))) Directory.CreateDirectory(Path.GetDirectoryName(logs));
                            System.IO.File.AppendAllText(logs, "\nError in ConvertFileToBase64 (.doc): " + ex.ToString());
                        } catch {}
                        return StatusCode(500, new { error = ex.Message, detail = ex.ToString() });
                    }
                }

                if (Path.GetExtension(file).Equals(".eml", StringComparison.OrdinalIgnoreCase))
                {
                    using var stream = new FileStream(file, FileMode.Open, FileAccess.Read);
                    var message = await MimeMessage.LoadAsync(stream);
                    string emailContent = message.HtmlBody ?? message.TextBody;
                    return Ok(new { Base64 = emailContent, fileType = "eml" });
                }

                if (Path.GetExtension(file).ToLower() == ".pdf")
                {
                    using var pdfStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                    using var memory = new MemoryStream();
                    await pdfStream.CopyToAsync(memory);
                    string base64String = Convert.ToBase64String(memory.ToArray());
                    return Ok(new { Base64 = base64String, fileType = "pdf" });
                }

                if (Path.GetExtension(file).ToLower() == ".txt")
                {
                    var textContent = await System.IO.File.ReadAllTextAsync(file);
                    byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(textContent);
                    string base64String = Convert.ToBase64String(textBytes);
                    return Ok(new { Base64 = base64String, fileType = "docx" });
                }

                if (Path.GetExtension(file).ToLower() == ".msg")
                {
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    using var fileStream = System.IO.File.OpenRead(file);
                    var reader = new MsgReader.Outlook.Storage.Message(fileStream);
                    string emailContent = reader.BodyHtml ?? reader.BodyText ?? "No content available";
                    byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(emailContent);
                    string base64String = Convert.ToBase64String(textBytes);
                    return Ok(new { Base64 = base64String, fileType = "msg" });
                }

                if (Path.GetExtension(file).ToLower() == ".xlsx" || Path.GetExtension(file).ToLower() == ".xls")
                {
                    try
                    {
                        IWorkbook workbook;
                        using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
                        {
                            if (Path.GetExtension(file).ToLower() == ".xlsx") workbook = new XSSFWorkbook(fileStream);
                            else workbook = new HSSFWorkbook(fileStream);
                        }

                        ISheet sheet = workbook.GetSheetAt(0);
                        StringBuilder htmlBuilder = new StringBuilder();
                        htmlBuilder.Append("<table border='1' style='border-collapse: collapse; width: 100%;'>");

                        for (int i = 0; i <= sheet.LastRowNum; i++)
                        {
                            IRow row = sheet.GetRow(i);
                            if (row != null)
                            {
                                htmlBuilder.Append("<tr>");
                                for (int j = 0; j < row.LastCellNum; j++)
                                {
                                    ICell cell = row.GetCell(j);
                                    string cellValue = cell?.ToString() ?? "";
                                    htmlBuilder.Append($"<td style='padding: 5px;'>{cellValue}</td>");
                                }
                                htmlBuilder.Append("</tr>");
                            }
                        }
                        htmlBuilder.Append("</table>");

                        byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(htmlBuilder.ToString());
                        string base64String = Convert.ToBase64String(textBytes);
                        return Ok(new { Base64 = base64String, fileType = "msg" });
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, new { error = ex.Message });
                    }
                }
                
                return NotFound();
            }
            catch (Exception ex)
            {
                string logs = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");
                try {
                if (!Directory.Exists(Path.GetDirectoryName(logs))) Directory.CreateDirectory(Path.GetDirectoryName(logs));
                System.IO.File.AppendAllText(logs, "\nFatal Error in ConvertFileToBase64: " + ex.ToString());
                } catch {}
                return NotFound();
            }
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


        [HttpPost]
        [Abp.Web.Models.DontWrapResult]
        public async Task<IActionResult> SaveDocument([FromBody] SaveDocRequestDto input)
        {
            try
            {
                if (input == null || string.IsNullOrEmpty(input.filePath) || string.IsNullOrEmpty(input.base64Content))
                {
                    return BadRequest("Invalid save request.");
                }

                string relativePath = input.filePath.Replace("/", "\\");
                string fullPath = Path.Combine(_hostingEnvironment.WebRootPath, "Common", "Paperless", relativePath);

                if (!System.IO.File.Exists(fullPath))
                {
                    return NotFound("File not found on server.");
                }

                // If content is HTML (from Quill), convert back to original format
                string ext = Path.GetExtension(fullPath).ToLower();
                if (ext == ".doc" || ext == ".docx" || ext == ".rtf")
                {
                    Document document = new Document();
                    
                    // The base64Content from Quill is just the HTML string encoded in base64
                    byte[] htmlBytes = Convert.FromBase64String(input.base64Content);
                    string htmlContent = Encoding.UTF8.GetString(htmlBytes);

                    using (var ms = new MemoryStream(htmlBytes))
                    {
                        document.LoadFromStream(ms, FileFormat.Html);
                        FileFormat targetFormat = ext == ".doc" ? FileFormat.Doc : (ext == ".rtf" ? FileFormat.Rtf : FileFormat.Docx);
                        document.SaveToFile(fullPath, targetFormat);
                    }
                }
                else
                {
                    // For other formats (like txt), just save the raw bytes
                    byte[] fileBytes = Convert.FromBase64String(input.base64Content);
                    await System.IO.File.WriteAllBytesAsync(fullPath, fileBytes);
                }

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error saving document: " + ex.Message);
            }
        }

        public class SaveDocRequestDto
        {
            public string fileName { get; set; }
            public string base64Content { get; set; }
            public string filePath { get; set; }
        }

        ///<Summary>
        /// Delete files
        ///</Summary>
        public async Task<responseBack> DeleteFile(string path, string key, string userType = null)
        {
            try
            {
                responseBack res = new responseBack();
                if (!string.IsNullOrEmpty(path))
                {
                    path = path.Replace("%23", "#");
                }
                if (!string.IsNullOrEmpty(key))
                {
                    key = key.Replace("%23", "#");
                }
                
                string folderName = string.Empty;
                if (!string.IsNullOrEmpty(path))
                {
                    folderName = Path.Combine(@"Common/Paperless/", path);
                    folderName = folderName.Replace("\\", "/");
                }

                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(folderName))
                {
                    int lastSlash = folderName.LastIndexOf('/');
                    if (lastSlash >= 0)
                    {
                        folderName = folderName.Substring(0, lastSlash) + "/" + key;
                    }
                    else
                    {
                        folderName = folderName + "/" + key;
                    }
                }

                if (!string.IsNullOrEmpty(folderName))
                {
                    string newpath = folderName.Replace("/", "\\");
                    string file = Path.Combine(_hostingEnvironment.WebRootPath, newpath);
                    
                    string shortFileName = Path.GetFileName(file);
                    
                    // ── CRITICAL FIX: Resolve the actual physical file path ──
                    // The frontend often sends a full URL (e.g., https://host/FileManager/filename.pdf)
                    // as the 'path' parameter, making the constructed 'file' path completely wrong.
                    // We MUST resolve the real path from the database using the short filename.
                    var master = _srEscrowFileMasterRepository.GetAll().FirstOrDefault(x => x.FileShortName == shortFileName);
                    if (master == null && !string.IsNullOrEmpty(shortFileName))
                    {
                        // The frontend might send a stale filename without tags (e.g., 'file.txt' instead of 'file~{EOX}.txt').
                        // 'Contains(shortFileName)' fails because the tag is inserted before the extension.
                        // We must search using the base filename without the extension.
                        string baseFileName = Path.GetFileNameWithoutExtension(shortFileName);
                        master = _srEscrowFileMasterRepository.GetAll().FirstOrDefault(x => x.FileShortName.StartsWith(baseFileName) || x.FileFullName.Contains(baseFileName));
                    }
                    
                    if (master != null && !string.IsNullOrEmpty(master.FileFullName))
                    {
                        // Use the database-stored physical path instead of the garbage URL-based path
                        file = master.FileFullName;
                        
                        // Wait! The master record might have an OUTDATED FileFullName if the file was assigned to users
                        // and renamed (e.g. ~{BR1-READ}.pdf) but the master record wasn't updated.
                        // We must check if the file actually exists at master.FileFullName.
                        if (!System.IO.File.Exists(file))
                        {
                            // It doesn't exist! Let's check SrFileMappings to see if we have a more recent physical path
                            var latestMapping = _srfilemapRepository.GetAll()
                                .Where(x => x.SrEscrowFileMasterId == master.Id && !string.IsNullOrEmpty(x.FileName))
                                .OrderByDescending(x => x.Id)
                                .FirstOrDefault();
                                
                            if (latestMapping != null && System.IO.File.Exists(latestMapping.FileName))
                            {
                                file = latestMapping.FileName;
                            }
                        }
                        
                        shortFileName = Path.GetFileName(file);
                    }
                    else if (!System.IO.File.Exists(file))
                    {
                        // Last resort: search the filesystem for this filename under the escrow folders
                        try
                        {
                            string rootPath = Path.Combine(_hostingEnvironment.WebRootPath, "Common", "Paperless");
                            if (Directory.Exists(rootPath))
                            {
                                string[] matchedFiles = Directory.GetFiles(rootPath, shortFileName, SearchOption.AllDirectories);
                                if (matchedFiles.Length == 1)
                                {
                                    file = matchedFiles[0];
                                    shortFileName = Path.GetFileName(file);
                                }
                                else if (matchedFiles.Length > 1)
                                {
                                    // If multiple matches, try to pick the one in an "Other" folder
                                    var otherMatch = matchedFiles.FirstOrDefault(f => f.IndexOf("\\Other\\", StringComparison.OrdinalIgnoreCase) >= 0);
                                    if (otherMatch != null)
                                    {
                                        file = otherMatch;
                                        shortFileName = Path.GetFileName(file);
                                    }
                                    else
                                    {
                                        file = matchedFiles[0];
                                        shortFileName = Path.GetFileName(file);
                                    }
                                }
                            }
                        }
                        catch { /* Filesystem search failed, continue with original path */ }
                    }
                    // ── END CRITICAL FIX ──
                    
                    // Reconstruct folderName from the resolved file path for downstream logic
                    string webRoot = _hostingEnvironment.WebRootPath;
                    if (file.StartsWith(webRoot, StringComparison.OrdinalIgnoreCase))
                    {
                        folderName = file.Substring(webRoot.Length).Replace("\\", "/");
                        if (folderName.StartsWith("/")) folderName = folderName.Substring(1);
                    }
                    
                    var assignedFiles = _srAssignedFilesDetailRepository.GetAll().Where(x => x.FileName == shortFileName).ToList();
                    bool isGlobalAdmin = AbpSession.UserId.HasValue && AbpSession.UserId.Value == 1;
                    
                    bool isOtherArea = file.IndexOf("\\Other\\", StringComparison.OrdinalIgnoreCase) >= 0 || file.IndexOf("/Other/", StringComparison.OrdinalIgnoreCase) >= 0;
                    bool hasTilde = shortFileName.IndexOf("~") > 0;

                    long currentUserId = AbpSession.UserId ?? 0;
                    if (string.IsNullOrEmpty(userType)) {
                        string[] parts = folderName.Split('/');
                        if (parts.Length >= 5) {
                            string company = parts[2];
                            string escrow = parts[4];
                            var userMap = _srfilemapRepository.GetAll().FirstOrDefault(x => x.UserId == currentUserId && x.FileName.Contains(company) && x.FileName.Contains(escrow) && x.Action != "READ");
                            if (userMap != null) {
                                userType = userMap.Action.Replace("{", "").Replace("}", "");
                                int dashIdx = userType.IndexOf("-");
                                if (dashIdx > 0) userType = userType.Substring(0, dashIdx);
                            }
                        }
                        // Fallback: if userType is still empty, try to get it from any mapping for this user
                        if (string.IsNullOrEmpty(userType)) {
                            var anyUserMap = _srfilemapRepository.GetAll().FirstOrDefault(x => x.UserId == currentUserId && x.Action != "READ");
                            if (anyUserMap != null) {
                                userType = anyUserMap.Action.Replace("{", "").Replace("}", "");
                                int dashIdx = userType.IndexOf("-");
                                if (dashIdx > 0) userType = userType.Substring(0, dashIdx);
                            }
                        }
                    }

                    bool isEscrowOfficer = !string.IsNullOrEmpty(userType) && (userType.StartsWith("EO", StringComparison.OrdinalIgnoreCase) || userType.StartsWith("EA", StringComparison.OrdinalIgnoreCase));
                    
                    SrFileMapping trueUploaderMapping = null;
                    if (master != null) 
                    {
                        var allMappings = _srfilemapRepository.GetAll().Where(x => x.SrEscrowFileMasterId == master.Id && x.Action == "READ").ToList();
                        // The original uploader's mapping is always created first, so it has the lowest Id.
                        trueUploaderMapping = allMappings.OrderBy(x => x.Id).FirstOrDefault();
                    }
                    if (trueUploaderMapping == null) 
                    {
                        // Fallback: try matching by the resolved file path
                        trueUploaderMapping = _srfilemapRepository.GetAll().FirstOrDefault(x => x.FileName == file && x.Action == "READ");
                    }
                    if (trueUploaderMapping == null && !string.IsNullOrEmpty(shortFileName))
                    {
                        // Fallback: try matching by short filename
                        trueUploaderMapping = _srfilemapRepository.GetAll().Where(x => x.FileName.Contains(shortFileName) && x.Action == "READ").OrderBy(x => x.Id).FirstOrDefault();
                    }

                    var uploaderMapping = trueUploaderMapping;
                    bool isUploader = uploaderMapping != null && uploaderMapping.UserId == AbpSession.UserId;

                    if (!string.IsNullOrEmpty(userType) && !string.IsNullOrEmpty(shortFileName))
                    {
                        if (shortFileName.Contains("{" + userType + "-READ}", StringComparison.OrdinalIgnoreCase) || 
                            shortFileName.Contains("{" + userType + "-READS}", StringComparison.OrdinalIgnoreCase))
                        {
                            isUploader = true;
                        }
                    }

                    bool isAssigned = isOtherArea ? hasTilde : assignedFiles.Count > 0;
                    
                    // ── SIMPLIFIED DELETE LOGIC ──
                    // Rule 1: Admin can always delete
                    // Rule 2: EOX deleting someone else's upload → strip assignments, preserve for uploader
                    // Rule 3: Assigned user (SR1, etc.) deleting → strip THEIR tag only
                    // Rule 4: Everyone else (including uploader) → physically delete the file
                    
                    // Fetch the uploader's role early to determine if they are an EOX
                    string uploaderRole = "";
                    if (uploaderMapping != null) {
                        string escrowId = "";
                        string[] folderParts = folderName.Split('/');
                        if (folderParts.Length >= 5)
                        {
                            escrowId = folderParts[4];
                        }
                        
                        var uploaderUser = _userRepository.FirstOrDefault(uploaderMapping.UserId);
                        if (uploaderUser != null && !string.IsNullOrEmpty(escrowId)) {
                            var uploaderEd = _escrowDetailRepository.GetAll().FirstOrDefault(x => x.EscrowId == escrowId && x.Email == uploaderUser.EmailAddress);
                            if (uploaderEd != null && !string.IsNullOrEmpty(uploaderEd.Usertype)) {
                                uploaderRole = uploaderEd.Usertype;
                            }
                        }
                        
                        if (string.IsNullOrEmpty(uploaderRole)) {
                            var uploaderRoleMap = _srfilemapRepository.GetAll().FirstOrDefault(x => x.UserId == uploaderMapping.UserId && x.Action != "READ");
                            if (uploaderRoleMap != null) {
                                uploaderRole = uploaderRoleMap.Action.Replace("{", "").Replace("}", "");
                                int uDash = uploaderRole.IndexOf("-");
                                if (uDash > 0) uploaderRole = uploaderRole.Substring(0, uDash);
                            }
                        }
                    }
                    bool uploaderIsEox = !string.IsNullOrEmpty(uploaderRole) && (uploaderRole.StartsWith("EO", StringComparison.OrdinalIgnoreCase) || uploaderRole.StartsWith("EA", StringComparison.OrdinalIgnoreCase));

                    if (isEscrowOfficer && uploaderMapping != null && uploaderMapping.UserId != AbpSession.UserId && !isOtherArea && !isGlobalAdmin && !uploaderIsEox)
                    {
                        // EOX is deleting a file uploaded by a REGULAR user (BR/SR) → preserve for uploader
                        foreach (var assignedFile in assignedFiles)
                        {
                            _srAssignedFilesDetailRepository.Delete(assignedFile);
                        }
                        
                        int tildeIdx = shortFileName.IndexOf("~");
                        if (tildeIdx > 0) 
                        {
                            string ext = Path.GetExtension(shortFileName);
                            string baseName = shortFileName.Substring(0, tildeIdx);
                            string tags = shortFileName.Substring(tildeIdx + 1).Replace(ext, "");
                            
                            string newTags = "";
                            
                            int i = 0;
                            while (i < tags.Length) {
                                int start = tags.IndexOf('{', i);
                                if (start == -1) break;
                                int end = tags.IndexOf('}', start);
                                if (end == -1) break;
                                string tag = tags.Substring(start, end - start + 1);
                                
                                if (!string.IsNullOrEmpty(uploaderRole) && tag.StartsWith("{" + uploaderRole)) {
                                    newTags += tag;
                                }
                                
                                i = end + 1;
                            }

                            string newShortName = baseName + (string.IsNullOrEmpty(newTags) ? "" : "~" + newTags) + ext;
                            string newFileFullPath = Path.Combine(Path.GetDirectoryName(file), newShortName);
                            
                            if (System.IO.File.Exists(file) && file != newFileFullPath) 
                            {
                                System.IO.File.Move(file, newFileFullPath);
                            }
                            
                            if (uploaderMapping != null) 
                            {
                                uploaderMapping.FileName = newFileFullPath;
                                _srfilemapRepository.Update(uploaderMapping);
                            }
                            
                            // Update master record
                            if (master != null) 
                            {
                                master.FileFullName = newFileFullPath;
                                master.FileShortName = newShortName;
                                _srEscrowFileMasterRepository.Update(master);
                            }
                        }
                        
                        res.message = "File assignments cleared and returned to uploader.";
                        res.Success = true;
                        try { await _chatHub.Clients.All.SendAsync("getFileUploadMessage", new { refreshOnly = true }); } catch {}
                        return res;
                    }
                    else if (!isEscrowOfficer && !isGlobalAdmin)
                    {
                        // Any non-EOX, non-Admin user (BR1, SR1, etc.) deleting a file
                        // First, check if ANYONE ELSE has access to this file (via DB assignments or filename tags).
                        bool hasOtherAssignments = assignedFiles.Any(x => x.UserId != currentUserId);
                        bool hasOtherTags = false;
                        
                        int tildeIdx = shortFileName.IndexOf("~");
                        if (tildeIdx > 0) 
                        {
                            string ext = Path.GetExtension(shortFileName);
                            string tags = shortFileName.Substring(tildeIdx + 1).Replace(ext, "");
                            string userTag = "{" + userType;
                            int i = 0;
                            while (i < tags.Length) {
                                int start = tags.IndexOf('{', i);
                                if (start == -1) break;
                                int end = tags.IndexOf('}', start);
                                if (end == -1) break;
                                string tag = tags.Substring(start, end - start + 1);
                                if (!tag.StartsWith(userTag)) {
                                    hasOtherTags = true;
                                    break;
                                }
                                i = end + 1;
                            }
                        }
                        
                        bool othersHaveAccess = hasOtherAssignments || hasOtherTags;
                        
                        if (othersHaveAccess && !isUploader)
                        {
                            // OTHERS have access, and this user is NOT the uploader. 
                            // We cannot physically delete. We must HIDE it from this user.
                            string newShortName = shortFileName;
                            string newFileFullPath = file;
                            
                            if (tildeIdx > 0) 
                            {
                                string ext = Path.GetExtension(shortFileName);
                                string baseName = shortFileName.Substring(0, tildeIdx);
                                string tags = shortFileName.Substring(tildeIdx + 1).Replace(ext, "");
                                
                                string newTags = "";
                                int i = 0;
                                while (i < tags.Length) {
                                    int start = tags.IndexOf('{', i);
                                    if (start == -1) break;
                                    int end = tags.IndexOf('}', start);
                                    if (end == -1) break;
                                    string tag = tags.Substring(start, end - start + 1);
                                    if (!tag.StartsWith("{" + userType)) {
                                        newTags += tag;
                                    }
                                    i = end + 1;
                                }
    
                                newShortName = baseName + (string.IsNullOrEmpty(newTags) ? "" : "~" + newTags) + ext;
                                newFileFullPath = Path.Combine(Path.GetDirectoryName(file), newShortName);
                            }
                            
                            // Try to rename/move the physical file
                            try {
                                if (System.IO.File.Exists(file) && file != newFileFullPath) 
                                {
                                    System.IO.File.Move(file, newFileFullPath);
                                    
                                    // Update ALL remaining assignment records to point to the new filename
                                    var otherAssigned = assignedFiles.Where(x => x.UserId != currentUserId).ToList();
                                    foreach(var a in otherAssigned) {
                                        a.FileName = newShortName;
                                        _srAssignedFilesDetailRepository.Update(a);
                                    }
                                    
                                    // Update uploader mapping and master to new filename
                                    if (uploaderMapping != null && uploaderMapping.UserId != currentUserId) 
                                    {
                                        uploaderMapping.FileName = newFileFullPath;
                                        _srfilemapRepository.Update(uploaderMapping);
                                    }
                                    if (master != null) 
                                    {
                                        master.FileFullName = newFileFullPath;
                                        master.FileShortName = newShortName;
                                        _srEscrowFileMasterRepository.Update(master);
                                    }
                                }
                            } catch { /* File rename/move failed, but we still clean up DB below */ }
                            
                            // Clean up DB records for THIS user
                            var myAssignments = assignedFiles.Where(x => x.UserId == currentUserId).ToList();
                            foreach (var a in myAssignments)
                            {
                                _srAssignedFilesDetailRepository.Delete(a);
                            }
                            
                            var myReadMappings = _srfilemapRepository.GetAll()
                                .Where(x => x.UserId == currentUserId && x.Action == "READ" && 
                                       (x.FileName.Contains(shortFileName) || (master != null && x.SrEscrowFileMasterId == master.Id)))
                                .ToList();
                            foreach (var m in myReadMappings) {
                                _srfilemapRepository.Delete(m);
                            }
                            
                            if (isUploader && uploaderMapping != null) {
                                _srfilemapRepository.Delete(uploaderMapping);
                            }
    
                            res.message = "File removed from your view successfully.";
                            res.Success = true;
                            try { await _chatHub.Clients.All.SendAsync("getFileUploadMessage", new { refreshOnly = true }); } catch {}
                            return res;
                        }
                        
                        // If NO ONE ELSE has access, DO NOT return here. 
                        // Let it fall through to the DEFAULT physical delete below!
                    }
                    
                    // DEFAULT: For all other cases (uploader, EOX who uploaded, main area, unassigned files, etc.)
                    // → Physically delete the file

                    bool deletedPhysical = false;
                    if (System.IO.File.Exists(file))
                    {
                        System.IO.File.Delete(file);
                        res.message = "File deleted successfully. Path was: " + file; 
                        res.Success = true;
                        deletedPhysical = true;
                    } 
                    else 
                    {
                        if (file.IndexOf("\\Other\\", StringComparison.OrdinalIgnoreCase) > 0 || file.IndexOf("/Other/", StringComparison.OrdinalIgnoreCase) > 0)
                        {
                            try 
                            {
                                string rootPath = Path.Combine(_hostingEnvironment.WebRootPath, "Common", "Paperless");
                                string[] parts = path.Replace("\\", "/").Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                                if (parts.Length >= 3)
                                {
                                    string escrowPath = Path.Combine(rootPath, parts[0], parts[1], parts[2]);
                                    if (Directory.Exists(escrowPath))
                                    {
                                        string[] matchedFiles = Directory.GetFiles(escrowPath, key, SearchOption.AllDirectories);
                                        if (matchedFiles.Length == 1)
                                        {
                                            System.IO.File.Delete(matchedFiles[0]);
                                            res.message = "File deleted successfully via fallback search. Path was: " + matchedFiles[0];
                                            res.Success = true;
                                            deletedPhysical = true;
                                        }
                                        else if (matchedFiles.Length > 1)
                                        {
                                            res.message = "File NOT FOUND at: " + file + " (Multiple files found with the same name during fallback search)";
                                            res.Success = false;
                                        }
                                        else 
                                        {
                                            res.message = "File NOT FOUND at: " + file;
                                            res.Success = false;
                                        }
                                    }
                                }
                                else
                                {
                                    res.message = "File NOT FOUND at: " + file;
                                    res.Success = false;
                                }
                            }
                            catch (Exception ex)
                            {
                                res.message = "File NOT FOUND at: " + file + ". Fallback search failed: " + ex.Message;
                                res.Success = false;
                            }
                        }
                        else 
                        {
                            res.message = "File NOT FOUND at: " + file; 
                            res.Success = false;
                        }
                    }

                    if (deletedPhysical)
                    {
                        var filed = _srfilemapRepository.GetAll().Where(x => x.FileName == file).ToList();
                        if (filed != null)
                        {
                            foreach (var id in filed)
                            {
                                _srfilemapRepository.Delete(id);
                            }
                        }

                        var esignFile = _esignRepository.GetAll().Where(x => file.Contains(x.FullFilePath)).ToList();
                        if (esignFile.Count > 0)
                        {
                            foreach (var item in esignFile)
                            {
                                _esignRepository.Delete(item);
                            }
                        }

                        foreach (var assignedFile in assignedFiles)
                        {
                            _srAssignedFilesDetailRepository.Delete(assignedFile);
                        }

                        var masterFiles = _srEscrowFileMasterRepository.GetAll().Where(x => x.FileShortName == shortFileName).ToList();
                        foreach (var masterFile in masterFiles)
                        {
                            _srEscrowFileMasterRepository.Delete(masterFile);
                        }
                    }
                }
                if (res.Success) {
                    try { await _chatHub.Clients.All.SendAsync("getFileUploadMessage", new { refreshOnly = true }); } catch {}
                }
                return res;
            }
            catch (Exception ex)
            {
                string logs = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");
                if (!System.IO.File.Exists(logs))
                {
                    FileStream fs1 = new FileStream(logs, FileMode.OpenOrCreate, FileAccess.Write);
                    fs1.Close();
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
                                else
                                {
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

            var find = _esignRepository.GetAll().FirstOrDefault(x => x.FileName == filename);
            if (find == null || string.IsNullOrEmpty(find.ZohoAction))
                return zohosignInPopup;

            var esignCompanyCode = find.EsignCompanyCode.ToString();
            var EscrowDetails = _escrowDetailRepository.GetAll().Where(x => x.EscrowId == Escrow).ToList();
            var ZohoAction = JsonConvert.DeserializeObject<List<Action>>(find.ZohoAction);

            if (esignCompanyCode == "2001") 
            {
                var myDeserializedClass = await getSignStatus(find.RequestId);
                if (myDeserializedClass?.document_form_data?.actions != null)
                {
                    var signPercentage = myDeserializedClass.document_form_data.actions;

                    foreach (var item in ZohoAction)
                    {
                        EsignNameStatus zohosignIn = new EsignNameStatus
                        {
                            Email = item.recipient_email,
                            Name = item.recipient_name,
                            UserType = EscrowDetails.FirstOrDefault(x => x.Email == item.recipient_email)?.Usertype,
                            signing_order = item.signing_order,
                            TotalSignatureCount = item.fields.Count(x => x.field_type_name == "Signature"),
                            TotalinitialsCount = item.fields.Count(x => x.field_type_name == "Initial"),
                            TotalMandatorySignatureCount = item.fields.Count(x => x.field_type_name == "Signature" && x.is_mandatory),
                            TotalMandatoryInitialsCount = item.fields.Count(x => x.field_type_name == "Initial" && x.is_mandatory),
                        };

                        zohosignIn.TotalOptinalSignatureCount = zohosignIn.TotalSignatureCount - zohosignIn.TotalMandatorySignatureCount;
                        zohosignIn.TotalOptinalInitialsCount = zohosignIn.TotalinitialsCount - zohosignIn.TotalMandatoryInitialsCount;

                        var signData = signPercentage.FirstOrDefault(x => x.recipient_email == item.recipient_email);
                        zohosignIn.ZohoSignSignature = !string.IsNullOrWhiteSpace(signData?.signed_time);

                        zohosignInPopup.Add(zohosignIn);
                    }
                }
                else
                {
                    foreach (var item in ZohoAction)
                    {
                        EsignNameStatus zohosignIn = new EsignNameStatus
                        {
                            Email = item.recipient_email,
                            Name = item.recipient_name,
                            UserType = EscrowDetails.FirstOrDefault(x => x.Email == item.recipient_email)?.Usertype,
                            signing_order = item.signing_order,
                            TotalSignatureCount = item.fields.Count(x => x.field_type_name == "Signature"),
                            TotalinitialsCount = item.fields.Count(x => x.field_type_name == "Initial"),
                            TotalMandatorySignatureCount = item.fields.Count(x => x.field_type_name == "Signature" && x.is_mandatory),
                            TotalMandatoryInitialsCount = item.fields.Count(x => x.field_type_name == "Initial" && x.is_mandatory),
                        };

                        zohosignIn.TotalOptinalSignatureCount = zohosignIn.TotalSignatureCount - zohosignIn.TotalMandatorySignatureCount;
                        zohosignIn.TotalOptinalInitialsCount = zohosignIn.TotalinitialsCount - zohosignIn.TotalMandatoryInitialsCount;
                        zohosignIn.ZohoSignSignature = false;

                        zohosignInPopup.Add(zohosignIn);
                    }
                }
            }
            else if (esignCompanyCode == "3001")
            {
                var signerList = JsonConvert.DeserializeObject<List<DocuSigninUserMapping>>(find.ZohoAction ?? "[]");

                foreach (var signer in signerList)
                {
                    EsignNameStatus signDetail = new EsignNameStatus
                    {
                        Email = signer.recipientEmail,
                        Name = signer.recipientName,
                        UserType = EscrowDetails.FirstOrDefault(x => x.Email == signer.recipientEmail)?.Usertype,
                        signing_order = signer.signingOrder,

                        TotalSignatureCount = signer.totalSignatureCount,
                        TotalMandatorySignatureCount = signer.totalMandatorySignatureCount,
                        TotalinitialsCount = signer.totalInitialsCount,
                        TotalMandatoryInitialsCount = signer.totalMandatoryInitialsCount,

                        TotalOptinalSignatureCount = signer.totalSignatureCount - signer.totalMandatorySignatureCount,
                        TotalOptinalInitialsCount = signer.totalInitialsCount - signer.totalMandatoryInitialsCount,

                        ZohoSignSignature = signer.status == "completed"
                    };

                    zohosignInPopup.Add(signDetail);
                }
            }

            return zohosignInPopup;
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

        [HttpPost]
        [Microsoft.AspNetCore.Authorization.Authorize]
        [Abp.Web.Models.DontWrapResult]
        public async Task<responseBack> ProcessRequest(string path, string userId)
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

                string uploaderEmail = null;
                string cleanPath = path;
                string[] pathParts = path.Replace("\\", "/").Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (pathParts.Length > 0)
                {
                    string lastPart = pathParts[pathParts.Length - 1];
                    if (lastPart.Contains("@") && lastPart.Contains("."))
                    {
                        uploaderEmail = lastPart;
                        cleanPath = string.Join("/", pathParts.Take(pathParts.Length - 1));
                    }
                }

                long parsedUserId = 0;
                if (!string.IsNullOrEmpty(uploaderEmail))
                {
                    var userObj = _userRepository.GetAll().FirstOrDefault(x => x.EmailAddress == uploaderEmail);
                    if (userObj != null)
                    {
                        parsedUserId = userObj.Id;
                    }
                }
                
                if (parsedUserId == 0)
                {
                    long.TryParse(userId, out parsedUserId);
                }

                // Resolve company, subcompany, escrowId from the cleaned path
                string company = "";
                string subCompany = "";
                string escrowId = "";
                
                string[] cleanedParts = cleanPath.Replace("\\", "/").Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (cleanedParts.Length >= 3)
                {
                    company = cleanedParts[0];
                    subCompany = cleanedParts[1];
                    escrowId = cleanedParts[2];
                }

                string userRole = "";
                if (parsedUserId > 0 && !string.IsNullOrEmpty(escrowId))
                {
                    var userObj = _userRepository.FirstOrDefault(parsedUserId);
                    if (userObj != null)
                    {
                        var ed = _escrowDetailRepository.GetAll().FirstOrDefault(x => x.EscrowId == escrowId && x.Email == userObj.EmailAddress);
                        if (ed != null && !string.IsNullOrEmpty(ed.Usertype))
                        {
                            userRole = ed.Usertype.ToUpper();
                        }
                    }
                }

                string finalFileName = fileName;
                bool isOfficer = !string.IsNullOrEmpty(userRole) && (userRole.StartsWith("EO") || userRole.StartsWith("EA"));
                bool isAdmin = parsedUserId == 1;

                if (!isOfficer && !isAdmin && !string.IsNullOrEmpty(userRole))
                {
                    string ext = Path.GetExtension(fileName);
                    string baseName = Path.GetFileNameWithoutExtension(fileName);
                    int tildeIdx = baseName.IndexOf("~");
                    if (tildeIdx > 0) baseName = baseName.Substring(0, tildeIdx);

                    finalFileName = $"{baseName}~{{{userRole}-READ}}{{EOX-READ}}{{EAX-READ}}{ext}";
                }

                var folderPath = Path.Combine("wwwroot", "Common", "Paperless", cleanPath.Replace("/", "\\"));
                var fullSavePath = Path.Combine(Directory.GetCurrentDirectory(), folderPath);
                var fullFilePath = Path.Combine(fullSavePath, finalFileName);

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

                using (Stream stream = new FileStream(fullFilePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                if (parsedUserId != 1)
                {
                    if (cleanedParts.Length < 3)
                    {
                        res.message = "Invalid path format. Expected: Company/SubCompany/EscrowId/Other";
                        res.statusCode = 400;
                        return res;
                    }

                    // Logic to Create/Get Master File Record
                    long srEscrowFileMasterId = 0;
                    var dbSREscrowFileMaster = _srEscrowFileMasterRepository.GetAll()
                        .Where(x => x.FileFullName == fullFilePath)
                        .FirstOrDefault();

                    if (dbSREscrowFileMaster == null)
                    {
                        SREscrowFileMaster sREscrowFileMaster = new SREscrowFileMaster();
                        sREscrowFileMaster.FileFullName = fullFilePath; // Save full path
                        sREscrowFileMaster.FileShortName = finalFileName; // Save just filename
                        srEscrowFileMasterId = _srEscrowFileMasterRepository.InsertAndGetId(sREscrowFileMaster);

                        // Create History Record
                        CreateOrEditEscrowFileHistoryDto escrowFileHistory = new CreateOrEditEscrowFileHistoryDto();
                        escrowFileHistory.SrEscrowFileMasterId = srEscrowFileMasterId;
                        escrowFileHistory.FileFullPath = finalFileName;
                        escrowFileHistory.UserId = parsedUserId; 
                        escrowFileHistory.Message = FileConstant.ADD_File;
                        escrowFileHistory.ActionType = FileConstantAction.ADD_File;
                        await _escrowFileHistoriesAppService.CreateOrEdit(escrowFileHistory);
                    }
                    else
                    {
                        srEscrowFileMasterId = dbSREscrowFileMaster.Id;
                    }

                    var coesfm = new CreateOrEditSrFileMappingDto
                    {
                        UserId = Convert.ToInt32(parsedUserId),
                        FileName = fullFilePath,
                        IsActive = true,
                        EscrowiId = escrowId,
                        Action = "READ",
                        SrEscrowFileMasterId = srEscrowFileMasterId
                    };
                    
                    var existingMapping = _srfilemapRepository.GetAll()
                        .FirstOrDefault(x => x.UserId == coesfm.UserId && x.FileName == fullFilePath);
                    
                    if (existingMapping == null)
                    {
                         await _ISrFileMappingsAppService.CreateOrEdit(coesfm);
                    }

                    // Grant READ permission to EOX/EOA users so they can see the uploaded file
                    await _filePermissionService.GrantEoxEoaReadPermissionAsync(
                        escrowId, srEscrowFileMasterId, fullFilePath, Convert.ToInt32(parsedUserId));
                    
                    // Assign General tag if uploading to Other
                    if (cleanPath.Contains("\\Other") || cleanPath.Contains("/Other"))
                    {
                        var generalTag = _escrowFileTagsRepository.GetAll().FirstOrDefault(x => x.TagDescription.ToLower() == "general");
                        if (generalTag != null)
                        {
                            var cleanName = finalFileName;
                            int tildeIdx = finalFileName.IndexOf("~");
                            if (tildeIdx > 0)
                            {
                                string extPart = Path.GetExtension(finalFileName);
                                string baseName = finalFileName.Substring(0, tildeIdx);
                                if (baseName.EndsWith(extPart, StringComparison.OrdinalIgnoreCase))
                                {
                                    cleanName = baseName;
                                }
                                else
                                {
                                    cleanName = baseName + extPart;
                                }
                            }

                            var existingTag = _tagsAndFileMappingsRepository.GetAll()
                                .FirstOrDefault(x => x.FileName == cleanName && x.TagId == generalTag.Id);

                            if (existingTag == null)
                            {
                                _tagsAndFileMappingsRepository.Insert(new TagsAndFileMappings
                                {
                                    FileName = cleanName,
                                    TagId = generalTag.Id
                                });
                            }
                        }
                    }
                }

                await _chatHub.Clients.All.SendAsync("getFileUploadMessage", new { fileFullName = finalFileName });

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
        public object FileSystem(string company, string subCompany, string escrow, string userId, string usertype, string usersname)
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

            GetAllSrFileMappingsInput obj = new GetAllSrFileMappingsInput();
            obj.Filter = userId;
            var rootPath = Path.Combine(_hostingEnvironment.WebRootPath, SampleImagesRelativePath);
            var targetPath = Path.Combine(rootPath, company, subCompany, escrow);

            Root temp = new Root { success = true, result = new List<Result>() };
            if (Directory.Exists(targetPath))
            {
                var directoryInfo = new DirectoryInfo(targetPath);
                foreach (var dir in directoryInfo.GetDirectories())
                {
                    temp.result.Add(new Result
                    {
                        key = Path.Combine(company, subCompany, escrow, dir.Name),
                        name = dir.Name,
                        isDirectory = true,
                        hasSubDirectories = dir.GetDirectories().Any()
                    });
                }

                foreach (var file in directoryInfo.GetFiles())
                {
                    temp.result.Add(new Result
                    {
                        key = Path.Combine(company, subCompany, escrow, file.Name),
                        name = file.Name,
                        isDirectory = false,
                        size = (int)file.Length,
                        CustomFields = new Dictionary<string, object>
                        {
                            { "url", GetFileItemUrl(file) }
                        }
                    });
                }
            }

            var checkPermission = _srfilemapRepository.GetAll();
            var usrdetail = _userRepository.GetAll().Where(x => x.Id == Convert.ToInt64(userId)).FirstOrDefault();

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
                        if (lst.isDirectory)
                        {
                            continue;
                        }
                        if (lst.key.Contains("\\Other\\"))
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
                                 else if (usertype.StartsWith("RA") == true)
                                 {
                                     temp1 = checkPermission.Where(x => x.FileName.Contains(strBunch) && x.FileName.Contains(lst.name) && x.Action.Contains("RAX")).FirstOrDefault();
                                 }
                                 else if (usertype.StartsWith("RB") == true)
                                 {
                                     temp1 = checkPermission.Where(x => x.FileName.Contains(strBunch) && x.FileName.Contains(lst.name) && x.Action.Contains("RBX")).FirstOrDefault();
                                 }
                                 else if (usertype.StartsWith("EO") == true)
                                 {
                                     temp1 = checkPermission.Where(x => x.FileName.Contains(strBunch) && x.FileName.Contains(lst.name) && x.Action.Contains("EOX")).FirstOrDefault();
                                 }
                                 else if (usertype.StartsWith("EA") == true)
                                 {
                                     temp1 = checkPermission.Where(x => x.FileName.Contains(strBunch) && x.FileName.Contains(lst.name) && x.Action.Contains("EAX")).FirstOrDefault();
                                 }
                            }
                                if (temp1 == null)
                                {
                                    temp1 = checkPermission.Where(x => x.FileName.Contains(company) && x.FileName.Contains(subCompany) && x.FileName.Contains(escrow) && x.Action.Contains(usertype)).FirstOrDefault();
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
                                if (comp == true || usertype.StartsWith("EO") || usertype.StartsWith("EA"))
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
                                     else if (usertype.Contains("RA") && lst.name.Contains("RAX"))
                                     {
                                         isValidFileForUserType = true;
                                     }
                                     else if (usertype.Contains("RB") && lst.name.Contains("RBX"))
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

                                    var st = _srAssignedFilesDetailRepository
                                        .GetAll()
                                        .OrderByDescending(x => x.Id)
                                        .FirstOrDefault(x => x.FileName == lst.name.TrimEnd() && x.UserId == usrdetail.Id);

                                    var filefound = _srfilemapRepository.GetAll().ToList();
                                    string fileNameToSearch = st != null ? st.FileName : lst.name.TrimEnd();
                                    var selectedfile = filefound.Where(x => x.FileName.Contains(fileNameToSearch)).FirstOrDefault();
                                    var SRFileMasterId = _srEscrowFileMasterRepository.GetAll().Where(x => x.FileShortName == fileNameToSearch).FirstOrDefault();
                                    
                                    if (st == null)
                                    {
                                        res.srAssignedFileId = 0;
                                        res.signing = "Unsigned";
                                        res.status = "Not applicable";
                                        res.OtherAction = false;
                                        res.OtherActionNote = "";
                                        res.signStatus = "Unsigned";
                                        res.updateOn = "";
                                    }
                                    else
                                    {
                                        res.OtherAction = SRFileMasterId?.OtherAction == true ? true : false;
                                        res.OtherActionNote = SRFileMasterId?.OtherActionNote;
                                        res.srAssignedFileId = SRFileMasterId?.Id ?? 0;
                                        res.signStatus = st.SigningStatus;
                                        res.updateOn = st.UpdatedOn.ToString("MM/dd/yy HH:mm");
                                    }

                                    if (SRFileMasterId != null)
                                    {
                                        var lastUpdated = _escrowFileHistoryRepository.GetAll().Where(x => x.SrEscrowFileMasterId == SRFileMasterId.Id && x.ActionType != FileConstantAction.Download_File).OrderByDescending(x => x.CreatedAt).FirstOrDefault();
                                        if (lastUpdated != null)
                                        {
                                            res.updateOn = lastUpdated.CreatedAt.ToString("MM/dd/yy HH:mm");
                                        }
                                    }

                                    // Get signing details based on email and file name
                                    var data = GetSignDetailsFile(usrdetail.EmailAddress, lst.name.TrimEnd());
                                    var Signin_percentage = string.Empty;
                                    bool signed = false, unsigned = false, partialSigned = false, sent = false;

                                    if (data.Any())
                                    {
                                        signed = data.Any(d => d.Status == "Signed" || d.Status == "success");
                                        unsigned = data.Any(d => d.Status == "Unsigned");
                                        partialSigned = data.Any(d => d.Status == "Partially Signed");
                                        sent = data.Any(d => d.Status == "sent");
                                        // Get the signing percentage from the relevant status
                                        var status = data.FirstOrDefault(d => d.Status == "Signed" || d.Status == "Partially Signed" || d.Status == "sent" || d.Status == "success");
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
                                    else if (sent)
                                    {
                                        var percent = string.IsNullOrEmpty(Signin_percentage) ? "0" : Signin_percentage;
                                        res.signing = (percent == "0") ? "Unsigned" : $"Signed - {percent} %";
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
                                     if (lst.key.Contains(userType) || usertype.StartsWith("EO") || usertype.StartsWith("EA") || (lst.key.Contains("BRX") || lst.key.Contains("SRX") || lst.key.Contains("RAX") || lst.key.Contains("RBX") || lst.key.Contains("EOX") || lst.key.Contains("EAX")))
                                    {
                                        string tempAccess = string.Empty;
                                        
                                        // First, try to find the exact usertype tag, e.g. {SR1-READS}
                                        var exactMatch = System.Text.RegularExpressions.Regex.Match(lst.key, @"\{" + usertype + @"-([^}]+)\}");
                                        if (exactMatch.Success)
                                        {
                                            tempAccess = exactMatch.Groups[1].Value.Trim();
                                        }
                                        else
                                        {
                                            // Try wildcard match based on usertype prefix
                                            string wildcard = string.Empty;
                                            if (usertype.StartsWith("SR")) wildcard = "SRX";
                                            else if (usertype.StartsWith("BR")) wildcard = "BRX";
                                            else if (usertype.StartsWith("TC")) wildcard = "TCX";
                                            else if (usertype.StartsWith("RA")) wildcard = "RAX";
                                            else if (usertype.StartsWith("RB")) wildcard = "RBX";
                                            else if (usertype.StartsWith("EO")) wildcard = "EOX";
                                            else if (usertype.StartsWith("EA")) wildcard = "EAX";

                                            if (!string.IsNullOrEmpty(wildcard))
                                            {
                                                var wildcardMatch = System.Text.RegularExpressions.Regex.Match(lst.key, @"\{" + wildcard + @"-([^}]+)\}");
                                                if (wildcardMatch.Success)
                                                {
                                                    tempAccess = wildcardMatch.Groups[1].Value.Trim();
                                                }
                                            }
                                        }

                                        if (string.IsNullOrEmpty(tempAccess) && (usertype.StartsWith("EO") || usertype.StartsWith("EA")))
                                        {
                                            tempAccess = "READS";
                                        }

                                        res.access = tempAccess.Replace("-", "");

                                        // Determine action and status based on the signing status
                                        if (res.access.Contains("S"))
                                        {
                                            res.action = (usertype == "EOX" || usertype == "EAX")
                                                ? "No Action Required"
                                                : ((st != null && st.SigningStatus == "Signed")
                                                    ? "Completed"
                                                    : "Fill out and Electronically Sign");
                                            res.status = (st != null && st.SigningStatus == "Signed") ? "Signed Fully" : "Nobody signed yet";
                                        }
                                        else if (res.access.Contains("E"))
                                        {
                                            res.status = st != null ? st.InputStatus : "Unread";
                                            res.action = (st != null && st.InputStatus == "Input Completed") ? "Completed" : "Fill out";
                                        }
                                        else
                                        {
                                            res.action = "No action required";
                                            res.status = (st != null && st.ReadStatus == "Read") ? "Read by all" : "Read by none";
                                        }
                                    }

                                    res.name = lst.name.Substring(0, lst.name.IndexOf("~"));
                                }
                                else 
                                {
                                    res.name = lst.name;
                                }

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
        public object FileSystem1(string company, string subCompany, string escrow, string userId)
        {
            string rootPath = Path.Combine(_hostingEnvironment.WebRootPath, "Common", "Paperless");
            string escrowPath = Path.Combine(rootPath, company, subCompany, escrow);
            
            var allOtherFiles = new List<string>();
            
            if (Directory.Exists(escrowPath))
            {
                // 1. Files in Escrow\Other (original)
                string mainOther = Path.Combine(escrowPath, "Other");
                if (Directory.Exists(mainOther))
                {
                    allOtherFiles.AddRange(Directory.GetFiles(mainOther));
                }
            }
            
            var files = allOtherFiles.ToArray();
            var checkPermission = _srfilemapRepository.GetAll();

            int parsedUserId = 0;
            int.TryParse(userId, out parsedUserId);
            var usermap = checkPermission.FirstOrDefault(x => x.UserId == parsedUserId && x.FileName.Contains(company) && x.FileName.Contains(escrow) && x.Action != "READ");
            string currentUserType = usermap?.Action ?? "";
            int dashIdx = currentUserType.IndexOf("-");
            if (dashIdx > 0)
            {
                currentUserType = currentUserType.Substring(0, dashIdx);
            }
            currentUserType = currentUserType.Replace("{", "").Replace("}", "");

            var userObj = _userRepository.FirstOrDefault(parsedUserId);
            string userEmail = userObj?.EmailAddress ?? "";

            if (string.IsNullOrEmpty(currentUserType) || currentUserType == "REA" || currentUserType == "READ" || currentUserType == "READS")
            {
                var ed = _escrowDetailRepository.GetAll().FirstOrDefault(x => x.EscrowId == escrow && x.Email == userEmail);
                if (ed != null && !string.IsNullOrEmpty(ed.Usertype))
                {
                    currentUserType = ed.Usertype.ToUpper();
                }
                else
                {
                    var companyEd = _escrowDetailRepository.GetAll()
                        .FirstOrDefault(x => x.Company == company && x.Email == userEmail && x.Usertype != null && (x.Usertype.StartsWith("EO") || x.Usertype.StartsWith("EA")));
                    
                    if (companyEd != null)
                    {
                        currentUserType = companyEd.Usertype.ToUpper();
                    }
                }
            }

            List<Result> newFile = new List<Result>();

            foreach (var file in files)
{
                string fileName = Path.GetFileName(file);
                var permission = checkPermission.FirstOrDefault(x => x.FileName.Contains(company) && x.FileName.Contains(fileName) && x.FileName.Contains("Other") && x.Action == "READ");
                
                string relativeFilePath = file.Replace(rootPath + "\\", "").Replace("\\", "/");
                string parentFolderPath = relativeFilePath.Substring(0, relativeFilePath.LastIndexOf('/'));

                bool isAllowed = false;
                if (currentUserType.StartsWith("EO") || currentUserType.StartsWith("EA"))
                {
                    var enterprise = _enterpriseRepository.GetAll()
                        .FirstOrDefault(x => x.EnterpriseName == company || x.Subcompany == company);
                    
                    bool restrictToAssigned = enterprise?.RestrictToAssignedOfficer ?? false;
                    bool isEox = currentUserType.Equals("EOX", StringComparison.OrdinalIgnoreCase);
                    
                    if (restrictToAssigned && !isEox)
                    {
                        // Check if they are mapped in EscrowDetails for this escrow
                        bool isAssigned = _escrowDetailRepository.GetAll()
                            .Any(x => x.Email == userEmail && x.EscrowId == escrow);
                        
                        if (isAssigned)
                        {
                            isAllowed = true;
                        }
                        else
                        {
                            var srEscrow = _ISrEscrowRepository.GetAll()
                                .FirstOrDefault(x => x.SubCompanyName == company && x.EscrowNo == escrow);
                            
                            if (srEscrow != null && !string.IsNullOrEmpty(srEscrow.EOEmail) && srEscrow.EOEmail.Equals(userEmail, StringComparison.OrdinalIgnoreCase))
                            {
                                isAllowed = true;
                            }
                        }
                    }
                    else
                    {
                        isAllowed = true;
                    }
                }
                else
                {
                    // 1. Is it in their folder?
                    if (!string.IsNullOrEmpty(userEmail) && parentFolderPath.Contains(userEmail, StringComparison.OrdinalIgnoreCase))
                    {
                        isAllowed = true;
                    }
                    // 2. Or is it explicitly assigned to them via tags?
                    else if (fileName.Contains($"{{{currentUserType}}}") || fileName.Contains($"{{{currentUserType}-"))
                    {
                        isAllowed = true;
                    }
                    // 3. Or is it in the shared Other folder AND has no tags?
                    else if (parentFolderPath.EndsWith(escrow + "/Other", StringComparison.OrdinalIgnoreCase) && !fileName.Contains("{"))
                    {
                        isAllowed = true;
                    }
                }

                if (!isAllowed)
                    continue;

                Result res = new Result();
                res.name = fileName;
                
                string uploaderRole = "";
                int endIdx = fileName.IndexOf("-READ}");
                if (endIdx < 0) endIdx = fileName.IndexOf("-READS}");
                
                if (endIdx > 0) 
                {
                    int startIdx = fileName.LastIndexOf("{", endIdx);
                    if (startIdx >= 0) 
                    {
                        uploaderRole = fileName.Substring(startIdx + 1, endIdx - startIdx - 1);
                    }
                }

                if (string.IsNullOrEmpty(uploaderRole))
                {
                    var uploaderMapping = checkPermission.FirstOrDefault(x => x.FileName.Contains(company) && x.FileName.Contains(fileName) && x.FileName.Contains("Other") && x.Action == "READ");
                    if (uploaderMapping != null) {
                        var uploaderUser = _userRepository.FirstOrDefault(uploaderMapping.UserId);
                        if (uploaderUser != null) {
                            var uploaderEd = _escrowDetailRepository.GetAll().FirstOrDefault(x => x.EscrowId == escrow && x.Email == uploaderUser.EmailAddress);
                            if (uploaderEd != null && !string.IsNullOrEmpty(uploaderEd.Usertype)) {
                                uploaderRole = uploaderEd.Usertype;
                            }
                        }
                        
                        if (string.IsNullOrEmpty(uploaderRole)) {
                            var uploaderRoleMap = checkPermission.FirstOrDefault(x => x.UserId == uploaderMapping.UserId && x.FileName.Contains(company) && x.FileName.Contains(escrow) && x.Action != "READ");
                            if (uploaderRoleMap != null) {
                                uploaderRole = uploaderRoleMap.Action; 
                                int uDash = uploaderRole.IndexOf("-");
                                if (uDash > 0) uploaderRole = uploaderRole.Substring(0, uDash);
                                uploaderRole = uploaderRole.Replace("{", "").Replace("}", "");
                            }
                        }
                    }
                }
                res.uploaderRole = uploaderRole;

                int tildeIdx = fileName.IndexOf("~");
                if (tildeIdx > 0)
                {
                    string extPart = Path.GetExtension(fileName);
                    string baseName = fileName.Substring(0, tildeIdx);
                    if (baseName.EndsWith(extPart, StringComparison.OrdinalIgnoreCase))
                    {
                        res.name = baseName;
                    }
                    else
                    {
                        res.name = baseName + extPart;
                    }
                }

                res.key = fileName;
                res.parentPath = parentFolderPath;
                
                // Preserve the access tag {TAGS} for the UI 'Assign Users' modal
                if (fileName.Contains("{") && fileName.Contains("}"))
                {
                    int start = fileName.IndexOf("{");
                    int end = fileName.IndexOf("}") + 1;
                    if (start >= 0 && end > start)
                    {
                        res.access = fileName.Substring(start, end - start);
                    }
                    else
                    {
                        res.access = "";
                    }
                }
                else
                {
                    res.access = "";
                }

                string ext = Path.GetExtension(file).ToLowerInvariant();
                res.fileType = ext switch
                {
                    ".pdf" => "/Images/pdf-file.png",
                    ".docx" => "/Images/docx-file.png",
                    ".doc" => "/Images/doc-file.png",
                    ".rtf" => "/Images/rtf-file.png",
                    ".txt" => "/Images/txt-file.png",
                    ".eml" => "/Images/eml-file.png",
                    ".msg" => "/Images/msg-file.png",
                    _ => ""
                };

                var mappingTags = _tagsAndFileMappingsRepository
                    .GetAll()
                    .Where(x => x.FileName == res.name)
                    .ToList();
                var tagIds = mappingTags.Select(x => x.TagId).ToList();
                var tags = _escrowFileTagsRepository
                    .GetAll()
                    .Where(x => tagIds.Contains(x.Id))
                    .ToList();

                if (tags.Any())
                    res.escrowFileTags = tags;

                res.srAssignedFileId = permission?.SrEscrowFileMasterId ?? 0;
                newFile.Add(res);
            }

            return newFile;
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


        ///<Summary>
        /// Get User Context (Companies and Escrows) for Sync App
        ///</Summary>
        [HttpGet]
        [Abp.Web.Models.DontWrapResult]
        public List<UserCompanyDto> GetUserContext(string username)
        {
            try
            {
                List<UserCompanyDto> getComp = new List<UserCompanyDto>();
                
                // First try matching by email directly
                var escrowDetails = _escrowDetailRepository.GetAll()
                    .Where(x => x.Email == username)
                    .ToList();

                // If no match by email, try to resolve username → email via Users table
                if (escrowDetails.Count == 0)
                {
                    var user = _userRepository.GetAll()
                        .FirstOrDefault(x => x.UserName == username || x.EmailAddress == username);
                    if (user != null && !string.IsNullOrEmpty(user.EmailAddress))
                    {
                        escrowDetails = _escrowDetailRepository.GetAll()
                            .Where(x => x.Email == user.EmailAddress)
                            .ToList();
                    }
                }

                foreach (var detail in escrowDetails)
                {
                    
                    var srEscrow = _ISrEscrowRepository.GetAll()
                        .Where(x => x.SubCompanyName == detail.Company && x.EscrowNo == detail.EscrowId)
                        .ToList();

                    foreach (var escrow in srEscrow)
                    {
                        var company = _enterpriseRepository.GetAll()
                            .Where(x => x.Id == escrow.EnterpriseId)
                            .FirstOrDefault();

                        if (company != null)
                        {
                            UserCompanyDto userCompany = new UserCompanyDto();
                            userCompany.address = escrow.PropertyAddress;
                            userCompany.company = company.EnterpriseName;
                            userCompany.subCompany = escrow.SubCompanyName;
                            userCompany.escrowId = escrow.EscrowNo;
                            userCompany.type = detail.Usertype;
                            
                            getComp.Add(userCompany);
                        }
                    }
                }
                return getComp;
            }
            catch (Exception ex)
            {
                // Log(ex);
                return null;
            }
        }

        /// <Summary>
        /// List all files in the Other folder for sync (server → local)
        /// </Summary>
        [HttpGet]
        [Abp.Web.Models.DontWrapResult]
        public List<SyncFileInfoDto> ListSyncFiles(string company, string subCompany, string escrowId)
        {
            try
            {
                var otherPath = Path.Combine(_hostingEnvironment.WebRootPath, "Common", "Paperless",
                    company, subCompany, escrowId, "Other");

                if (!Directory.Exists(otherPath))
                    return new List<SyncFileInfoDto>();

                var files = Directory.GetFiles(otherPath, "*", SearchOption.TopDirectoryOnly);
                var result = new List<SyncFileInfoDto>();

                foreach (var file in files)
                {
                    var fi = new FileInfo(file);
                    result.Add(new SyncFileInfoDto
                    {
                        fileName = fi.Name,
                        size = fi.Length,
                        lastModified = fi.LastWriteTimeUtc.ToString("o")
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                return new List<SyncFileInfoDto>();
            }
        }

        /// <Summary>
        /// Download a single file from the Other folder for sync
        /// </Summary>
        [HttpGet]
        [Abp.Web.Models.DontWrapResult]
        public IActionResult DownloadSyncFile(string company, string subCompany, string escrowId, string fileName)
        {
            try
            {
                // Sanitize fileName to prevent directory traversal
                fileName = Path.GetFileName(fileName);

                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "Common", "Paperless",
                    company, subCompany, escrowId, "Other", fileName);

                if (!System.IO.File.Exists(filePath))
                    return NotFound("File not found on server.");

                var memory = new MemoryStream();
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    stream.CopyTo(memory);
                }
                memory.Position = 0;

                var ext = Path.GetExtension(filePath).ToLowerInvariant();
                var mimeType = GetMimeTypes().GetValueOrDefault(ext, "application/octet-stream");
                return File(memory, mimeType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error downloading file: " + ex.Message);
            }
        }

    }

    public class UserCompanyDto
    {
        public string escrowId { get; set; }
        public string type { get; set; }
        public string company { get; set; }
        public string subCompany { get; set; }
        public string address { get; set; }
    }

    public class SyncFileInfoDto
    {
        public string fileName { get; set; }
        public long size { get; set; }
        public string lastModified { get; set; }
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
        public string uploaderRole { get; set; }

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

        public string parentPath { get; set; }

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

        public Dictionary<string, object> CustomFields { get; set; }

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


