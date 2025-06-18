using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace SR.EscrowBaseWeb.SRFileMapping
{
	[Table("SrFileMappings")]
    [Audited]
    public class SrFileMapping : Entity
    {

		public virtual string FileName { get; set; }
		
		public virtual int UserId { get; set; }
		
		public virtual bool IsActive { get; set; }
		
		public virtual string Action { get; set; }
		
		public virtual string EscrowiId { get; set; }

		public virtual long? SrEscrowFileMasterId { get; set; }

		public virtual bool OtherAction { get; set; }

		public virtual string OtherActionNote { get; set; }

	}
}