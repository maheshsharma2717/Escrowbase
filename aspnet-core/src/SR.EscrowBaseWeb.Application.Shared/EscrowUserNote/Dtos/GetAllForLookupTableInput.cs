using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.EscrowUserNote.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}