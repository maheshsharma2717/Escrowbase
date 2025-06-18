using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.EscrowUserNote.Dtos
{
    public class GetEscrowUserNotesForEditOutput
    {
        public CreateOrEditEscrowUserNotesDto EscrowUserNotes { get; set; }

        public string UserName { get; set; }

    }
}