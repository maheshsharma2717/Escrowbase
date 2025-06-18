using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.SrEscrows.Dtos
{
    public class GetSrEscrowForEditOutput
    {
        public CreateOrEditSrEscrowDto SrEscrow { get; set; }

    }
}