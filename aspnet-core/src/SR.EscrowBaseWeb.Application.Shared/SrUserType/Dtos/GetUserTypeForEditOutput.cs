using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.SrUserType.Dtos
{
    public class GetUserTypeForEditOutput
    {
		public CreateOrEditUserTypeDto UserType { get; set; }


    }
}