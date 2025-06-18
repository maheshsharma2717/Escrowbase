using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.SREscrowFileHistory.Dtos
{
    public class GetEscrowFileHistoryForEditOutput
    {
        public CreateOrEditEscrowFileHistoryDto EscrowFileHistory { get; set; }

        public string UserName { get; set; }

    }
}