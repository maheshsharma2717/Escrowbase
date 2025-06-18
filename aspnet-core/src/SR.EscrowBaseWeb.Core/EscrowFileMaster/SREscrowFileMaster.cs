using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace SR.EscrowBaseWeb.EscrowFileMaster
{
    [Table("SREscrowFileMasters")]
    [Audited]
    public class SREscrowFileMaster : Entity<long>
    {

        public virtual string FileFullName { get; set; }

        public virtual string FileShortName { get; set; }

        public virtual bool OtherAction { get; set; }

        public string OtherActionNote { get; set; }

       // public virtual long? UserId { get; set; }

    }
}