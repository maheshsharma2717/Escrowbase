using System.Threading.Tasks;
using SR.EscrowBaseWeb.Authorization.Users;

namespace SR.EscrowBaseWeb.WebHooks
{
    public interface IAppWebhookPublisher
    {
        Task PublishTestWebhook();
    }
}
