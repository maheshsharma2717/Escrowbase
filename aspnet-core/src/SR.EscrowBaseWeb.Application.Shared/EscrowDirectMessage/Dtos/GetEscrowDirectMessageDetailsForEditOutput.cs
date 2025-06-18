using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.EscrowDirectMessage.Dtos
{
    public class GetEscrowDirectMessageDetailsForEditOutput
    {
        public CreateOrEditEscrowDirectMessageDetailsDto EscrowDirectMessageDetails { get; set; }

        public string UserName { get; set; }

        public string UserName2 { get; set; }

    }
}