using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace SR.EscrowBaseWeb.SRSecurityQuestion
{
	[Table("SecurityQuestions")]
    public class SecurityQuestion : Entity 
    {

		public virtual string Question { get; set; }
		

    }
}