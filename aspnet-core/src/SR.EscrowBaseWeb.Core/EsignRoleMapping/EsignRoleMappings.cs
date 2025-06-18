using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace SR.EscrowBaseWeb.EsignRoleMapping
{
    [Table("EsignRoleMappingses")]
    public class EsignRoleMappings : Entity, IMustHaveTenant
    {
        public int TenantId { get; set; }

        public virtual int Id { get; set; }

        [StringLength(EsignRoleMappingsConsts.MaxEsignRoleLength, MinimumLength = EsignRoleMappingsConsts.MinEsignRoleLength)]
        public virtual string EsignRole { get; set; }

        [StringLength(EsignRoleMappingsConsts.MaxUserRoleLength, MinimumLength = EsignRoleMappingsConsts.MinUserRoleLength)]
        public virtual string UserRole { get; set; }

        public virtual int EsignCompanyCode { get; set; }

    }
}