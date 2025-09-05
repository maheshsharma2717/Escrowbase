using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace SR.EscrowBaseWeb.EsignCompany
{
    [Table("EsignCompanyMappings")]
    [Audited]
    public class EsignCompanyMapping : Entity
    {

        public virtual string ComanyName { get; set; }

        public virtual string IsActive { get; set; }

    }
}