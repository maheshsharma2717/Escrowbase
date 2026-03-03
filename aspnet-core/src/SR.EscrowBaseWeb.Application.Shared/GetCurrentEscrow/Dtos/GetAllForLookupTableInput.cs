using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.GetCurrentEscrow.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}