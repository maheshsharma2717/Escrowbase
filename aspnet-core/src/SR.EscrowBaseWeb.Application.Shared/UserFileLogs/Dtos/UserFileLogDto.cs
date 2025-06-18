
using System;
using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.UserFileLogs.Dtos
{
    public class UserFileLogDto : EntityDto<long>
    {
		public string Name { get; set; }

		public string FileName { get; set; }

		public string Status { get; set; }

		public string Usertype { get; set; }

		public string Accesslevel { get; set; }

		public DateTime UpdateOn { get; set; }

		 public long? UserId { get; set; }

		 
    }
}