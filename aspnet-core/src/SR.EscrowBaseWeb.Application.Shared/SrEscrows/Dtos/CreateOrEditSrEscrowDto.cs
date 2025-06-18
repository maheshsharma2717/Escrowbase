using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.SrEscrows.Dtos
{
    public class CreateOrEditSrEscrowDto : EntityDto<int?>
    {

        [Required]
        public string EscrowNo { get; set; }

        public string PropertyAddress { get; set; }

        public string EscrowOfficerName { get; set; }

        public string EOEmail { get; set; }

        public string EOPhone { get; set; }

        public string EoPhoneExt { get; set; }

        public string EoPhoneCell { get; set; }

        public string SubCompanyName { get; set; }

        public int EnterpriseId { get; set; }

        public string CustomDetails { get; set; }

        public string Logo { get; set; }

    }
}