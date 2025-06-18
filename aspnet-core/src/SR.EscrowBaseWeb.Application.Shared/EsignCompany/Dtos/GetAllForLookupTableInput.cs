using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.EsignCompany.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}