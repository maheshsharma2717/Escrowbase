
using System;
using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.SREnterprise.Dtos
{
    public class EnterpriseDto : EntityDto
    {
		public string EnterpriseName { get; set; }

		public string Email { get; set; }

		public string Phone { get; set; }

		public string Address1 { get; set; }

		public string Address2 { get; set; }

		public string City { get; set; }

		public string State { get; set; }

		public string PinCode { get; set; }

		public string Contry { get; set; }

		public int? ParentId { get; set; }

		public string DBName { get; set; }

 
        public string EnterpriseExt { get; set; }
        public string EnterpriseExtFlag { get; set; }
   
        public string PrimaryContact { get; set; }
        public string PrimaryContactCellNo { get; set; }
        public string AlternateEnterpriseName { get; set; }
        public string BrokerName { get; set; }
        public string CorporateName { get; set; }
        public string OfficePhone { get; set; }
        public string OfficeFax { get; set; }
        public string SecondaryEnterpriseEmail { get; set; }
        public string DisclosureVerbage { get; set; }
        public string LicenseVerbiage { get; set; }
        public string DefaultRealtor { get; set; }
        public string DefaultMbroker { get; set; }
        public string DefaultTitle { get; set; }
        public string DefaultRefi { get; set; }
        public string TaxPayerID { get; set; }
        public string LicenesNo { get; set; }
        public string EnterpriseId { get; set; }

    }
}