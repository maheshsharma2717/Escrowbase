
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.SREnterprise.Dtos
{
    public class CreateOrEditEnterpriseDto : EntityDto<int?>
    {

		[Required]
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


	}
}