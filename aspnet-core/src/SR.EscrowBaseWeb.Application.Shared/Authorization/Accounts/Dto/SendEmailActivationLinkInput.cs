using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.Authorization.Accounts.Dto
{
    public class SendEmailActivationLinkInput
    {
        [Required]
        public string EmailAddress { get; set; }
    }
}