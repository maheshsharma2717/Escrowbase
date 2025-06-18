using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.EscrowFileReminder.Dtos
{
    public class GetSrEscrowFileReminderForEditOutput
    {
        public CreateOrEditSrEscrowFileReminderDto SrEscrowFileReminder { get; set; }

        public string SREscrowFileMasterFileFullName { get; set; }

        public string UserName { get; set; }

    }
}