
using System;
using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.SRUserAnswer.Dtos
{
    public class UserAnswerDto : EntityDto
    {
		public string Question { get; set; }

		public string Answer { get; set; }


		 public long? UserId { get; set; }

		 
    }
}