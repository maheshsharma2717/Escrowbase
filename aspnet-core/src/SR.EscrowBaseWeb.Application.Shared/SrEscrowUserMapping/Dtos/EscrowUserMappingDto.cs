
using System;
using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.SrEscrowUserMapping.Dtos
{
    public class EscrowUserMappingDto : EntityDto
    {

		 public long? UserId { get; set; }

		 		 public int? EscrowClientId { get; set; }

		 
    }
}