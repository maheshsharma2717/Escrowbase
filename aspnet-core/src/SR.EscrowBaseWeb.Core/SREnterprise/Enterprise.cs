using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace SR.EscrowBaseWeb.SREnterprise
{
	[Table("Enterprises")]
    [Audited]
    public class Enterprise : Entity , IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		[Required]
		public virtual string EnterpriseName { get; set; }
		
		public virtual string Email { get; set; }
		
		public virtual string Phone { get; set; }
		
		public virtual string Address1 { get; set; }
		
		public virtual string Address2 { get; set; }
		
		public virtual string City { get; set; }
		
		public virtual string State { get; set; }
		
		public virtual string PinCode { get; set; }
		
		public virtual string Contry { get; set; }
		
		public int? ParentId { get; set; }
		
		public string DBName { get; set; }


		public virtual string EnterpriseExt { get; set; }
		public virtual string EnterpriseExtFlag { get; set; }
		public virtual string PrimaryContact { get; set; }
		public virtual string PrimaryContactCellNo { get; set; }
		public virtual string AlternateEnterpriseName { get; set; }
		public virtual string BrokerName { get; set; }
		public virtual string CorporateName { get; set; }
		public virtual string OfficePhone { get; set; }
		public virtual string OfficeFax { get; set; }

		public virtual string SecondaryEnterpriseEmail { get; set; }
		public virtual string DisclosureVerbage { get; set; }
		public virtual string LicenseVerbiage { get; set; }
		public virtual string DefaultRealtor { get; set; }
		public virtual string DefaultMbroker { get; set; }

		public virtual string DefaultTitle { get; set; }
		public virtual string DefaultRefi { get; set; }
		public virtual string TaxPayerID { get; set; }
		public virtual string LicenesNo { get; set; }

		public virtual string EnterpriseId { get; set; }
		public virtual string Subcompany { get; set; }
		public virtual string Logo { get; set; }

		//
		public virtual string ESignProviderCode { get; set; }
		public virtual string ESignClientId { get; set; }
		public virtual string ESignClientSecret { get; set; }
		public virtual string ESignApiAccountId { get; set; }
		public virtual string ESignUserId { get; set; }
		public virtual string ESignFolderId { get; set; }
		public virtual bool IsAdminAssigned { get; set; }
        public virtual bool IsActive { get; set; }
		public virtual string RefreshToken { get; set; }
		public virtual string AccessToken { get; set; }
		public virtual DateTime? AccessTokenTime { get; set; }

	}
}