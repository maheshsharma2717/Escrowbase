using Abp.Application.Services.Dto;
using System;

namespace SR.EscrowBaseWeb.SrEscrows.Dtos
{
    public class GetAllSrEscrowsInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string EscrowNoFilter { get; set; }

        public string PropertyAddressFilter { get; set; }

        public string EscrowOfficerNameFilter { get; set; }

        public string EOEmailFilter { get; set; }

        public string EOPhoneFilter { get; set; }

        public string EoPhoneExtFilter { get; set; }

        public string EoPhoneCellFilter { get; set; }

        public string SubCompanyNameFilter { get; set; }

        public int? MaxEnterpriseIdFilter { get; set; }
        public int? MinEnterpriseIdFilter { get; set; }

        public string CustomDetailsFilter { get; set; }

        public string LogoFilter { get; set; }

    }
}