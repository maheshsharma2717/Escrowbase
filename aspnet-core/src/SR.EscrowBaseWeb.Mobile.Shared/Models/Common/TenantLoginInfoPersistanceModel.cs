using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using SR.EscrowBaseWeb.Sessions.Dto;

namespace SR.EscrowBaseWeb.Models.Common
{
    [AutoMapFrom(typeof(TenantLoginInfoDto)),
     AutoMapTo(typeof(TenantLoginInfoDto))]
    public class TenantLoginInfoPersistanceModel : EntityDto
    {
        public string TenancyName { get; set; }

        public string Name { get; set; }
    }
}