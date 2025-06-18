using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.SRSecurityQuestion.Dtos
{
    public class GetSecurityQuestionForEditOutput
    {
		public CreateOrEditSecurityQuestionDto SecurityQuestion { get; set; }


    }
}