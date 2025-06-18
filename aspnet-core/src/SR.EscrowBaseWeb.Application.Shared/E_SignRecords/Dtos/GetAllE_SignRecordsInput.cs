using Abp.Application.Services.Dto;
using System;

namespace SR.EscrowBaseWeb.E_SignRecords.Dtos
{
    public class GetAllE_SignRecordsInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string EmailIdFilter { get; set; }

        public string EmbeddedURLFilter { get; set; }

        public string EmbeddedTokenFilter { get; set; }

        public long? MaxFolderIdFilter { get; set; }
        public long? MinFolderIdFilter { get; set; }

        public string FolderNameFilter { get; set; }

        public string FolderPasswordFilter { get; set; }

        public int? MaxPartyIdFilter { get; set; }
        public int? MinPartyIdFilter { get; set; }

        public int? MaxContractIdFilter { get; set; }
        public int? MinContractIdFilter { get; set; }

        public long? MaxCompanyIdFilter { get; set; }
        public long? MinCompanyIdFilter { get; set; }

        public long? MaxDocumentIdFilter { get; set; }
        public long? MinDocumentIdFilter { get; set; }

        public string StatusFilter { get; set; }

    }
}