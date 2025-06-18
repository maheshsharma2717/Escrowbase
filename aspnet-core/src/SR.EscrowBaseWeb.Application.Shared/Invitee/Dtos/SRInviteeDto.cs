
using System;
using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.Invitee.Dtos
{
    public class SRInviteeDto : EntityDto
    {
		public string Email { get; set; }

		public string Name { get; set; }

		public string Phone { get; set; }

		public bool IsSignedUp { get; set; }


		 public int? UserTypeId { get; set; }

		 		 public int? EscrowClientId { get; set; }

		 
    }
}