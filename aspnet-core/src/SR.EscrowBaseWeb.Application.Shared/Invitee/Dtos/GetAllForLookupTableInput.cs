using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.Invitee.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}