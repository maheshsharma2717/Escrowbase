using System.Collections.Generic;

namespace SR.EscrowBaseWeb.MultiTenancy.Payments
{
    public interface IPaymentGatewayStore
    {
        List<PaymentGatewayModel> GetActiveGateways();
    }
}
