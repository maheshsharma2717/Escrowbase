using System;
using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.EscrowHistory.Dtos
{
    public class EscrowAccessHistoryDto : EntityDto
    {
        public DateTime CreatedAt { get; set; }

        public long? UserId { get; set; }

        public int? EscrowId { get; set; }

    }
}