using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.E_SignRecords.Dtos
{
    public class GetE_SignRecordForEditOutput
    {
        public CreateOrEditE_SignRecordDto E_SignRecord { get; set; }

    }
}