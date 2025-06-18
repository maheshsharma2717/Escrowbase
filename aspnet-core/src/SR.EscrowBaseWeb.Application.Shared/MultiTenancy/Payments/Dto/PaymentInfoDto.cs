using SR.EscrowBaseWeb.Editions.Dto;

namespace SR.EscrowBaseWeb.MultiTenancy.Payments.Dto
{
    public class PaymentInfoDto
    {
        public EditionSelectDto Edition { get; set; }

        public decimal AdditionalPrice { get; set; }

        public bool IsLessThanMinimumUpgradePaymentAmount()
        {
            return AdditionalPrice < EscrowBaseWebConsts.MinimumUpgradePaymentAmount;
        }
    }
}
