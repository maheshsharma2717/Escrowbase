using Abp.Application.Services.Dto;
using System;

namespace SR.EscrowBaseWeb.EscrowDirectMessage.Dtos
{
    public class GetAllEscrowDirectMessageDetailsesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string UserNameFilter { get; set; }

        public string UserName2Filter { get; set; }

    }
}