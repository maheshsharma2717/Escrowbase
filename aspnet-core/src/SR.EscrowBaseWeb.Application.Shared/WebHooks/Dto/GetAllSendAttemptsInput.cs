using SR.EscrowBaseWeb.Dto;

namespace SR.EscrowBaseWeb.WebHooks.Dto
{
    public class GetAllSendAttemptsInput : PagedInputDto
    {
        public string SubscriptionId { get; set; }
    }
}
