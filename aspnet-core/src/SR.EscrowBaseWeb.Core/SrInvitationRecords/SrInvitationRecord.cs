using SR.EscrowBaseWeb.Authorization.Users;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace SR.EscrowBaseWeb.SrInvitationRecords
{
	[Table("SrInvitationRecords")]
    public class SrInvitationRecord : Entity<long> 
    {

		public virtual string Email { get; set; }
		
		public virtual string DomainAccessInstance { get; set; }
		
		public virtual string EscrowCompany { get; set; }
		
		public virtual string EscrowOfficer { get; set; }
		
		public virtual string EscrowContactEmail { get; set; }
		
		public virtual string EscrowNumber { get; set; }
		
		public virtual string Usertype { get; set; }
		
		public virtual string EscrowOfficerPhoneNumber { get; set; }
		

		public virtual long? UserId { get; set; }
		
        [ForeignKey("UserId")]
		public User UserFk { get; set; }
		
    }
}