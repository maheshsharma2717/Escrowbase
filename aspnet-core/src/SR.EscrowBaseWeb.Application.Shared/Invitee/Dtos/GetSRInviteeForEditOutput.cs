using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.Invitee.Dtos
{
    public class GetSRInviteeForEditOutput
    {
		public CreateOrEditSRInviteeDto SRInvitee { get; set; }

		public string UserTypeType { get; set;}

		public string EscrowClientName { get; set;}


    }
}