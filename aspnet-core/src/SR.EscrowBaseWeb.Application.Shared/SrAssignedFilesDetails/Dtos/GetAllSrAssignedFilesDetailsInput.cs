using Abp.Application.Services.Dto;
using System;

namespace SR.EscrowBaseWeb.SrAssignedFilesDetails.Dtos
{
    public class GetAllSrAssignedFilesDetailsInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public long? MaxUserIdFilter { get; set; }
        public long? MinUserIdFilter { get; set; }

        public long? MaxEOIdFilter { get; set; }
        public long? MinEOIdFilter { get; set; }

        public string FileNameFilter { get; set; }

        public string ReadStatusFilter { get; set; }

        public string SigningStatusFilter { get; set; }

        public string InputStatusFilter { get; set; }

        public DateTime? MaxUpdatedOnFilter { get; set; }
        public DateTime? MinUpdatedOnFilter { get; set; }
    }
}