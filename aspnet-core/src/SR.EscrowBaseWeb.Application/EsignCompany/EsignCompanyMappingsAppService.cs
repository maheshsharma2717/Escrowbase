using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using SR.EscrowBaseWeb.EsignCompany.Dtos;
using SR.EscrowBaseWeb.Dto;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using SR.EscrowBaseWeb.Storage;

namespace SR.EscrowBaseWeb.EsignCompany
{
    [AbpAuthorize(AppPermissions.Pages_EsignCompanyMappings)]
    public class EsignCompanyMappingsAppService : EscrowBaseWebAppServiceBase, IEsignCompanyMappingsAppService
    {
        private readonly IRepository<EsignCompanyMapping> _esignCompanyMappingRepository;

        public EsignCompanyMappingsAppService(IRepository<EsignCompanyMapping> esignCompanyMappingRepository)
        {
            _esignCompanyMappingRepository = esignCompanyMappingRepository;

        }

        public async Task<PagedResultDto<GetEsignCompanyMappingForViewDto>> GetAll(GetAllEsignCompanyMappingsInput input)
        {

            var filteredEsignCompanyMappings = _esignCompanyMappingRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.ComanyName.Contains(input.Filter) || e.IsActive.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ComanyNameFilter), e => e.ComanyName == input.ComanyNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.IsActiveFilter), e => e.IsActive == input.IsActiveFilter);

            var pagedAndFilteredEsignCompanyMappings = filteredEsignCompanyMappings
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var esignCompanyMappings = from o in pagedAndFilteredEsignCompanyMappings
                                       select new
                                       {

                                           o.ComanyName,
                                           o.IsActive,
                                           Id = o.Id
                                       };

            var totalCount = await filteredEsignCompanyMappings.CountAsync();

            var dbList = await esignCompanyMappings.ToListAsync();
            var results = new List<GetEsignCompanyMappingForViewDto>();

            foreach (var o in dbList)
            {
                var res = new GetEsignCompanyMappingForViewDto()
                {
                    EsignCompanyMapping = new EsignCompanyMappingDto
                    {

                        ComanyName = o.ComanyName,
                        IsActive = o.IsActive,
                        Id = o.Id,
                    }
                };

                results.Add(res);
            }

            return new PagedResultDto<GetEsignCompanyMappingForViewDto>(
                totalCount,
                results
            );

        }

        public async Task<GetEsignCompanyMappingForViewDto> GetEsignCompanyMappingForView(int id)
        {
            var esignCompanyMapping = await _esignCompanyMappingRepository.GetAsync(id);

            var output = new GetEsignCompanyMappingForViewDto { EsignCompanyMapping = ObjectMapper.Map<EsignCompanyMappingDto>(esignCompanyMapping) };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_EsignCompanyMappings_Edit)]
        public async Task<GetEsignCompanyMappingForEditOutput> GetEsignCompanyMappingForEdit(EntityDto input)
        {
            var esignCompanyMapping = await _esignCompanyMappingRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetEsignCompanyMappingForEditOutput { EsignCompanyMapping = ObjectMapper.Map<CreateOrEditEsignCompanyMappingDto>(esignCompanyMapping) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditEsignCompanyMappingDto input)
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

        [AbpAuthorize(AppPermissions.Pages_EsignCompanyMappings_Create)]
        protected virtual async Task Create(CreateOrEditEsignCompanyMappingDto input)
        {
            var esignCompanyMapping = ObjectMapper.Map<EsignCompanyMapping>(input);

            await _esignCompanyMappingRepository.InsertAsync(esignCompanyMapping);

        }

        [AbpAuthorize(AppPermissions.Pages_EsignCompanyMappings_Edit)]
        protected virtual async Task Update(CreateOrEditEsignCompanyMappingDto input)
        {
            var esignCompanyMapping = await _esignCompanyMappingRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, esignCompanyMapping);

        }

        [AbpAuthorize(AppPermissions.Pages_EsignCompanyMappings_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _esignCompanyMappingRepository.DeleteAsync(input.Id);
        }

    }
}