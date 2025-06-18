using System;
using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.EscrowDirectMessage.Dtos
{
    public class EscrowDirectMessageDetailsDto : EntityDto<long>
    {

        public long? EscrowUserId { get; set; }

        public long? SenderUserId { get; set; }

        public string Message { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string Status { get; set; }

        public string MessageType { get; set; }

    }
}