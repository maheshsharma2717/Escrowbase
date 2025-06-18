using System.Threading.Tasks;
using Abp.Webhooks;

namespace SR.EscrowBaseWeb.WebHooks
{
    public interface IWebhookEventAppService
    {
        Task<WebhookEvent> Get(string id);
    }
}
