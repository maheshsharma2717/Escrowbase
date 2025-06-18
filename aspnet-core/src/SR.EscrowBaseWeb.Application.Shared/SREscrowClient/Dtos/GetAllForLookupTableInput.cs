using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.SREscrowClient.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}