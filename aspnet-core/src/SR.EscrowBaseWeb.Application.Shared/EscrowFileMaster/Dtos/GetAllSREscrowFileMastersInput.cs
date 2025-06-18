using Abp.Application.Services.Dto;
using System;

namespace SR.EscrowBaseWeb.EscrowFileMaster.Dtos
{
    public class GetAllSREscrowFileMastersInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

    }
}