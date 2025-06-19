using Abp.Application.Services.Dto;
using System;

namespace SR.EscrowBaseWeb.EsignCompany.Dtos
{
    public class GetAllEsignCompanyMappingsInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string ComanyNameFilter { get; set; }

        public string IsActiveFilter { get; set; }

    }
}