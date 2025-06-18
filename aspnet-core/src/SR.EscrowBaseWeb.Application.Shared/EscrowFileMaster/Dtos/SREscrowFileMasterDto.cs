using System;
using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.EscrowFileMaster.Dtos
{
    public class SREscrowFileMasterDto : EntityDto<long>
    {
        public bool? OtherAction { get; set; }

        public string OtherActionNote { get; set; }
    }
}