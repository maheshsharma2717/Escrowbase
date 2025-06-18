
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.SrEscrowUserMapping.Dtos
{
    public class CreateOrEditEscrowUserMappingDto : EntityDto<int?>
    {

		 public long? UserId { get; set; }
		 
		 		 public int EscrowClientId { get; set; }
		 
		 
    }
}