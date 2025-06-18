using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace SR.EscrowBaseWeb.SrUserType
{
	[Table("UserTypes")]
    
    public class UserType : Entity 
    {

		public virtual string Type { get; set; }
		public virtual int? EnterpriseId { get; set; }
        public virtual long? UserId { get; set; }

    }
}