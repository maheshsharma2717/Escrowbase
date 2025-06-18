using System;
using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.EscrowUserNote.Dtos
{
    public class EscrowUserNotesDto : EntityDto
    {
        public string Message { get; set; }

        public string EscrowNumber { get; set; }

        public DateTime? CreatedAt { get; set; }

        public long? CreatedBy { get; set; }

    }
    public class EscrowUserNotesresponseDto : EntityDto
    {
        public string Message { get; set; }

        public string UserName { get; set; }

        public DateTime? CreatedAt { get; set; }

        public long? CreatedBy { get; set; }

    }
}