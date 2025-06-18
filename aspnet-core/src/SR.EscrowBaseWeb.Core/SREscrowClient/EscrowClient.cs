using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace SR.EscrowBaseWeb.SREscrowClient
{
	[Table("EscrowClients")]
    [Audited]
    public class EscrowClient : Entity 
    {

		public virtual string EscrowNumber { get; set; }
		
		public virtual string Name { get; set; }
		
		public virtual string Email { get; set; }
		
		public virtual string Phone { get; set; }
		
		public virtual string Address { get; set; }
		
		public virtual string State { get; set; }
		
		public virtual string Pincode { get; set; }
		

    }
}