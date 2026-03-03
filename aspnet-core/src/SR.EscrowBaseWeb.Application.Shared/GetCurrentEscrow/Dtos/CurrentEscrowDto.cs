using System;
using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.GetCurrentEscrow.Dtos
{
    public class CurrentEscrowDto : EntityDto
    {

        public string EscrowNo { get; set; }

        public string CompanyName { get; set; }

        public string SubCompanyName { get; set; }

        public DateTime CreatedOn { get; set; }

        public string FileName { get; set; }

        public  string UserName { get; set; }
      
        public Boolean IsActive { get; set; }
    }
}