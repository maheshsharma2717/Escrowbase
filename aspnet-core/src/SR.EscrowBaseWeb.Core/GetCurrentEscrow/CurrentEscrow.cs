using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace SR.EscrowBaseWeb.GetCurrentEscrow
{
    [Table("CurrentEscrows")]
    public class CurrentEscrow : Entity, IMayHaveTenant
    {
        public int? TenantId { get; set; }


        [Required]
        public virtual string EscrowNo { get; set; }

        [Required]
        public virtual string CompanyName { get; set; }

        [Required]
        public virtual string SubCompanyName { get; set; }

        public virtual DateTime CreatedOn { get; set; }

        [Required]
        public virtual string FileName { get; set; }

        [Required]
        public virtual string UserName { get; set; }

        [Required]
        public virtual Boolean IsActive { get; set; }

    }
}