using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace SR.EscrowBaseWeb.EscrowFileTag
{
    [Table("EscrowFileTagses")]
    public class EscrowFileTags : Entity, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public virtual string TagDescription { get; set; }

        public virtual string TagColor { get; set; }

        public virtual string EscrowNumber { get; set; }

        public virtual DateTime CreatedAt { get; set; }

        public virtual string FileName { get; set; }

    }
}