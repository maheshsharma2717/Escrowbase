using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.SrEscrows.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}