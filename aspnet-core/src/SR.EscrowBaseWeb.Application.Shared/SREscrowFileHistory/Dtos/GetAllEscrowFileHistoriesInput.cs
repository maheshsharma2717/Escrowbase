using Abp.Application.Services.Dto;
using System;

namespace SR.EscrowBaseWeb.SREscrowFileHistory.Dtos
{
    public class GetAllEscrowFileHistoriesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string UserNameFilter { get; set; }

        public long SrFileMappingId { get; set; }

    }
}