using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.SrInvitationRecords.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}