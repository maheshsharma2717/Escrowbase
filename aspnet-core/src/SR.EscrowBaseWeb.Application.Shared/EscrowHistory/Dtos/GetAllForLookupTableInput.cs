using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.EscrowHistory.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}