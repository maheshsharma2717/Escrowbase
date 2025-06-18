using SR.EscrowBaseWeb.SrUserType;
using SR.EscrowBaseWeb.SREscrowClient;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace SR.EscrowBaseWeb.Invitee
{
	[Table("SRInvitees")]
    public class SRInvitee : Entity 
    {

		public virtual string Email { get; set; }
		
		public virtual string Name { get; set; }
		
		public virtual string Phone { get; set; }
		
		public virtual bool IsSignedUp { get; set; }
		

		public virtual int? UserTypeId { get; set; }
		
        [ForeignKey("UserTypeId")]
		public UserType UserTypeFk { get; set; }
		
		public virtual int? EscrowClientId { get; set; }
		
        [ForeignKey("EscrowClientId")]
		public EscrowClient EscrowClientFk { get; set; }
		
    }
}