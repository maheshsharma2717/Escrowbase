using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.EsignRoleMapping.Dtos
{
    public class GetEsignRoleMappingsForEditOutput
    {
        public CreateOrEditEsignRoleMappingsDto EsignRoleMappings { get; set; }

    }
}