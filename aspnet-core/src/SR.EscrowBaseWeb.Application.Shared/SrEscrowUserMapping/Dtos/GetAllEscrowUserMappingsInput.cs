using Abp.Application.Services.Dto;
using System;

namespace SR.EscrowBaseWeb.SrEscrowUserMapping.Dtos
{
    public class GetAllEscrowUserMappingsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }


		 public string UserNameFilter { get; set; }

		 		 public string EscrowClientNameFilter { get; set; }

		 
    }
}