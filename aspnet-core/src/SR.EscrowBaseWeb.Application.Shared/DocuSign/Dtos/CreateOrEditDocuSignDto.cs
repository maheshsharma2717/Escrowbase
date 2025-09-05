using System;

namespace SR.EscrowBaseWeb.DocuSign.Dtos
{
    public class CreateOrEditDocuSignDto
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Base64Pdf { get; set; }

        public long FolderId { get; set; }
        public string FolderName { get; set; }
        public string FolderPassword { get; set; }

        public int PartyId { get; set; }
        public int ContractId { get; set; }

        public long CompanyId { get; set; }
        public long DocumentId { get; set; }

        public string FileName { get; set; }
        public string FullFilePath { get; set; }

        // Extra added for docusign
        public string EscrowId { get; set; }
        public byte[] FileContent { get; set; }
        public string SignerEmail { get; set; }
        public string SignerName { get; set; }
    }
}
