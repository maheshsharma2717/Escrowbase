using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.SrEscrowUserMapping.Dtos
{
    public class GetEscrowUserMappingForEditOutput
    {
		public CreateOrEditEscrowUserMappingDto EscrowUserMapping { get; set; }

		public string UserName { get; set;}

		public string EscrowClientName { get; set;}


    }
}