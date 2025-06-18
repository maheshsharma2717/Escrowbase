using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using SR.EscrowBaseWeb.SrAssignedFilesDetails.Dtos;
using SR.EscrowBaseWeb.Dto;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using SR.EscrowBaseWeb.Storage;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using SR.EscrowBaseWeb.E_SignRecords;
using SR.EscrowBaseWeb.SRFileMapping;
using SR.EscrowBaseWeb.SREscrowFileHistory.Dtos;
using SR.EscrowBaseWeb.SREscrowFileHistory;
using SR.EscrowBaseWeb.EscrowFileMaster;

namespace SR.EscrowBaseWeb.SrAssignedFilesDetails
{
    //[AbpAuthorize(AppPermissions.Pages_SrAssignedFilesDetails)]
    public class SrAssignedFilesDetailsAppService : EscrowBaseWebAppServiceBase, ISrAssignedFilesDetailsAppService
    {
        private readonly IRepository<SrAssignedFilesDetail, long> _srAssignedFilesDetailRepository;
        IWebHostEnvironment _hostingEnvironment;
        private readonly IRepository<E_SignRecord, long> _e_SignRecordRepository;
        private readonly IRepository<SrFileMapping> _srFileMappingRepository;
        private readonly IRepository<EscrowFileHistory, long> _escrowFileHistoryRepository;
        private readonly IRepository<SREscrowFileMaster, long> _srEscrowFileMasterRepository;


        public SrAssignedFilesDetailsAppService(IRepository<SrAssignedFilesDetail, long> srAssignedFilesDetailRepository,
            IWebHostEnvironment hostingEnvironment,
            IRepository<E_SignRecord, long> e_SignRecordRepository,
            IRepository<SrFileMapping> srFileMappingRepository,
            IRepository<EscrowFileHistory, long> escrowFileHistoryRepository,
             IRepository<SREscrowFileMaster, long> srEscrowFileMasterRepository
            )
        {
            _srAssignedFilesDetailRepository = srAssignedFilesDetailRepository;
            _hostingEnvironment = hostingEnvironment;
            _e_SignRecordRepository = e_SignRecordRepository;
            _srFileMappingRepository = srFileMappingRepository;
            _escrowFileHistoryRepository = escrowFileHistoryRepository;
            _srEscrowFileMasterRepository = srEscrowFileMasterRepository;

        }

        public async Task<PagedResultDto<GetSrAssignedFilesDetailForViewDto>> GetAll(GetAllSrAssignedFilesDetailsInput input)
        {

            var filteredSrAssignedFilesDetails = _srAssignedFilesDetailRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.FileName.Contains(input.Filter) || e.ReadStatus.Contains(input.Filter) || e.SigningStatus.Contains(input.Filter) || e.InputStatus.Contains(input.Filter))
                        .WhereIf(input.MinUserIdFilter != null, e => e.UserId >= input.MinUserIdFilter)
                        .WhereIf(input.MaxUserIdFilter != null, e => e.UserId <= input.MaxUserIdFilter)
                        .WhereIf(input.MinEOIdFilter != null, e => e.EOId >= input.MinEOIdFilter)
                        .WhereIf(input.MaxEOIdFilter != null, e => e.EOId <= input.MaxEOIdFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.FileNameFilter), e => e.FileName == input.FileNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ReadStatusFilter), e => e.ReadStatus == input.ReadStatusFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.SigningStatusFilter), e => e.SigningStatus == input.SigningStatusFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.InputStatusFilter), e => e.InputStatus == input.InputStatusFilter)
                        .WhereIf(input.MinUpdatedOnFilter != null, e => e.UpdatedOn >= input.MinUpdatedOnFilter)
                        .WhereIf(input.MaxUpdatedOnFilter != null, e => e.UpdatedOn <= input.MaxUpdatedOnFilter);                      


            var pagedAndFilteredSrAssignedFilesDetails = filteredSrAssignedFilesDetails
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var srAssignedFilesDetails = from o in pagedAndFilteredSrAssignedFilesDetails
                                         select new
                                         {

                                             Id = o.Id,                                           
                                         };

            var totalCount = await filteredSrAssignedFilesDetails.CountAsync();

            var dbList = await srAssignedFilesDetails.ToListAsync();
            var results = new List<GetSrAssignedFilesDetailForViewDto>();

            foreach (var o in dbList)
            {
                var res = new GetSrAssignedFilesDetailForViewDto()
                {
                    SrAssignedFilesDetail = new SrAssignedFilesDetailDto
                    {

                        Id = o.Id,                 
                    }
                };

                results.Add(res);
            }

            return new PagedResultDto<GetSrAssignedFilesDetailForViewDto>(
                totalCount,
                results
            );

        }

        public async Task<GetSrAssignedFilesDetailForViewDto> GetSrAssignedFilesDetailForView(long id)
        {
            var srAssignedFilesDetail = await _srAssignedFilesDetailRepository.GetAsync(id);

            var output = new GetSrAssignedFilesDetailForViewDto { SrAssignedFilesDetail = ObjectMapper.Map<SrAssignedFilesDetailDto>(srAssignedFilesDetail) };

            return output;
        }

        
        public async Task<GetSrAssignedFilesDetailForEditOutput> GetSrAssignedFilesDetailForEdit(EntityDto<long> input)
        {
            var srAssignedFilesDetail = await _srAssignedFilesDetailRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetSrAssignedFilesDetailForEditOutput { SrAssignedFilesDetail = ObjectMapper.Map<CreateOrEditSrAssignedFilesDetailDto>(srAssignedFilesDetail) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditSrAssignedFilesDetailDto input)
        {
            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        //[AbpAuthorize(AppPermissions.Pages_SrAssignedFilesDetails_Create)]
        protected virtual async Task Create(CreateOrEditSrAssignedFilesDetailDto input)
        {
            var srAssignedFilesDetail = ObjectMapper.Map<SrAssignedFilesDetail>(input);

            await _srAssignedFilesDetailRepository.InsertAsync(srAssignedFilesDetail);

        }

       
        protected virtual async Task Update(CreateOrEditSrAssignedFilesDetailDto input)
        {
            var srAssignedFilesDetail = await _srAssignedFilesDetailRepository.FirstOrDefaultAsync((long)input.Id);
            ObjectMapper.Map(input, srAssignedFilesDetail);

        }

      
        public async Task Delete(EntityDto<long> input)
        {
            await _srAssignedFilesDetailRepository.DeleteAsync(input.Id);
        }

        
        public async Task<bool> RenameFileNameAsync(RenameModel reNameData)
        {
            var dbSrFileList = await _srAssignedFilesDetailRepository.GetAll().Where(x=>x.SrEscrowFileMasterId == reNameData.Id).ToListAsync();
            if (dbSrFileList.Count >0)
            {
                reNameData.ParentPath = reNameData.ParentPath.Replace("/", "\\");
                string filee = Path.Combine(_hostingEnvironment.WebRootPath, "Common", "Paperless");
                string fullFilePath = Path.Combine(filee, reNameData.ParentPath, dbSrFileList.FirstOrDefault().FileName);

                FileInfo file = new FileInfo(fullFilePath);
                if (file.Exists)
                {
                    string newFilePath = Path.Combine(filee, reNameData.ParentPath, reNameData.NewFileName);
                    file.MoveTo(newFilePath);

                    // updating E_SignRecord
                    var dbEsignRecoreds = await _e_SignRecordRepository.GetAll().FirstOrDefaultAsync(x => x.FileName == dbSrFileList.FirstOrDefault().FileName);
                    if (dbEsignRecoreds != null)
                    {
                        dbEsignRecoreds.FileName = reNameData.NewFileName;
                        dbEsignRecoreds.FullFilePath = "Common\\Paperless\\" + reNameData.ParentPath + "\\" + reNameData.NewFileName;
                        _e_SignRecordRepository.Update(dbEsignRecoreds);
                    }

                    // updating   _srFileMapping
                    var dbSRFileMapping = await _srFileMappingRepository.GetAll().Where(x => x.SrEscrowFileMasterId == reNameData.Id).ToListAsync();
                    foreach(var item in dbSRFileMapping)
                    {
                        item.FileName = newFilePath;
                        await _srFileMappingRepository.UpdateAsync(item);
                    }
                    string oldFileName = string.Empty;
                    foreach (var dbSrFile in dbSrFileList) {
                        // updating srAssignedFilesDetail
                        oldFileName = dbSrFile.FileName;
                        dbSrFile.FileName = reNameData.NewFileName;
                        await _srAssignedFilesDetailRepository.UpdateAsync(dbSrFile);
                    }
                    var dbFileMaster = _srEscrowFileMasterRepository.Get(reNameData.Id);
                    if(dbFileMaster != null)
                    {
                        dbFileMaster.FileFullName = newFilePath;
                        dbFileMaster.FileShortName = System.IO.Path.GetFileName(newFilePath);
                       await _srEscrowFileMasterRepository.UpdateAsync(dbFileMaster);
                    }

                    EscrowFileHistory escrowFileHistory = new EscrowFileHistory();
                    escrowFileHistory.SrEscrowFileMasterId = reNameData.Id;
                    escrowFileHistory.FileFullPath = reNameData.NewFileName;
                    escrowFileHistory.UserId = AbpSession.UserId;
                    escrowFileHistory.Message = "File  Renamed From " + oldFileName + " to " +reNameData.NewFileName;
                    escrowFileHistory.ActionType = "File Renamed after Esign";
                    escrowFileHistory.CreatedAt = DateTime.Now;
                    await _escrowFileHistoryRepository.InsertAsync(escrowFileHistory);
                }
            }
            return false;
        }

    }
    public class RenameModel
    {
        public long Id { get; set; }
        //public string EscrowId { get; set; }
        public string NewFileName { get; set; }
        public string ParentPath { get; set; }
    }
}