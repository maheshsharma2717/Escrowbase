using Abp.Application.Services.Dto;
using System;

namespace SR.EscrowBaseWeb.TagsAndFileMapping.Dtos
{
    public class GetAllTagsAndFileMappingsesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

    }
}