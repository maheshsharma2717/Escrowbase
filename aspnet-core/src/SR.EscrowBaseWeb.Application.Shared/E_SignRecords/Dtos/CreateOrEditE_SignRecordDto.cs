using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.E_SignRecords.Dtos
{
    public class CreateOrEditE_SignRecordDto : EntityDto<long?>
    {

        public string EmailId { get; set; }

        public string EmbeddedURL { get; set; }

        public string EmbeddedToken { get; set; }

        public long FolderId { get; set; }

        public string FolderName { get; set; }

        public string FolderPassword { get; set; }

        public int PartyId { get; set; }

        public int ContractId { get; set; }

        public long CompanyId { get; set; }

        public long DocumentId { get; set; }

        public string FileName { get; set; }

        public string Status { get; set; }

        public int EsignCompanyCode { get; set; }

        public string FullFilePath { get; set; }

        public string RequestId { get; set; }

        public string ZohoAction { get; set; }

    }
}