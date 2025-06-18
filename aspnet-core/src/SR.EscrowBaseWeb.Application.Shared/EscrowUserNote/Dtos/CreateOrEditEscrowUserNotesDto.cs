using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.EscrowUserNote.Dtos
{
    public class CreateOrEditEscrowUserNotesDto : EntityDto<int?>
    {

        public string Message { get; set; }

        public string EscrowNumber { get; set; }

        public DateTime? CreatedAt { get; set; }

        public long? CreatedBy { get; set; }

    }
}