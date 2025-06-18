using Abp.Auditing;
using SR.EscrowBaseWeb.Configuration.Dto;

namespace SR.EscrowBaseWeb.Configuration.Tenants.Dto
{
    public class TenantEmailSettingsEditDto : EmailSettingsEditDto
    {
        public bool UseHostDefaultEmailSettings { get; set; }
    }
}