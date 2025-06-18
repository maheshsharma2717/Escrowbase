using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.UserFileLogs.Dtos
{
    public class GetUserFileLogForEditOutput
    {
		public CreateOrEditUserFileLogDto UserFileLog { get; set; }

		public string UserName { get; set;}


    }
}