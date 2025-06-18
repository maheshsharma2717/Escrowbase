
using System;
using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.EscrowDetails.Dtos
{
    public class EscrowDetailDto : EntityDto<long>
    {
		public string Name { get; set; }

		public string Email { get; set; }

		public string Company { get; set; }

		public string EscrowId { get; set; }

		public string Usertype { get; set; }


		 public long? UserId { get; set; }

		 
    }
}