using Abp.Application.Services.Dto;
using System;

namespace SR.EscrowBaseWeb.SrInvitationRecords.Dtos
{
    public class GetAllSrInvitationRecordsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string EmailFilter { get; set; }

		public string DomainAccessInstanceFilter { get; set; }

		public string EscrowCompanyFilter { get; set; }

		public string EscrowOfficerFilter { get; set; }

		public string EscrowContactEmailFilter { get; set; }

		public string EscrowNumberFilter { get; set; }

		public string UsertypeFilter { get; set; }

		public string EscrowOfficerPhoneNumberFilter { get; set; }


		 public string UserNameFilter { get; set; }

		 
    }
}