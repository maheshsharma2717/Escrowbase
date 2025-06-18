using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using SR.EscrowBaseWeb.Invitee;
using SR.EscrowBaseWeb.SREnterprise;
using SR.EscrowBaseWeb.SREscrowClient;
using SR.EscrowBaseWeb.SREscrowClient.Dtos;

namespace SR.EscrowBaseWeb.Web.Controllers
{
    public class UploadFileFolderController : EscrowBaseWebControllerBase
    {


        private readonly IRepository<EscrowClient> _escrowClientRepository;
        private readonly IRepository<Enterprise, int> _lookup_enterpriseRepository;
        private readonly IRepository<SRInvitee> _srInviteeRepository;


        public UploadFileFolderController(IRepository<EscrowClient> escrowClientRepository, IRepository<Enterprise, int> lookup_enterpriseRepository, IRepository<SRInvitee> srInviteeRepository)
        {
            _escrowClientRepository = escrowClientRepository;
            _lookup_enterpriseRepository = lookup_enterpriseRepository;
            _srInviteeRepository = srInviteeRepository;

        }



        public IActionResult Index()
        {
            return View();
        }


        public responseData UploadFileFolderEnterprise(string enterprseFolder)
        {
            responseData res = new responseData();

            try
            {
                var file = Request.Form.Files[0];
                var folderName = Path.Combine(@"wwwroot\\Common\\Paperless");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (file.Length > 0)
                {


                    string fileName = Path.GetFileName(file.FileName);

                    string escrowNumber = Path.GetFileName(Path.GetDirectoryName(file.FileName));
                    EscrowClientsAppService item = new EscrowClientsAppService(_escrowClientRepository);
                    try
                    {
                        CreateOrEditEscrowClientDto obj = new CreateOrEditEscrowClientDto();
                        obj.EscrowNumber = escrowNumber;
                        // obj.EnterpriseId =267;
                        var temp = item.CreateOrEdit(obj);
                    }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                    catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                    {

                    }

                    string folder = Path.GetDirectoryName(file.FileName);
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {

                        file.CopyTo(stream);

                    }

                    char[] spearator = { ';', ' ' };
                    Int32 count = 3;



                    string[] strlist = folder.Split(spearator,
                           count, StringSplitOptions.None);

                    if (dbPath.Contains("Escrow.txt"))
                    {
                        ReadTextFile(dbPath);
                    }

                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {

            }
            return null;

        }


        public responseData ReadTextFile(string path)
        {
            int counter = 0;
            string line;

            // Read the file and display it line by line.  
            System.IO.StreamReader file = new System.IO.StreamReader(path);
            while ((line = file.ReadLine()) != null)
            {
                System.Console.WriteLine(line);
                if (!line.Contains("Name;EmailAddress;Usertype"))
                {
                    char[] spearator = { ';', ' ' };
                    Int32 count = 3;



                    string[] strlist = line.Split(spearator,
                           count, StringSplitOptions.None);
                    if (strlist.Length > 0)
                    {


                        SendMailEscrow(strlist[0], strlist[1], strlist[2]);
                    }

                }

                counter++;
            }

            file.Close();
            System.Console.WriteLine("There were {0} lines.", counter);
            // Suspend the screen.  
            System.Console.ReadLine();
            return null;
        }



        public responseData SendMailEscrow(string name, string fromEmail, string type)
        {

            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("manoj11609@gmail.com");
                mail.To.Add(fromEmail);
                mail.Subject = "Test Mail";

                mail.IsBodyHtml = true;
                mail.Body = "test applicaton";
                SmtpClient SmtpServer = new SmtpClient();
                SmtpServer.Port = 487;
                SmtpServer.Credentials = new System.Net.NetworkCredential("manoj11609@gmail.com", "rsam@11609");
                SmtpServer.Host = "smtp.gmail.com";
                SmtpServer.EnableSsl = true;
                //SmtpServer.UseDefaultCredentials = true;
                //SmtpServer.Timeout = 1500000;
                SmtpServer.Send(mail);
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return null;

            }
            return null;
        }




    }
}
