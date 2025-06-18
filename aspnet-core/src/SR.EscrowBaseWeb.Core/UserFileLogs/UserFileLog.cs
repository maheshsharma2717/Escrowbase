using SR.EscrowBaseWeb.Authorization.Users;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace SR.EscrowBaseWeb.UserFileLogs
{
	[Table("UserFileLogs")]
    public class UserFileLog : Entity<long> 
    {

		public virtual string Name { get; set; }
		
		public virtual string FileName { get; set; }
		
		public virtual string Status { get; set; }
		
		public virtual string Usertype { get; set; }
		
		public virtual string Accesslevel { get; set; }
		
		public virtual DateTime UpdateOn { get; set; }
		

		public virtual long? UserId { get; set; }
		
        [ForeignKey("UserId")]
		public User UserFk { get; set; }
		
    }
}