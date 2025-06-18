
using System;
using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.SRFileMapping.Dtos
{
    public class SrFileMappingDto : EntityDto
    {
		public string FileName { get; set; }

		public int UserId { get; set; }

		public bool IsActive { get; set; }

		public string Action { get; set; }

		public string EscrowiId { get; set; }

		public bool? OtherAction { get; set; }

		public string OtherActionNote { get; set; }

	}
}