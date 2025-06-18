using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.SrInvitationRecords.Dtos
{
    public class GetSrInvitationRecordForEditOutput
    {
		public CreateOrEditSrInvitationRecordDto SrInvitationRecord { get; set; }

		public string UserName { get; set;}


    }
}