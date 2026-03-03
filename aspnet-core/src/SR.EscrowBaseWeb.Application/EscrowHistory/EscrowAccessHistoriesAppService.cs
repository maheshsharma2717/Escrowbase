using SR.EscrowBaseWeb.Authorization.Users;
using SR.EscrowBaseWeb.SrEscrows;
using SR.EscrowBaseWeb.SrInvitationRecords;
using SR.EscrowBaseWeb.EscrowDetails;
using SR.EscrowBaseWeb.SrEscrows;
using SR.EscrowBaseWeb.SREnterprise;

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using SR.EscrowBaseWeb.EscrowHistory.Dtos;
using SR.EscrowBaseWeb.Dto;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using SR.EscrowBaseWeb.Storage;
using Microsoft.AspNetCore.Authorization;
using Abp.Runtime.Session;

namespace SR.EscrowBaseWeb.EscrowHistory
{
    public class EscrowAccessHistoriesAppService : EscrowBaseWebAppServiceBase, IEscrowAccessHistoriesAppService
    {
        private readonly IRepository<EscrowAccessHistory> _escrowAccessHistoryRepository;
        private readonly IRepository<User, long> _lookup_userRepository;
        private readonly IRepository<SrInvitationRecord, long> _srInvitationRecordRepository;
        private readonly IRepository<EscrowDetail, long> _escrowDetailRepository;
        private readonly IRepository<SrEscrow> _srEscrowRepository;
        private readonly IRepository<Enterprise> _enterpriseRepository;

        public EscrowAccessHistoriesAppService(
            IRepository<EscrowAccessHistory> escrowAccessHistoryRepository, 
            IRepository<User, long> lookup_userRepository, 
            IRepository<SrInvitationRecord, long> srInvitationRecordRepository,
            IRepository<EscrowDetail, long> escrowDetailRepository,
            IRepository<SrEscrow> srEscrowRepository,
            IRepository<Enterprise> enterpriseRepository,
            IAbpSession abpSession)
        {
            _escrowAccessHistoryRepository = escrowAccessHistoryRepository;
            _lookup_userRepository = lookup_userRepository;
            _srInvitationRecordRepository = srInvitationRecordRepository;
            _escrowDetailRepository = escrowDetailRepository;
            _srEscrowRepository = srEscrowRepository;
            _enterpriseRepository = enterpriseRepository;
            AbpSession = abpSession;
        }

        [AbpAuthorize(AppPermissions.Pages_EscrowAccessHistories)]
        public async Task<PagedResultDto<GetEscrowAccessHistoryForViewDto>> GetAll(GetAllEscrowAccessHistoriesInput input)
        {

            var filteredEscrowAccessHistories = _escrowAccessHistoryRepository.GetAll()
                        .Include(e => e.UserFk)
                        .Include(e => e.EscrowFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false)
                        .WhereIf(input.MinCreatedAtFilter != null, e => e.CreatedAt >= input.MinCreatedAtFilter)
                        .WhereIf(input.MaxCreatedAtFilter != null, e => e.CreatedAt <= input.MaxCreatedAtFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserFk != null && e.UserFk.Name == input.UserNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.EscrowClientNameFilter), e => e.EscrowFk != null && e.EscrowFk.EscrowNo == input.EscrowClientNameFilter);

            var pagedAndFilteredEscrowAccessHistories = filteredEscrowAccessHistories
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var escrowAccessHistories = from o in pagedAndFilteredEscrowAccessHistories
                                        join o1 in _lookup_userRepository.GetAll() on o.UserId equals o1.Id into j1
                                        from s1 in j1.DefaultIfEmpty()
                                        join o2 in _srEscrowRepository.GetAll() on o.EscrowId equals o2.Id into j2
                                        from s2 in j2.DefaultIfEmpty()
                                        select new GetEscrowAccessHistoryForViewDto()
                                        {
                                            EscrowAccessHistory = new EscrowAccessHistoryDto
                                            {
                                                CreatedAt = o.CreatedAt,
                                                Id = o.Id
                                            },
                                            UserName = s1 == null || s1.Name == null ? "" : s1.Name.ToString(),
                                            EscrowClientName = s2 == null || s2.EscrowNo == null ? "" : s2.EscrowNo.ToString()
                                        };

            var totalCount = await filteredEscrowAccessHistories.CountAsync();

            return new PagedResultDto<GetEscrowAccessHistoryForViewDto>(
                totalCount,
                await escrowAccessHistories.ToListAsync()
            );
        }

        [AbpAuthorize(AppPermissions.Pages_EscrowAccessHistories)]
        public async Task<GetEscrowAccessHistoryForViewDto> GetEscrowAccessHistoryForView(int id)
        {
            var escrowAccessHistory = await _escrowAccessHistoryRepository.GetAsync(id);

            var output = new GetEscrowAccessHistoryForViewDto { EscrowAccessHistory = ObjectMapper.Map<EscrowAccessHistoryDto>(escrowAccessHistory) };

            if (escrowAccessHistory.UserId != null)
            {
                var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)escrowAccessHistory.UserId);
                output.UserName = _lookupUser?.Name?.ToString();
            }

            if (escrowAccessHistory.EscrowId != null)
            {
                var _lookupEscrowClient = await _srEscrowRepository.FirstOrDefaultAsync((int)escrowAccessHistory.EscrowId);
                output.EscrowClientName = _lookupEscrowClient?.EscrowNo?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_EscrowAccessHistories_Edit)]
        public async Task<GetEscrowAccessHistoryForEditOutput> GetEscrowAccessHistoryForEdit(EntityDto input)
        {
            var escrowAccessHistory = await _escrowAccessHistoryRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetEscrowAccessHistoryForEditOutput { EscrowAccessHistory = ObjectMapper.Map<CreateOrEditEscrowAccessHistoryDto>(escrowAccessHistory) };

            if (escrowAccessHistory.UserId != null)
            {
                var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)escrowAccessHistory.UserId);
                output.UserName = _lookupUser?.Name?.ToString();
            }

            if (escrowAccessHistory.EscrowId != null)
            {
                var _lookupEscrowClient = await _srEscrowRepository.FirstOrDefaultAsync((int)escrowAccessHistory.EscrowId);
                output.EscrowClientName = _lookupEscrowClient?.EscrowNo?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditEscrowAccessHistoryDto input)
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

        [AbpAuthorize(AppPermissions.Pages_EscrowAccessHistories_Create)]
        protected virtual async Task Create(CreateOrEditEscrowAccessHistoryDto input)
        {
            var escrowAccessHistory = ObjectMapper.Map<EscrowAccessHistory>(input);

            if (AbpSession.TenantId != null)
            {
                escrowAccessHistory.TenantId = (int?)AbpSession.TenantId ?? 0;
            }

            await _escrowAccessHistoryRepository.InsertAsync(escrowAccessHistory);
        }

        [AbpAuthorize(AppPermissions.Pages_EscrowAccessHistories_Edit)]
        protected virtual async Task Update(CreateOrEditEscrowAccessHistoryDto input)
        {
            var escrowAccessHistory = await _escrowAccessHistoryRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, escrowAccessHistory);
        }

        [AbpAuthorize(AppPermissions.Pages_EscrowAccessHistories_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _escrowAccessHistoryRepository.DeleteAsync(input.Id);
        }

        [AbpAuthorize(AppPermissions.Pages_EscrowAccessHistories)]
        public async Task<PagedResultDto<EscrowAccessHistoryUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_userRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Name != null && e.Name.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var userList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<EscrowAccessHistoryUserLookupTableDto>();
            foreach (var user in userList)
            {
                lookupTableDtoList.Add(new EscrowAccessHistoryUserLookupTableDto
                {
                    Id = user.Id,
                    DisplayName = user.Name?.ToString()
                });
            }

            return new PagedResultDto<EscrowAccessHistoryUserLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_EscrowAccessHistories)]
        public async Task<PagedResultDto<EscrowAccessHistoryEscrowClientLookupTableDto>> GetAllEscrowClientForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _srEscrowRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.EscrowNo != null && e.EscrowNo.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var escrowClientList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<EscrowAccessHistoryEscrowClientLookupTableDto>();
            foreach (var escrowClient in escrowClientList)
            {
                lookupTableDtoList.Add(new EscrowAccessHistoryEscrowClientLookupTableDto
                {
                    Id = escrowClient.Id,
                    DisplayName = escrowClient.EscrowNo?.ToString()
                });
            }

            return new PagedResultDto<EscrowAccessHistoryEscrowClientLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }


        [AbpAuthorize]
        public async Task LogAccess(LogAccessInput input)
        {
            Logger.Info($"LogAccess called for escrowId: {input.EscrowId}, number: {input.EscrowNumber}. User: {AbpSession.UserId}");
            var userId = AbpSession.UserId;
            if (userId == null) return;

            // Resolve escrowId if only number is provided
            if ((input.EscrowId == null || input.EscrowId == 0) && !string.IsNullOrEmpty(input.EscrowNumber))
            {
                // Use SrEscrow repository instead of EscrowClient as per new FK reference
                var escrow = await _srEscrowRepository.FirstOrDefaultAsync(e => e.EscrowNo == input.EscrowNumber);
                if (escrow != null)
                {
                    input.EscrowId = escrow.Id;
                }
            }

            if (input.EscrowId == null)
            {
                Logger.Warn($"LogAccess: Could not resolve escrowId for number: {input.EscrowNumber}");
                return;
            }

            Logger.Info($"LogAccess: Resolved escrowId: {input.EscrowId}");            
            var history = new EscrowAccessHistory
            {
                UserId = userId,
                EscrowId = input.EscrowId,
                CreatedAt = DateTime.UtcNow, // Use UtcNow
                TenantId = AbpSession.TenantId ?? 1 // Default tenant? Or from session.
            };

            await _escrowAccessHistoryRepository.InsertAsync(history);
        }

        [AbpAuthorize]
        public async Task<List<RecentEscrowDto>> GetRecentEscrows()
        {
            try
            {
                var userId = AbpSession.UserId;
                if (userId == null)
                {
                    return new List<RecentEscrowDto>();
                }

                var recentAccesses = await _escrowAccessHistoryRepository.GetAll()
                    .Where(e => e.UserId == userId)
                    .OrderByDescending(e => e.CreatedAt)
                    .Include(e => e.EscrowFk)
                    .ToListAsync();

                var distinctAccesses = recentAccesses
                    .GroupBy(e => e.EscrowId)
                    .Select(g => g.First())
                    .Take(10)
                    .ToList();

                var result = new List<RecentEscrowDto>();

                foreach (var access in distinctAccesses)
                {
                    if (access.EscrowFk == null) continue;

                    var srEscrow = access.EscrowFk;
                    string escrowNumber = srEscrow.EscrowNo;

                    string companyName = "";
                    string subCompanyName = "";
                    string userType = "EOX";

                    var escrowDetail = await _escrowDetailRepository.FirstOrDefaultAsync(ed =>
                        ed.UserId == userId &&
                        ed.EscrowId == escrowNumber);

                    if (escrowDetail != null)
                    {
                        subCompanyName = escrowDetail.Company;
                        userType = escrowDetail.Usertype;                        
                    }

                    // Always try to get Enterprise Name if we have keys (already in srEscrow)
                    // Logic was: get srEscrow from subCo + escrowNo, then get Enterprise
                    // We already have srEscrow. 
                    if(srEscrow != null)
                    {
                        var enterprise = await _enterpriseRepository.FirstOrDefaultAsync(e => e.Id == srEscrow.EnterpriseId);
                        if (enterprise != null)
                        {
                            companyName = enterprise.EnterpriseName;
                        }
                    }

                    result.Add(new RecentEscrowDto
                    {
                        EscrowId = srEscrow.Id,
                        EscrowNumber = escrowNumber,
                        CompanyName = companyName,
                        SubCompanyName = subCompanyName,
                        UserType = userType,
                        LastAccessTime = access.CreatedAt
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("Error in GetRecentEscrows()", ex);
                throw new UserFriendlyException("Something went wrong while fetching recent escrows.");
            }
        }


    }
}