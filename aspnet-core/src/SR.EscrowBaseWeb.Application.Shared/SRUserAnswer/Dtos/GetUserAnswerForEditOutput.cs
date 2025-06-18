using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.SRUserAnswer.Dtos
{
    public class GetUserAnswerForEditOutput
    {
		public CreateOrEditUserAnswerDto UserAnswer { get; set; }

		public string UserName { get; set;}


    }
}