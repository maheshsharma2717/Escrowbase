//using System;
//using Abp.Application.Services.Dto;

//namespace SR.EscrowBaseWeb.SrAssignedFilesDetails.Dtos
//{
//    public class SrAssignedFilesDetailDto : EntityDto<long>
//    {

//    }
//}




using System;
using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.SrAssignedFilesDetails.Dtos
{
    public class SrAssignedFilesDetailDto : EntityDto<long>
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
