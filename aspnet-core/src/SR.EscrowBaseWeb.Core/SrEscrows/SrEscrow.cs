using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace SR.EscrowBaseWeb.SrEscrows
{
    [Table("SrEscrows")]
    [Audited]
    public class SrEscrow : Entity
    {

        [Required]
        public virtual string EscrowNo { get; set; }

        public virtual string PropertyAddress { get; set; }

        public virtual string EscrowOfficerName { get; set; }

        public virtual string EOEmail { get; set; }

        public virtual string EOPhone { get; set; }

        public virtual string EoPhoneExt { get; set; }

        public virtual string EoPhoneCell { get; set; }

        public virtual string SubCompanyName { get; set; }

        public virtual int EnterpriseId { get; set; }

        public virtual string CustomDetails { get; set; }

        public virtual string Logo { get; set; }

    }
}