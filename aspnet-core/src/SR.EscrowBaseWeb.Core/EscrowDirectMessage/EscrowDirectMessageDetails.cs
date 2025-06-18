using SR.EscrowBaseWeb.Authorization.Users;
using SR.EscrowBaseWeb.Authorization.Users;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace SR.EscrowBaseWeb.EscrowDirectMessage
{
    [Table("EscrowDirectMessageDetailses")]
    public class EscrowDirectMessageDetails : Entity<long>
    {

        public virtual string Message { get; set; }

        public virtual DateTime? CreatedDate { get; set; }

        public virtual bool Status { get; set; }

        public virtual string MessageType { get; set; }

        public virtual long? EscrowUserId { get; set; }

        [ForeignKey("EscrowUserId")]
        public User EscrowUserFk { get; set; }

        public virtual long? SenderUserId { get; set; }

        [ForeignKey("SenderUserId")]
        public User SenderUserFk { get; set; }

    }
}