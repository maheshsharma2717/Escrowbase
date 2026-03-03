using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.EscrowHistory.Dtos
{
    public class GetEscrowAccessHistoryForEditOutput
    {
        public CreateOrEditEscrowAccessHistoryDto EscrowAccessHistory { get; set; }

        public string UserName { get; set; }

        public string EscrowClientName { get; set; }

    }
}