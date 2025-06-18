using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.SrAssignedFilesDetails.Dtos
{
    public class CreateOrEditSrAssignedFilesDetailDto : EntityDto<long?>
    {

        public long UserId { get; set; }

        public long? EOId { get; set; }

        public string FileName { get; set; }

        public string ReadStatus { get; set; }

        public string SigningStatus { get; set; }

        public string InputStatus { get; set; }

        public DateTime UpdatedOn { get; set; }

    }
}