using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.EscrowHistory.Dtos
{
    public class CreateOrEditEscrowAccessHistoryDto : EntityDto<int?>
    {

        public DateTime CreatedAt { get; set; }

        public long? UserId { get; set; }

        public int? EscrowId { get; set; }

    }
}