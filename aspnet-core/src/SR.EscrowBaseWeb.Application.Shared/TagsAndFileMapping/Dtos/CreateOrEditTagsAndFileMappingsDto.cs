using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.TagsAndFileMapping.Dtos
{
    public class CreateOrEditTagsAndFileMappingsDto : EntityDto<int?>
    {

        public int TagId { get; set; }

        public string FileName { get; set; }

     

    }
    public class DeleteTagsAndFileMappingsDto
    {

        public int TagId { get; set; }

        public string FileName { get; set; }

    }
}