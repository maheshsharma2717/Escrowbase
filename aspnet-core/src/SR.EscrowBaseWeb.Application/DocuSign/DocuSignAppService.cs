using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Timing;
using SR.EscrowBaseWeb.DocuSign.Dtos;
using SR.EscrowBaseWeb.E_SignRecords;
using System;
using System.Threading.Tasks;

namespace SR.EscrowBaseWeb.DocuSign
{
    public class DocuSignAppService : EscrowBaseWebAppServiceBase, IApplicationService
    {
        private readonly DocuSignService _docuSignService;
        private readonly IRepository<E_SignRecord, long> _esignRecordRepository;

        public DocuSignAppService(
            DocuSignService docuSignService,
            IRepository<E_SignRecord, long> esignRecordRepository)
        {
            _docuSignService = docuSignService;
            _esignRecordRepository = esignRecordRepository;
        }

        public async Task<string> SendEnvelopeAndGetUrl(CreateOrEditDocuSignDto input)
        {
            var pdfBytes = Convert.FromBase64String(input.Base64Pdf);

            var result = await _docuSignService.SendEnvelopeForEmbeddedSigningAsync(
                input.Email,
                input.Name,
                pdfBytes);

            var entity = new E_SignRecord
            {
                EmailId = input.Email,
                EmbeddedURL = result.SigningUrl,
                EmbeddedToken = "", // optional, fill if you track token
                FolderId = input.FolderId,
                FolderName = input.FolderName,
                FolderPassword = input.FolderPassword,
                PartyId = input.PartyId,
                ContractId = input.ContractId,
                CompanyId = AbpSession.TenantId ?? 1, // fallback if tenant null
                DocumentId = input.DocumentId,
                FileName = input.FileName,
                Status = "sent",
                EsignCompanyCode = 2, // 2 for DocuSign, 1 for Zoho (as example)
                FullFilePath = input.FullFilePath,
                RequestId = result.EnvelopeId,
                ZohoAction = null,
                Signin_percentage = "0"
            };

            await _esignRecordRepository.InsertAsync(entity);

            return result.SigningUrl;
        }
    }
}