using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.SrAssignedFilesDetails.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}