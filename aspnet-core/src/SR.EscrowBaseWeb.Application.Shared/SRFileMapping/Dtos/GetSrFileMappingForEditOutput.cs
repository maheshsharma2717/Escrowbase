using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.SRFileMapping.Dtos
{
    public class GetSrFileMappingForEditOutput
    {
		public CreateOrEditSrFileMappingDto SrFileMapping { get; set; }


    }
}