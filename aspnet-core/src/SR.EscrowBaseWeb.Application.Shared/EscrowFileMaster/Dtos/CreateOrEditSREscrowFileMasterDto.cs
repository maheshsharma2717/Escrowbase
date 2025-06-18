using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.EscrowFileMaster.Dtos
{
    public class CreateOrEditSREscrowFileMasterDto : EntityDto<long?>
    {

        public string FileFullName { get; set; }

        public string FileShortName { get; set; }

        public bool? OtherAction { get; set; }

        public string OtherActionNote { get; set; }

    }


}