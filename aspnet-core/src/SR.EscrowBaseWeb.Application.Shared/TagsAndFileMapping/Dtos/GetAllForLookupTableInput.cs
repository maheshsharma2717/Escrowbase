using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.TagsAndFileMapping.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}