using System.Threading.Tasks;
using Abp.Application.Services;
using SR.EscrowBaseWeb.MultiTenancy.Payments.Dto;
using SR.EscrowBaseWeb.MultiTenancy.Payments.Stripe.Dto;

namespace SR.EscrowBaseWeb.MultiTenancy.Payments.Stripe
{
    public interface IStripePaymentAppService : IApplicationService
    {
        Task ConfirmPayment(StripeConfirmPaymentInput input);

        StripeConfigurationDto GetConfiguration();

        Task<SubscriptionPaymentDto> GetPaymentAsync(StripeGetPaymentInput input);

        Task<string> CreatePaymentSession(StripeCreatePaymentSessionInput input);
    }
}