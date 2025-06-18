using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.SrAssignedFilesDetails.Dtos
{
    public class GetSrAssignedFilesDetailForEditOutput
    {
        public CreateOrEditSrAssignedFilesDetailDto SrAssignedFilesDetail { get; set; }

    }
}