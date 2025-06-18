using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.EscrowFileTag.Dtos
{
    public class GetEscrowFileTagsForEditOutput
    {
        public CreateOrEditEscrowFileTagsDto EscrowFileTags { get; set; }

    }
}