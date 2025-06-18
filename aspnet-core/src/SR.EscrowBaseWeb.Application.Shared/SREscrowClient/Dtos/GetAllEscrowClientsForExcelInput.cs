using Abp.Application.Services.Dto;
using System;

namespace SR.EscrowBaseWeb.SREscrowClient.Dtos
{
    public class GetAllEscrowClientsForExcelInput
    {
		public string Filter { get; set; }

		public string EscrowNumberFilter { get; set; }

		public string NameFilter { get; set; }

		public string EmailFilter { get; set; }

		public string PhoneFilter { get; set; }



    }
}