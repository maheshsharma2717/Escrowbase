using Abp.Application.Services.Dto;
using System;

namespace SR.EscrowBaseWeb.Invitee.Dtos
{
    public class GetAllSRInviteesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }


		 public string UserTypeTypeFilter { get; set; }

		 		 public string EscrowClientNameFilter { get; set; }

		 
    }
}