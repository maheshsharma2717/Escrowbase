using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.SRFileMapping.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}