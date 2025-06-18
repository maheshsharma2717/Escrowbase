using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using SR.EscrowBaseWeb.SrAssignedFilesDetails;

namespace SR.EscrowBaseWeb.EscrowFileReminder.Dtos
{
    public class CreateOrEditSrEscrowFileReminderDto : EntityDto<long?>
    {
        public List<ReminderTypeList> ReminderType { get; set; }

        public string SentTo { get; set; }

        public string ReminderText { get; set; }

        public string SentFrom { get; set; }

        public DateTime? CreatedAt { get; set; }

        public bool ReminderStatus { get; set; }

        public string SentToUserType { get; set; }

        public string SentFromUserType { get; set; }

        public long SREscrowFileMasterId { get; set; }

        public long? CreatedBy { get; set; }

        public string EscrowNumber { get; set; }
        
        public List<AssignedFileUser> assignedFileUser { get;set;}

    }
    public class ReminderTypeList
    {
        public string ReminderType { get; set; }
    }

    public class MessageSendResponse
    {
        public string UserName { get; set; }
        public string UserType { get; set; }
        public string MessageStatus { get; set; }
        public string ReminderType { get; set; }
    }
}