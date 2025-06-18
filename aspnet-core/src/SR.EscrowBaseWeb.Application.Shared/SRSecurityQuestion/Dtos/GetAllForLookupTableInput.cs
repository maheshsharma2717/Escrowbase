using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.SRSecurityQuestion.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}