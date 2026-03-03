using Abp.Application.Services.Dto;
using System;

namespace SR.EscrowBaseWeb.GetCurrentEscrow.Dtos
{
    public class GetAllCurrentEscrowsForExcelInput
    {
        public string Filter { get; set; }

        public string EscrowNoFilter { get; set; }

        public string CompanyNameFilter { get; set; }

        public string SubCompanyNameFilter { get; set; }

        public string CreatedOnFilter { get; set; }

    }
}