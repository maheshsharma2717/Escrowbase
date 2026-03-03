using SR.EscrowBaseWeb.Authorization.Users;
using SR.EscrowBaseWeb.SrEscrows;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace SR.EscrowBaseWeb.EscrowHistory
{
    [Table("EscrowAccessHistories")]
    public class EscrowAccessHistory : Entity, IMustHaveTenant
    {
        public int TenantId { get; set; }

        public virtual DateTime CreatedAt { get; set; }

        public virtual long? UserId { get; set; }

        [ForeignKey("UserId")]
        public User UserFk { get; set; }

        public virtual int? EscrowId { get; set; }

        [ForeignKey("EscrowId")]
        public SrEscrow EscrowFk { get; set; }

    }
}