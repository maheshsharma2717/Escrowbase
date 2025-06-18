using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.EscrowFileTag.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}