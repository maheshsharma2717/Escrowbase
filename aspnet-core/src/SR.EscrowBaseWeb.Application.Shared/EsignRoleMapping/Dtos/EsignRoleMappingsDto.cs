using System;
using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.EsignRoleMapping.Dtos
{
    public class EsignRoleMappingsDto : EntityDto
    {
        public int Id { get; set; }

        public string EsignRole { get; set; }

        public string UserRole { get; set; }

        public int EsignCompanyCode { get; set; }

    }
}