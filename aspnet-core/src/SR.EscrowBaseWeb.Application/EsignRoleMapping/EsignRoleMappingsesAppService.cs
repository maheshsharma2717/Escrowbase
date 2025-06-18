using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using SR.EscrowBaseWeb.EsignRoleMapping.Dtos;
using SR.EscrowBaseWeb.Dto;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using SR.EscrowBaseWeb.Storage;

namespace SR.EscrowBaseWeb.EsignRoleMapping
{
    [AbpAuthorize(AppPermissions.Pages_EsignRoleMappingses)]
    public class EsignRoleMappingsesAppService : EscrowBaseWebAppServiceBase, IEsignRoleMappingsesAppService
    {
        private readonly IRepository<EsignRoleMappings> _esignRoleMappingsRepository;

        public EsignRoleMappingsesAppService(IRepository<EsignRoleMappings> esignRoleMappingsRepository)
        {
            _esignRoleMappingsRepository = esignRoleMappingsRepository;

        }

        public async Task<PagedResultDto<GetEsignRoleMappingsForViewDto>> GetAll(GetAllEsignRoleMappingsesInput input)
        {

            var filteredEsignRoleMappingses = _esignRoleMappingsRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.EsignRole.Contains(input.Filter) || e.UserRole.Contains(input.Filter));

            var pagedAndFilteredEsignRoleMappingses = filteredEsignRoleMappingses
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var esignRoleMappingses = from o in pagedAndFilteredEsignRoleMappingses
                                      select new
                                      {

                                          o.Id,
                                          o.EsignRole,
                                          o.UserRole,
                                          o.EsignCompanyCode,
                                          
                                      };

            var totalCount = await filteredEsignRoleMappingses.CountAsync();

            var dbList = await esignRoleMappingses.ToListAsync();
            var results = new List<GetEsignRoleMappingsForViewDto>();

            foreach (var o in dbList)
            {
                var res = new GetEsignRoleMappingsForViewDto()
                {
                    EsignRoleMappings = new EsignRoleMappingsDto
                    {

                        Id = o.Id,
                        EsignRole = o.EsignRole,
                        UserRole = o.UserRole,
                        EsignCompanyCode = o.EsignCompanyCode,
                       
                    }
                };

                results.Add(res);
            }

            return new PagedResultDto<GetEsignRoleMappingsForViewDto>(
                totalCount,
                results
            );

        }

        [AbpAuthorize(AppPermissions.Pages_EsignRoleMappingses_Edit)]
        public async Task<GetEsignRoleMappingsForEditOutput> GetEsignRoleMappingsForEdit(EntityDto input)
        {
            var esignRoleMappings = await _esignRoleMappingsRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetEsignRoleMappingsForEditOutput { EsignRoleMappings = ObjectMapper.Map<CreateOrEditEsignRoleMappingsDto>(esignRoleMappings) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditEsignRoleMappingsDto input)
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

        [AbpAuthorize(AppPermissions.Pages_EsignRoleMappingses_Create)]
        protected virtual async Task Create(CreateOrEditEsignRoleMappingsDto input)
        {
            var esignRoleMappings = ObjectMapper.Map<EsignRoleMappings>(input);

            if (AbpSession.TenantId != null)
            {
                esignRoleMappings.TenantId = (int)AbpSession.TenantId;
            }

            await _esignRoleMappingsRepository.InsertAsync(esignRoleMappings);

        }

        [AbpAuthorize(AppPermissions.Pages_EsignRoleMappingses_Edit)]
        protected virtual async Task Update(CreateOrEditEsignRoleMappingsDto input)
        {
            var esignRoleMappings = await _esignRoleMappingsRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, esignRoleMappings);

        }

        [AbpAuthorize(AppPermissions.Pages_EsignRoleMappingses_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _esignRoleMappingsRepository.DeleteAsync(input.Id);
        }

    }
}