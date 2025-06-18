using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace SR.EscrowBaseWeb.SrAssignedFilesDetails
{
    [Table("SrAssignedFilesDetails")]
    public class SrAssignedFilesDetail : Entity<long>
    {

        public virtual long UserId { get; set; }

        public virtual long? EOId { get; set; }

        public virtual string FileName { get; set; }

        public virtual string ReadStatus { get; set; }

        public virtual string SigningStatus { get; set; }

        public virtual string InputStatus { get; set; }

        public virtual DateTime UpdatedOn { get; set; }

        public virtual long? SrEscrowFileMasterId { get; set; }

    }
}