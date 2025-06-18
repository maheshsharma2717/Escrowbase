using SR.EscrowBaseWeb.Authorization.Users;
using SR.EscrowBaseWeb.SREscrowClient;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace SR.EscrowBaseWeb.SrEscrowUserMapping
{
	[Table("EscrowUserMappings")]
    [Audited]
    public class EscrowUserMapping : Entity 
    {


		public virtual long? UserId { get; set; }
		
     
		public User UserFk { get; set; }
		
		public virtual int? EscrowClientId { get; set; }
		
        
		public EscrowClient EscrowClientFk { get; set; }
		
    }
}