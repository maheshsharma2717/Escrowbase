using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.EsignRoleMapping.Dtos
{
    public class CreateOrEditEsignRoleMappingsDto : EntityDto<int?>
    {

        public int Id { get; set; }

        [StringLength(EsignRoleMappingsConsts.MaxEsignRoleLength, MinimumLength = EsignRoleMappingsConsts.MinEsignRoleLength)]
        public string EsignRole { get; set; }

        [StringLength(EsignRoleMappingsConsts.MaxUserRoleLength, MinimumLength = EsignRoleMappingsConsts.MinUserRoleLength)]
        public string UserRole { get; set; }

        public int EsignCompanyCode { get; set; }

    }
}