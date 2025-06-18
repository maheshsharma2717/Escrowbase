using Abp.Application.Services.Dto;
using System;

namespace SR.EscrowBaseWeb.SREnterprise.Dtos
{
    public class GetAllEnterprisesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string EnterpriseNameFilter { get; set; }

		public string EmailFilter { get; set; }

		public string PhoneFilter { get; set; }



    }
}