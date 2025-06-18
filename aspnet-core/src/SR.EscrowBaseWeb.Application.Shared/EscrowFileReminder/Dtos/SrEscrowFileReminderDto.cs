using System;
using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.EscrowFileReminder.Dtos
{
    public class SrEscrowFileReminderDto : EntityDto<long>
    {
        public string ReminderType { get; set; }

        public string SentTo { get; set; }

        public string ReminderText { get; set; }

        public string SentFrom { get; set; }

        public DateTime? CreatedAt { get; set; }

        public bool ReminderStatus { get; set; }

        public string SentToUserType { get; set; }

        public string SentFromUserType { get; set; }

        public long? SREscrowFileMasterId { get; set; }

        public long? CreatedBy { get; set; }

    }
}