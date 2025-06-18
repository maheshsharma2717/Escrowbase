using SR.EscrowBaseWeb.Authorization.Users;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace SR.EscrowBaseWeb.SREscrowFileHistory
{
    [Table("EscrowFileHistories")]
    [Audited]
    public class EscrowFileHistory : Entity<long>
    {

        [StringLength(EscrowFileHistoryConsts.MaxMessageLength, MinimumLength = EscrowFileHistoryConsts.MinMessageLength)]
        public virtual string Message { get; set; }

        [StringLength(EscrowFileHistoryConsts.MaxActionTypeLength, MinimumLength = EscrowFileHistoryConsts.MinActionTypeLength)]
        public virtual string ActionType { get; set; }

        public virtual string FileFullPath { get; set; }

        public virtual DateTime CreatedAt { get; set; }

        public virtual long? UserId { get; set; }

        [ForeignKey("UserId")]
        public User UserFk { get; set; }

      public virtual long? SrEscrowFileMasterId { get; set; }

    }
}