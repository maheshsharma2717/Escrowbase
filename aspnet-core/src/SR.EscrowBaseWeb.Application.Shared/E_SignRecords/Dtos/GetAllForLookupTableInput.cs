using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.E_SignRecords.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}