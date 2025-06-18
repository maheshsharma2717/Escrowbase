using Abp.AutoMapper;
using SR.EscrowBaseWeb.Organizations.Dto;

namespace SR.EscrowBaseWeb.Models.Users
{
    [AutoMapFrom(typeof(OrganizationUnitDto))]
    public class OrganizationUnitModel : OrganizationUnitDto
    {
        public bool IsAssigned { get; set; }
    }
}