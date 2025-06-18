using System;
using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.EscrowFileTag.Dtos
{
    public class EscrowFileTagsDto : EntityDto
    {
        public string TagDescription { get; set; }

        public string TagColor { get; set; }

        public string EscrowNumber { get; set; }

       // public DateTime? CreatedAt { get; set; }

        public string FileName { get; set; }

    }
}