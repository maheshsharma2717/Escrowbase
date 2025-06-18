using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.SREscrowFileHistory.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}