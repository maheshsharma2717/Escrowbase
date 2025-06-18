namespace SR.EscrowBaseWeb.ApiClient
{
    public class TenantInformation
    {
        public string TenancyName { get; set; }

        public int TenantId { get; set; }

        public TenantInformation(string tenancyName, int tenantId)
        {
            TenancyName = tenancyName;
            TenantId = tenantId;
        }
    }
}