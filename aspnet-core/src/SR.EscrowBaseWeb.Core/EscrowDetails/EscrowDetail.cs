using SR.EscrowBaseWeb.Authorization.Users;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace SR.EscrowBaseWeb.EscrowDetails
{
	[Table("EscrowDetails")]
    public class EscrowDetail : Entity<long> 
    {

		public virtual string Name { get; set; }
		
		public virtual string Email { get; set; }
		
		public virtual string Company { get; set; }
		
		public virtual string EscrowId { get; set; }
		
		public virtual string Usertype { get; set; }
		

		public virtual long? UserId { get; set; }
		
        [ForeignKey("UserId")]
		public User UserFk { get; set; }
		
    }
}