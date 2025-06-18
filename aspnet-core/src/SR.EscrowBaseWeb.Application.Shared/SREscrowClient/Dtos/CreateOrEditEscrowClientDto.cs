
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.SREscrowClient.Dtos
{
    public class CreateOrEditEscrowClientDto : EntityDto<int?>
    {

		public string EscrowNumber { get; set; }
		
		public string Name { get; set; }
		
		public string Email { get; set; }
		
		public string Phone { get; set; }
		
		public string Address { get; set; }
		
		public string State { get; set; }
		
		public string Pincode { get; set; }
		

    }
}