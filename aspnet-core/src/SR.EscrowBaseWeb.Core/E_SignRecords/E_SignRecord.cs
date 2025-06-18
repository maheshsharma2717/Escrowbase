using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace SR.EscrowBaseWeb.E_SignRecords
{
    [Table("E_SignRecords")]
    public class E_SignRecord : Entity<long>
    {

        public virtual string EmailId { get; set; }

        public virtual string EmbeddedURL { get; set; }

        public virtual string EmbeddedToken { get; set; }

        public virtual long FolderId { get; set; }

        public virtual string FolderName { get; set; }

        public virtual string FolderPassword { get; set; }

        public virtual int PartyId { get; set; }

        public virtual int ContractId { get; set; }

        public virtual long CompanyId { get; set; }

        public virtual long DocumentId { get; set; }

        public virtual string FileName { get; set; }

        public virtual string Status { get; set; }

        public virtual int EsignCompanyCode { get; set; }

        public virtual string FullFilePath { get; set; }

        public virtual string RequestId { get; set; }

        public virtual string ZohoAction { get; set; }

        public virtual string Signin_percentage { get; set; }

    }
}