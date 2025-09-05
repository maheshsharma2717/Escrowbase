using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using SR.EscrowBaseWeb.E_SignRecords.Dtos;
using SR.EscrowBaseWeb.Dto;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using SR.EscrowBaseWeb.Storage;
using Abp.Domain.Uow;

namespace SR.EscrowBaseWeb.E_SignRecords
{
    //[AbpAuthorize(AppPermissions.Pages_E_SignRecords)]
    public class E_SignRecordsAppService : EscrowBaseWebAppServiceBase, IE_SignRecordsAppService
    {
        private readonly IRepository<E_SignRecord, long> _e_SignRecordRepository;
        private IUnitOfWorkManager _unitOfWorkManager;

        public E_SignRecordsAppService(IRepository<E_SignRecord, long> e_SignRecordRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _e_SignRecordRepository = e_SignRecordRepository;
            _unitOfWorkManager = unitOfWorkManager;

        }

        public async Task<PagedResultDto<GetE_SignRecordForViewDto>> GetAll(GetAllE_SignRecordsInput input)
        {

            var filteredE_SignRecords = _e_SignRecordRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.EmailId.Contains(input.Filter) || e.EmbeddedURL.Contains(input.Filter) || e.EmbeddedToken.Contains(input.Filter) || e.FolderName.Contains(input.Filter) || e.FolderPassword.Contains(input.Filter) || e.FileName.Contains(input.Filter) || e.Status.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.EmailIdFilter), e => e.EmailId == input.EmailIdFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.EmbeddedURLFilter), e => e.EmbeddedURL == input.EmbeddedURLFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.EmbeddedTokenFilter), e => e.EmbeddedToken == input.EmbeddedTokenFilter)
                        .WhereIf(input.MinFolderIdFilter != null, e => e.FolderId >= input.MinFolderIdFilter)
                        .WhereIf(input.MaxFolderIdFilter != null, e => e.FolderId <= input.MaxFolderIdFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.FolderNameFilter), e => e.FolderName == input.FolderNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.FolderPasswordFilter), e => e.FolderPassword == input.FolderPasswordFilter)
                        .WhereIf(input.MinPartyIdFilter != null, e => e.PartyId >= input.MinPartyIdFilter)
                        .WhereIf(input.MaxPartyIdFilter != null, e => e.PartyId <= input.MaxPartyIdFilter)
                        .WhereIf(input.MinContractIdFilter != null, e => e.ContractId >= input.MinContractIdFilter)
                        .WhereIf(input.MaxContractIdFilter != null, e => e.ContractId <= input.MaxContractIdFilter)
                        .WhereIf(input.MinCompanyIdFilter != null, e => e.CompanyId >= input.MinCompanyIdFilter)
                        .WhereIf(input.MaxCompanyIdFilter != null, e => e.CompanyId <= input.MaxCompanyIdFilter)
                        .WhereIf(input.MinDocumentIdFilter != null, e => e.DocumentId >= input.MinDocumentIdFilter)
                        .WhereIf(input.MaxDocumentIdFilter != null, e => e.DocumentId <= input.MaxDocumentIdFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.StatusFilter), e => e.Status == input.StatusFilter);

            var pagedAndFilteredE_SignRecords = filteredE_SignRecords
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var e_SignRecords = from o in pagedAndFilteredE_SignRecords
                                select new
                                {
                                    o.EmailId,
                                    o.EmbeddedURL,
                                    o.EmbeddedToken,
                                    o.FolderId,
                                    o.FolderName,
                                    o.FolderPassword,
                                    o.PartyId,
                                    o.ContractId,
                                    o.CompanyId,
                                    o.DocumentId,
                                    o.Status,
                                    Id = o.Id,
                                    o.EsignCompanyCode
                                };

            var totalCount = await filteredE_SignRecords.CountAsync();

            var dbList = await e_SignRecords.ToListAsync();
            var results = new List<GetE_SignRecordForViewDto>();

            foreach (var o in dbList)
            {
                var res = new GetE_SignRecordForViewDto()
                {
                    E_SignRecord = new E_SignRecordDto
                    {
                        EmailId = o.EmailId,
                        EmbeddedURL = o.EmbeddedURL,
                        EmbeddedToken = o.EmbeddedToken,
                        FolderId = o.FolderId,
                        FolderName = o.FolderName,
                        FolderPassword = o.FolderPassword,
                        PartyId = o.PartyId,
                        ContractId = o.ContractId,
                        CompanyId = o.CompanyId,
                        DocumentId = o.DocumentId,
                        Status = o.Status,
                        Id = o.Id,
                        EsignCompanyCode = o.EsignCompanyCode
                    }
                };

                results.Add(res);
            }

            return new PagedResultDto<GetE_SignRecordForViewDto>(
                totalCount,
                results
            );

        }

        public List<E_SignRecord> GetAllE_Sign(int EsignCompanyCode)
        {

            return _e_SignRecordRepository.GetAll().Where(x => x.EsignCompanyCode == EsignCompanyCode).ToList();

        }




        public async Task<GetE_SignRecordForViewDto> GetE_SignRecordForView(long id)
        {
            var e_SignRecord = await _e_SignRecordRepository.GetAsync(id);

            var output = new GetE_SignRecordForViewDto { E_SignRecord = ObjectMapper.Map<E_SignRecordDto>(e_SignRecord) };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_E_SignRecords_Edit)]
        public async Task<GetE_SignRecordForEditOutput> GetE_SignRecordForEdit(EntityDto<long> input)
        {
            var e_SignRecord = await _e_SignRecordRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetE_SignRecordForEditOutput { E_SignRecord = ObjectMapper.Map<CreateOrEditE_SignRecordDto>(e_SignRecord) };

            return output;
        }

        public async Task<string> CreateOrEdit(CreateOrEditE_SignRecordDto input)
        {
            if (input.Id == null)
            {
                return await Create(input);
            }
            else
            {
                return await Update(input);
            }
        }

        //[AbpAuthorize(AppPermissions.Pages_E_SignRecords_Create)]
        protected virtual async Task<string> Create(CreateOrEditE_SignRecordDto input)
        {
            try
            {
                using (var unit = _unitOfWorkManager.Begin())
                {
                    var e_SignRecord = ObjectMapper.Map<E_SignRecord>(input);

                    await _e_SignRecordRepository.InsertAsync(e_SignRecord);
                    await CurrentUnitOfWork.SaveChangesAsync();
                    unit.Complete();  // Commit the transaction.

                    return "success";
                }
            }
            catch (Exception ex)
            {
                // Log the exception details (consider using a logging framework)
                // Example: _logger.LogError(ex, "Error creating E_SignRecord");

                // Return a generic error message
                return "An error occurred while processing your request." + ex.InnerException;
            }
        }

        //[AbpAuthorize(AppPermissions.Pages_E_SignRecords_Edit)]
        protected virtual async Task<string> Update(CreateOrEditE_SignRecordDto input)
        {


            var e_SignRecord = await _e_SignRecordRepository.FirstOrDefaultAsync((long)input.Id);
            ObjectMapper.Map(input, e_SignRecord);
            await CurrentUnitOfWork.SaveChangesAsync();
            return "sucess";



        }

        public virtual async void UpdateEsignRecord(long Id, string embededURl)
        {
            try
            {
                var dbESignRecord = await _e_SignRecordRepository.GetAll().Where(x => x.Id == Id).FirstOrDefaultAsync();
                if (dbESignRecord != null)
                {
                    dbESignRecord.EmbeddedURL = embededURl;
                }
                await _e_SignRecordRepository.UpdateAsync(dbESignRecord);
                await CurrentUnitOfWork.SaveChangesAsync();

            }
            catch (Exception ex)
            {
            }

        }

        public virtual async Task<bool> UpdateEsignStatus(long Id, string Status, string signin_percentage)
        {
            try
            {
                // using (var unit = _unitOfWorkManager.Begin())
                //{
                var dbESignRecord = await _e_SignRecordRepository.GetAll().Where(x => x.Id == Id).FirstOrDefaultAsync();
                if (dbESignRecord != null)
                {
                    dbESignRecord.Status = Status;
                    dbESignRecord.Signin_percentage = signin_percentage;
                }
                await _e_SignRecordRepository.UpdateAsync(dbESignRecord);
                return true;
                //  await CurrentUnitOfWork.SaveChangesAsync();
                // unit.Complete();
                // }

            }
            catch (Exception ex)
            {

            }
            return false;


        }

        [AbpAuthorize(AppPermissions.Pages_E_SignRecords_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            await _e_SignRecordRepository.DeleteAsync(input.Id);
        }

    }
}