using SR.EscrowBaseWeb.Authorization.Users;

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using SR.EscrowBaseWeb.SREscrowFileHistory.Dtos;
using SR.EscrowBaseWeb.Dto;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using SR.EscrowBaseWeb.Storage;
using SR.EscrowBaseWeb.SrAssignedFilesDetails;

namespace SR.EscrowBaseWeb.SREscrowFileHistory
{
    //[AbpAuthorize(AppPermissions.Pages_EscrowFileHistories)]
    public class EscrowFileHistoriesAppService : EscrowBaseWebAppServiceBase, IEscrowFileHistoriesAppService
    {
        private readonly IRepository<EscrowFileHistory, long> _escrowFileHistoryRepository;
        private readonly IRepository<SrAssignedFilesDetail, long> _srAssignedFilesDetailRepository;
        private readonly IRepository<User, long> _lookup_userRepository;

        public EscrowFileHistoriesAppService(IRepository<EscrowFileHistory, long> escrowFileHistoryRepository, IRepository<User, long> lookup_userRepository,
            IRepository<SrAssignedFilesDetail, long> srAssignedFilesDetailRepository
            )
        {
            _escrowFileHistoryRepository = escrowFileHistoryRepository;
            _srAssignedFilesDetailRepository = srAssignedFilesDetailRepository;
            _lookup_userRepository = lookup_userRepository;

        }

        public async Task<PagedResultDto<GetEscrowFileHistoryForViewDto>> GetAll(GetAllEscrowFileHistoriesInput input)
        {
           
            var filteredEscrowFileHistories = _escrowFileHistoryRepository.GetAll()
                        .Include(e => e.UserFk)
                        .Where(x=> x.SrEscrowFileMasterId == input.SrFileMappingId)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Message.Contains(input.Filter) || e.ActionType.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserFk != null && e.UserFk.Name == input.UserNameFilter);

            var pagedAndFilteredEscrowFileHistories = filteredEscrowFileHistories
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var escrowFileHistories = from o in pagedAndFilteredEscrowFileHistories
                                      join o1 in _lookup_userRepository.GetAll() on o.UserId equals o1.Id into j1
                                      from s1 in j1.DefaultIfEmpty()

                                      select new
                                      {
                                          Id = o.Id,
                                         // UserName = s1 == null || s1.User == null ? "" : s1.Name.ToString(),
                                          UserName = s1 == null || s1.UserName == null ? "" : s1.UserName.ToString(),
                                          Message = o.Message,
                                          ActionType = o.ActionType,
                                          CreatedAt = o.CreatedAt,
                                          SRAssignedFilesId = o.SrEscrowFileMasterId

                                      };

            var totalCount = await filteredEscrowFileHistories.CountAsync();

            var dbList = await escrowFileHistories.ToListAsync();
            var results = new List<GetEscrowFileHistoryForViewDto>();

            foreach (var o in dbList)
            {
                var res = new GetEscrowFileHistoryForViewDto()
                {
                    EscrowFileHistory = new EscrowFileHistoryDto
                    {

                        Id = o.Id,
                        Message = o.Message,
                        ActionType = o.ActionType,
                        CreatedAt = o.CreatedAt,
                        SRAssignedFilesId = o.SRAssignedFilesId
                    },
                    UserName = o.UserName
                };

                results.Add(res);
            }

            return new PagedResultDto<GetEscrowFileHistoryForViewDto>(
                totalCount,
                results.OrderByDescending(o => o.EscrowFileHistory.CreatedAt).ToList()
            ); 

        }

        public async Task<GetEscrowFileHistoryForViewDto> GetEscrowFileHistoryForView(long id)
        {
            var escrowFileHistory = await _escrowFileHistoryRepository.GetAsync(id);

            var output = new GetEscrowFileHistoryForViewDto { EscrowFileHistory = ObjectMapper.Map<EscrowFileHistoryDto>(escrowFileHistory) };

            if (output.EscrowFileHistory.UserId != null)
            {
                var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.EscrowFileHistory.UserId);
                output.UserName = _lookupUser?.Name?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_EscrowFileHistories_Edit)]
        public async Task<GetEscrowFileHistoryForEditOutput> GetEscrowFileHistoryForEdit(EntityDto<long> input)
        {
            var escrowFileHistory = await _escrowFileHistoryRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetEscrowFileHistoryForEditOutput { EscrowFileHistory = ObjectMapper.Map<CreateOrEditEscrowFileHistoryDto>(escrowFileHistory) };

            if (output.EscrowFileHistory.UserId != null)
            {
                var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.EscrowFileHistory.UserId);
                output.UserName = _lookupUser?.Name?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditEscrowFileHistoryDto input)
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

        //[AbpAuthorize(AppPermissions.Pages_EscrowFileHistories_Create)]
        protected virtual async Task Create(CreateOrEditEscrowFileHistoryDto input)
        {
            try
            {

                var escrowFileHistory = ObjectMapper.Map<EscrowFileHistory>(input);
                escrowFileHistory.CreatedAt = DateTime.Now;
                await _escrowFileHistoryRepository.InsertAsync(escrowFileHistory);
            }catch(Exception ex)
            {

            }

        }

        //[AbpAuthorize(AppPermissions.Pages_EscrowFileHistories_Edit)]
        protected virtual async Task Update(CreateOrEditEscrowFileHistoryDto input)
        {
            var escrowFileHistory = await _escrowFileHistoryRepository.FirstOrDefaultAsync((long)input.Id);
            ObjectMapper.Map(input, escrowFileHistory);

        }

        //[AbpAuthorize(AppPermissions.Pages_EscrowFileHistories_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            await _escrowFileHistoryRepository.DeleteAsync(input.Id);
        }

        //[AbpAuthorize(AppPermissions.Pages_EscrowFileHistories)]
        public async Task<PagedResultDto<EscrowFileHistoryUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_userRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Name != null && e.Name.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var userList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<EscrowFileHistoryUserLookupTableDto>();
            foreach (var user in userList)
            {
                lookupTableDtoList.Add(new EscrowFileHistoryUserLookupTableDto
                {
                    Id = user.Id,
                    DisplayName = user.Name?.ToString()
                });
            }

            return new PagedResultDto<EscrowFileHistoryUserLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        public async Task<long?> GetUserIdFromSession()
        {
            return AbpSession.UserId;
        }


    }
}