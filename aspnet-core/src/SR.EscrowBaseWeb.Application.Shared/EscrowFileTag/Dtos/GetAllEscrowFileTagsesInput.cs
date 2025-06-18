using Abp.Application.Services.Dto;
using System;

namespace SR.EscrowBaseWeb.EscrowFileTag.Dtos
{
    public class GetAllEscrowFileTagsesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

    }
}