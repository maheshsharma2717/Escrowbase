using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.EsignCompany.Dtos
{
    public class GetEsignCompanyMappingForEditOutput
    {
        public CreateOrEditEsignCompanyMappingDto EsignCompanyMapping { get; set; }

    }
}