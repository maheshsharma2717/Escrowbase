using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.EscrowFileMaster.Dtos
{
    public class GetSREscrowFileMasterForEditOutput
    {
        public CreateOrEditSREscrowFileMasterDto SREscrowFileMaster { get; set; }

    }
}