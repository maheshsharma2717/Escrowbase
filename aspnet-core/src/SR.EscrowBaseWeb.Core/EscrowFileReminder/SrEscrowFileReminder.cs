using SR.EscrowBaseWeb.EscrowFileMaster;
using SR.EscrowBaseWeb.Authorization.Users;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace SR.EscrowBaseWeb.EscrowFileReminder
{
    [Table("srEscrowFileReminders")]
    [Audited]
    public class SrEscrowFileReminder : Entity<long>
    {

        public virtual string ReminderType { get; set; }

        public virtual string SentTo { get; set; }

        public virtual string ReminderText { get; set; }

        public virtual string SentFrom { get; set; }

        public virtual DateTime? CreatedAt { get; set; }

        public virtual bool ReminderStatus { get; set; }

        public virtual string SentToUserType { get; set; }

        public virtual string SentFromUserType { get; set; }

        public virtual long? SREscrowFileMasterId { get; set; }

        [ForeignKey("SREscrowFileMasterId")]
        public SREscrowFileMaster SREscrowFileMasterFk { get; set; }

        public virtual long? CreatedBy { get; set; }

        [ForeignKey("CreatedBy")]
        public User CreatedByFk { get; set; }

    }
}