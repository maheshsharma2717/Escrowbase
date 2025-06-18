using Abp.Application.Services.Dto;
using System;

namespace SR.EscrowBaseWeb.SRFileMapping.Dtos
{
    public class GetAllSrFileMappingsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string FileNameFilter { get; set; }

		public int? MaxUserIdFilter { get; set; }
		public int? MinUserIdFilter { get; set; }

		public int? IsActiveFilter { get; set; }

		public string ActionFilter { get; set; }

		public string EscrowiIdFilter { get; set; }

		public int UserId { get; set; }

	}
}