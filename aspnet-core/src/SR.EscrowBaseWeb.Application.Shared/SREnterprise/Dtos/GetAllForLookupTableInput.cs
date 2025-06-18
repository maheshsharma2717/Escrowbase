using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.SREnterprise.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}