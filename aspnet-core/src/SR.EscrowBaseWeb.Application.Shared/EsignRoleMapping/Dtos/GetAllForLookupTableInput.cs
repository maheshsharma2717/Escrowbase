using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.EsignRoleMapping.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}