using System.Threading.Tasks;
using Abp.Application.Services;
using SR.EscrowBaseWeb.MultiTenancy.Payments.PayPal.Dto;

namespace SR.EscrowBaseWeb.MultiTenancy.Payments.PayPal
{
    public interface IPayPalPaymentAppService : IApplicationService
    {
        Task ConfirmPayment(long paymentId, string paypalOrderId);

        PayPalConfigurationDto GetConfiguration();
    }
}
