using Abp.AutoMapper;
using SR.EscrowBaseWeb.ApiClient;

namespace SR.EscrowBaseWeb.Models.Common
{
    [AutoMapFrom(typeof(TenantInformation)),
     AutoMapTo(typeof(TenantInformation))]
    public class TenantInformationPersistanceModel
    {
        public string TenancyName { get; set; }

        public int TenantId { get; set; }
    }
}