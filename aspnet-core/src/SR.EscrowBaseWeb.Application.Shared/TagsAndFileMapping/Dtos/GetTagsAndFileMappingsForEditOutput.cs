using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.TagsAndFileMapping.Dtos
{
    public class GetTagsAndFileMappingsForEditOutput
    {
        public CreateOrEditTagsAndFileMappingsDto TagsAndFileMappings { get; set; }

    }
}