using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.SREscrowFileHistory.Dtos
{
    public class CreateOrEditEscrowFileHistoryDto : EntityDto<long?>
    {

        public long? UserId { get; set; }
         
        public virtual string Message { get; set; }

         
        public virtual string ActionType { get; set; }

        public virtual string FileFullPath { get; set; }
        public virtual long? SrEscrowFileMasterId { get; set; }

    }
}