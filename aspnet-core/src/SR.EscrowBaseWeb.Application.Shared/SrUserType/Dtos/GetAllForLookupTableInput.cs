using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.SrUserType.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}