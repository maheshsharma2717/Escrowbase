
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.SRSecurityQuestion.Dtos
{
    public class CreateOrEditSecurityQuestionDto : EntityDto<int?>
    {

		public string Question { get; set; }
		
		

    }
}