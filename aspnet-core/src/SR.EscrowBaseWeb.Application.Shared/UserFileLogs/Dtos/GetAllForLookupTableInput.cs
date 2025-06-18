using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.UserFileLogs.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}