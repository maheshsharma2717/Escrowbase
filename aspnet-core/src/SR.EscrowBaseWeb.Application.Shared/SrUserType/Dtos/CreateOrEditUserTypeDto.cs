
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.SrUserType.Dtos
{
    public class CreateOrEditUserTypeDto : EntityDto<int?>
    {

		public string Type { get; set; }
		
        public int? EnterpriseId { get; set; }

        public long? UserId { get; set; }
    }
}