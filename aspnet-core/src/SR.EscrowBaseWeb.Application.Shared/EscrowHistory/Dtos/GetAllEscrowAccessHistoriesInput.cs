using Abp.Application.Services.Dto;
using System;

namespace SR.EscrowBaseWeb.EscrowHistory.Dtos
{
    public class GetAllEscrowAccessHistoriesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public DateTime? MaxCreatedAtFilter { get; set; }
        public DateTime? MinCreatedAtFilter { get; set; }

        public string UserNameFilter { get; set; }

        public string EscrowClientNameFilter { get; set; }

    }
}