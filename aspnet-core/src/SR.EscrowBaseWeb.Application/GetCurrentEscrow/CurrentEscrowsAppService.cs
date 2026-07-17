using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using SR.EscrowBaseWeb.GetCurrentEscrow.Dtos;
using SR.EscrowBaseWeb.Dto;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using SR.EscrowBaseWeb.Storage;

namespace SR.EscrowBaseWeb.GetCurrentEscrow
{
    //[AbpAuthorize(AppPermissions.Pages_CurrentEscrows)]
    public class CurrentEscrowsAppService : EscrowBaseWebAppServiceBase, ICurrentEscrowsAppService
    {
        private readonly IRepository<CurrentEscrow> _currentEscrowRepository;

        public CurrentEscrowsAppService(IRepository<CurrentEscrow> currentEscrowRepository)
        {
            _currentEscrowRepository = currentEscrowRepository;

        }

        public async Task<PagedResultDto<GetCurrentEscrowForViewDto>> GetAll(GetAllCurrentEscrowsInput input)
        {
            string targetUser = input.Filter;
            string targetUserName = null;

            if (string.IsNullOrWhiteSpace(targetUser) && AbpSession.UserId.HasValue)
            {
                var currentUser = await GetCurrentUserAsync();
                targetUser = currentUser?.EmailAddress;
                targetUserName = currentUser?.UserName;
            }

            var filteredCurrentEscrows = _currentEscrowRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(targetUser), e => e.UserName == targetUser || (!string.IsNullOrEmpty(targetUserName) && e.UserName == targetUserName))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.EscrowNoFilter), e => e.EscrowNo == input.EscrowNoFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CompanyNameFilter), e => e.CompanyName == input.CompanyNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.SubCompanyNameFilter), e => e.SubCompanyName == input.SubCompanyNameFilter)
                        .WhereIf(input.MinCreatedOnFilter != null, e => e.CreatedOn >= input.MinCreatedOnFilter)
                        .WhereIf(input.MaxCreatedOnFilter != null, e => e.CreatedOn <= input.MaxCreatedOnFilter);

            var pagedAndFilteredCurrentEscrows = filteredCurrentEscrows
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var currentEscrows = from o in pagedAndFilteredCurrentEscrows
                                 select new
                                 {

                                     Id = o.Id,
                                     escrowNo = o.EscrowNo,
                                     companyName = o.CompanyName,
                                     subCompanyName = o.SubCompanyName,
                                     createdOn = o.CreatedOn,
                                     fileName = o.FileName,
                                     userName = o.UserName,
                                     isActive = o.IsActive,
                                 };

            var totalCount = await filteredCurrentEscrows.CountAsync();

            var dbList = await currentEscrows.ToListAsync();
            var results = new List<GetCurrentEscrowForViewDto>();

            foreach (var o in dbList)
            {
                var res = new GetCurrentEscrowForViewDto()
                {
                    CurrentEscrow = new CurrentEscrowDto
                    {

                        Id = o.Id,
                        EscrowNo = o.escrowNo,
                        CompanyName = o.companyName,
                        SubCompanyName = o.subCompanyName,
                        CreatedOn = o.createdOn,
                        FileName = o.fileName,
                        UserName = o.userName,
                        IsActive = o.isActive
                    }
                };

                results.Add(res);
            }

            return new PagedResultDto<GetCurrentEscrowForViewDto>(
                totalCount,
                results
            );

        }

        public async Task<GetCurrentEscrowForViewDto> GetCurrentEscrowForView(int id)
        {
            var currentEscrow = await _currentEscrowRepository.GetAsync(id);

            var output = new GetCurrentEscrowForViewDto { CurrentEscrow = ObjectMapper.Map<CurrentEscrowDto>(currentEscrow) };

            return output;
        }

        //[AbpAuthorize(AppPermissions.Pages_CurrentEscrows_Edit)]
        public async Task<GetCurrentEscrowForEditOutput> GetCurrentEscrowForEdit(EntityDto input)
        {
            var currentEscrow = await _currentEscrowRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetCurrentEscrowForEditOutput { CurrentEscrow = ObjectMapper.Map<CreateOrEditCurrentEscrowDto>(currentEscrow) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditCurrentEscrowDto input)
        {
            if (input.Id == 0 || input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        //[AbpAuthorize(AppPermissions.Pages_CurrentEscrows_Create)]
        protected virtual async Task Create(CreateOrEditCurrentEscrowDto input)
        {
            var currentEscrow = ObjectMapper.Map<CurrentEscrow>(input);


            await _currentEscrowRepository.InsertAsync(currentEscrow);

        }

        //[AbpAuthorize(AppPermissions.Pages_CurrentEscrows_Edit)]
        protected virtual async Task Update(CreateOrEditCurrentEscrowDto input)

        {
            var currentEscrow = await _currentEscrowRepository.FirstOrDefaultAsync((int)input.Id);

            ObjectMapper.Map(input, currentEscrow);

            // Only update → UoW will commit automatically
            await _currentEscrowRepository.UpdateAsync(currentEscrow);
        }

        //[AbpAuthorize(AppPermissions.Pages_CurrentEscrows_Delete)]
        public async Task Delete(EntityDto input)
        {
            try
            {
                var currentEscrow = await _currentEscrowRepository.FirstOrDefaultAsync(input.Id);
                if (currentEscrow == null)
                {
                    throw new UserFriendlyException("Record not found.");
                }

                await _currentEscrowRepository.DeleteAsync(currentEscrow);
                await CurrentUnitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.Error("Delete failed: " + ex.Message, ex);
                throw;
            }
        }

    }
}