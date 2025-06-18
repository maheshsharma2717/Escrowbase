using Abp.Application.Services.Dto;
using System;

namespace SR.EscrowBaseWeb.EscrowFileReminder.Dtos
{
    public class GetAllSrEscrowFileRemindersInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string SentFromUserTypeFilter { get; set; }

        public string SREscrowFileMasterFileFullNameFilter { get; set; }

        public string UserNameFilter { get; set; }

    }
}