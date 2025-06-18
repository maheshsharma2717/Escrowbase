using Abp.Application.Services.Dto;
using System;

namespace SR.EscrowBaseWeb.SRUserAnswer.Dtos
{
    public class GetAllUserAnswersInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }


		 public string UserNameFilter { get; set; }

		 
    }
}