
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.SRUserAnswer.Dtos
{
    public class CreateOrEditUserAnswerDto : EntityDto<int?>
    {

		public string Question { get; set; }
		
		
		public string Answer { get; set; }
		
		
		 public long? UserId { get; set; }
		 
		 
    }
}