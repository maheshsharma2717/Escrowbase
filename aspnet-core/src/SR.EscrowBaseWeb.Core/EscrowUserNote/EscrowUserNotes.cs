using SR.EscrowBaseWeb.Authorization.Users;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace SR.EscrowBaseWeb.EscrowUserNote
{
    [Table("EscrowUserNoteses")]
    [Audited]
    public class EscrowUserNotes : Entity, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public virtual string Message { get; set; }

        public virtual string EscrowNumber { get; set; }

        public virtual DateTime? CreatedAt { get; set; }

        public virtual long? CreatedBy { get; set; }

        [ForeignKey("CreatedBy")]
        public User CreatedByFk { get; set; }

    }
}