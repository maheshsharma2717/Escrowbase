using Abp.Application.Services.Dto;
using System;

namespace SR.EscrowBaseWeb.GetCurrentEscrow.Dtos
{
    public class GetAllCurrentEscrowsInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string EscrowNoFilter { get; set; }

        public string CompanyNameFilter { get; set; }

        public string SubCompanyNameFilter { get; set; }


        public DateTime? MaxCreatedOnFilter { get; set; }
        public DateTime? MinCreatedOnFilter { get; set; }

    }
}