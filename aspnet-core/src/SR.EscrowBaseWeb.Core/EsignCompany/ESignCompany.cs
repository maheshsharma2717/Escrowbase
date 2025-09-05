using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace SR.EscrowBaseWeb.EsignCompany
{
    [Table("E_SignCompany")]
    public class ESignCompany : Entity<long>
    {
        public string CompanyName { get; set; }
        public int SystemCode { get; set; }
        public bool IsActive { get; set; }
    }

}
