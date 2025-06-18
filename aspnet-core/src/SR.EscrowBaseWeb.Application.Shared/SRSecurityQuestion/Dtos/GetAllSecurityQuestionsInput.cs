using Abp.Application.Services.Dto;
using System;

namespace SR.EscrowBaseWeb.SRSecurityQuestion.Dtos
{
    public class GetAllSecurityQuestionsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }



    }
}