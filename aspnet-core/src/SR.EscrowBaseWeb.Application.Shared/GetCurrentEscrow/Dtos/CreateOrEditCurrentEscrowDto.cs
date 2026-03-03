using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.GetCurrentEscrow.Dtos
{
    public class CreateOrEditCurrentEscrowDto : EntityDto<int?>
    {

        [Required]
        public string EscrowNo { get; set; }

        [Required]
        public string CompanyName { get; set; }

        [Required]
        public string SubCompanyName { get; set; }

        public DateTime? CreatedOn { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public Boolean IsActive { get; set; }
    }
}