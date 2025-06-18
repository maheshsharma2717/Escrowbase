using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.SRUserAnswer.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}