using Abp.Application.Services.Dto;
using System;

namespace SR.EscrowBaseWeb.UserFileLogs.Dtos
{
    public class GetAllUserFileLogsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string NameFilter { get; set; }

		public string FileNameFilter { get; set; }

		public string StatusFilter { get; set; }

		public string UsertypeFilter { get; set; }

		public string AccesslevelFilter { get; set; }

		public DateTime? MaxUpdateOnFilter { get; set; }
		public DateTime? MinUpdateOnFilter { get; set; }


		 public string UserNameFilter { get; set; }

		 
    }
}