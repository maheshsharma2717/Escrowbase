using Abp.Application.Services.Dto;
using System;

namespace SR.EscrowBaseWeb.EscrowUserNote.Dtos
{
    public class GetAllEscrowUserNotesesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string UserNameFilter { get; set; }

    }
}