using System;
using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.EsignCompany.Dtos
{
    public class EsignCompanyMappingDto : EntityDto
    {
        public string ComanyName { get; set; }

        public string IsActive { get; set; }

    }
}