using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.SrEscrowUserMapping.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}