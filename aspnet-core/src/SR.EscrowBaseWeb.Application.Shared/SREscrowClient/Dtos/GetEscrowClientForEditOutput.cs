using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.SREscrowClient.Dtos
{
    public class GetEscrowClientForEditOutput
    {
		public CreateOrEditEscrowClientDto EscrowClient { get; set; }


    }
}