using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.GetCurrentEscrow.Dtos
{
    public class GetCurrentEscrowForEditOutput
    {
        public CreateOrEditCurrentEscrowDto CurrentEscrow { get; set; }

    }
}