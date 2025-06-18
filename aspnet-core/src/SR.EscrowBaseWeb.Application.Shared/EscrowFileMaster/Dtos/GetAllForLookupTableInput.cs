using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.EscrowFileMaster.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}