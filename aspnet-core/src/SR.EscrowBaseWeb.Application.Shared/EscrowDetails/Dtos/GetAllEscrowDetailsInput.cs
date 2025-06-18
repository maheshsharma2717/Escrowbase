using Abp.Application.Services.Dto;
using System;

namespace SR.EscrowBaseWeb.EscrowDetails.Dtos
{
    public class GetAllEscrowDetailsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string NameFilter { get; set; }

		public string EmailFilter { get; set; }

		public string CompanyFilter { get; set; }

		public string EscrowIdFilter { get; set; }

		public string UsertypeFilter { get; set; }


		 public string UserNameFilter { get; set; }

		 
    }
}