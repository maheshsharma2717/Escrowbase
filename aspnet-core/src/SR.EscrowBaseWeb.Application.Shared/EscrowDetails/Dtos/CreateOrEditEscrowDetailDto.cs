
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.EscrowDetails.Dtos
{
    public class CreateOrEditEscrowDetailDto : EntityDto<long?>
    {

		public string Name { get; set; }
		
		
		public string Email { get; set; }
		
		
		public string Company { get; set; }
		
		
		public string EscrowId { get; set; }
		
		
		public string Usertype { get; set; }
		
		
		 public long? UserId { get; set; }
		 
		 
    }
}