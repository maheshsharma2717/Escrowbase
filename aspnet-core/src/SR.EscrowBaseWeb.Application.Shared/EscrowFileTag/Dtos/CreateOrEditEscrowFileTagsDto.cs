using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.EscrowFileTag.Dtos
{
    public class CreateOrEditEscrowFileTagsDto : EntityDto<int?>
    {

        public string TagDescription { get; set; }

        public string TagColor { get; set; }

        public string EscrowNumber { get; set; }

        public DateTime CreatedAt { get; set; }

        public string FileName { get; set; }

    }
}