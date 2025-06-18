
using System;
using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.SrInvitationRecords.Dtos
{
    public class SrInvitationRecordDto : EntityDto<long>
    {
		public string Email { get; set; }

		public string DomainAccessInstance { get; set; }

		public string EscrowCompany { get; set; }

		public string EscrowOfficer { get; set; }

		public string EscrowContactEmail { get; set; }

		public string EscrowNumber { get; set; }

		public string Usertype { get; set; }

		public string EscrowOfficerPhoneNumber { get; set; }


		 public long? UserId { get; set; }

		 
    }
}