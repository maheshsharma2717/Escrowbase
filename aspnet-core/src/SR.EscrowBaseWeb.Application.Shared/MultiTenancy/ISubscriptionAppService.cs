using System.Threading.Tasks;
using Abp.Application.Services;

namespace SR.EscrowBaseWeb.MultiTenancy
{
    public interface ISubscriptionAppService : IApplicationService
    {
        Task DisableRecurringPayments();

        Task EnableRecurringPayments();
    }
}
