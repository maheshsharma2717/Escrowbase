using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.EscrowDetails.Dtos
{
    public class GetEscrowDetailForEditOutput
    {
		public CreateOrEditEscrowDetailDto EscrowDetail { get; set; }

		public string UserName { get; set;}


    }
}