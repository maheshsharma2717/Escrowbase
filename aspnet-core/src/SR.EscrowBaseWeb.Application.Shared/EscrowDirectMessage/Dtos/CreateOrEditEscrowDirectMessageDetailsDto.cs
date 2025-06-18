using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SR.EscrowBaseWeb.EscrowDirectMessage.Dtos
{
    public class CreateOrEditEscrowDirectMessageDetailsDto : EntityDto<long?>
    {
        public List<ReminderTypeListEscrowDirectMessage> ReminderType { get; set; }
        public string Message { get; set; }

        public DateTime? CreatedDate { get; set; }

        public bool Status { get; set; }

        public long? EscrowUserId { get; set; }

        public long? SenderUserId { get; set; }
        public string EscrowNumber { get; set; }

        public string UserType { get; set; }

    }

    public class ReminderTypeListEscrowDirectMessage
    {
        public string ReminderType { get; set; }
    }
}