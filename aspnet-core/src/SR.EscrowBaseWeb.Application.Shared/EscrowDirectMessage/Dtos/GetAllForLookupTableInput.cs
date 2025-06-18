using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.EscrowDirectMessage.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}