using Abp.Configuration;
using Abp.Net.Mail;
using Abp.Net.Mail.Smtp;
using Abp.Runtime.Security;

namespace SR.EscrowBaseWeb.Net.Emailing
{
    public class EscrowBaseWebSmtpEmailSenderConfiguration : SmtpEmailSenderConfiguration
    {
        public EscrowBaseWebSmtpEmailSenderConfiguration(ISettingManager settingManager) : base(settingManager)
        {

        }

        public override string Password => SimpleStringCipher.Instance.Decrypt(GetNotEmptySettingValue(EmailSettingNames.Smtp.Password));
    }
}