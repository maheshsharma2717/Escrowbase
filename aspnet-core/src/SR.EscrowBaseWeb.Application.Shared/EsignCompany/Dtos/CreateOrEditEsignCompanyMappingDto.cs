using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.EsignCompany.Dtos
{
    public class CreateOrEditEsignCompanyMappingDto : EntityDto<int?>
    {

        public string ComanyName { get; set; }

        public string IsActive { get; set; }

    }
}