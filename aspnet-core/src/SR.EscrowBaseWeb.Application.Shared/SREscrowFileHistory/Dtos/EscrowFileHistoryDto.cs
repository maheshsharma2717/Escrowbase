using System;
using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.SREscrowFileHistory.Dtos
{
    public class EscrowFileHistoryDto : EntityDto<long>
    {

        public long? UserId { get; set; }

         
        public virtual string Message { get; set; }

         
        public virtual string ActionType { get; set; }

        public virtual string FileFullPath { get; set; }

        public virtual DateTime CreatedAt { get; set; }
        public virtual long? SRAssignedFilesId { get; set; }
    }
}