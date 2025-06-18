using SR.EscrowBaseWeb.Authorization.Users;
using SR.EscrowBaseWeb.SREscrowClient;


using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using SR.EscrowBaseWeb.SrEscrowUserMapping.Dtos;
using SR.EscrowBaseWeb.Dto;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace SR.EscrowBaseWeb.SrEscrowUserMapping
{
	[AbpAuthorize(AppPermissions.Pages_EscrowUserMappings)]
    public class EscrowUserMappingsAppService : EscrowBaseWebAppServiceBase, IEscrowUserMappingsAppService
    {
		 private readonly IRepository<EscrowUserMapping> _escrowUserMappingRepository;
		 private readonly IRepository<User,long> _lookup_userRepository;
		 private readonly IRepository<EscrowClient,int> _lookup_escrowClientRepository;
		 

		  public EscrowUserMappingsAppService(IRepository<EscrowUserMapping> escrowUserMappingRepository , IRepository<User, long> lookup_userRepository, IRepository<EscrowClient, int> lookup_escrowClientRepository) 
		  {
			_escrowUserMappingRepository = escrowUserMappingRepository;
			_lookup_userRepository = lookup_userRepository;
		_lookup_escrowClientRepository = lookup_escrowClientRepository;
		
		  }

		 public async Task<PagedResultDto<GetEscrowUserMappingForViewDto>> GetAll(GetAllEscrowUserMappingsInput input)
         {
			
			var filteredEscrowUserMappings = _escrowUserMappingRepository.GetAll()
						.Include( e => e.UserFk)
						.Include( e => e.EscrowClientFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false )
						.WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserFk != null && e.UserFk.Name == input.UserNameFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.EscrowClientNameFilter), e => e.EscrowClientFk != null && e.EscrowClientFk.Name == input.EscrowClientNameFilter);

			var pagedAndFilteredEscrowUserMappings = filteredEscrowUserMappings
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var escrowUserMappings = from o in pagedAndFilteredEscrowUserMappings
                         join o1 in _lookup_userRepository.GetAll() on o.UserId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         join o2 in _lookup_escrowClientRepository.GetAll() on o.EscrowClientId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()
                         
                         select new GetEscrowUserMappingForViewDto() {
							EscrowUserMapping = new EscrowUserMappingDto
							{
                                Id = o.Id
							},
                         	UserName = s1 == null || s1.Name == null ? "" : s1.Name.ToString(),
                         	EscrowClientName = s2 == null || s2.Name == null ? "" : s2.Name.ToString()
						};

            var totalCount = await filteredEscrowUserMappings.CountAsync();

            return new PagedResultDto<GetEscrowUserMappingForViewDto>(
                totalCount,
                await escrowUserMappings.ToListAsync()
            );
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_EscrowUserMappings_Edit)]
		 public async Task<GetEscrowUserMappingForEditOutput> GetEscrowUserMappingForEdit(EntityDto input)
         {
            var escrowUserMapping = await _escrowUserMappingRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetEscrowUserMappingForEditOutput {EscrowUserMapping = ObjectMapper.Map<CreateOrEditEscrowUserMappingDto>(escrowUserMapping)};

		    if (output.EscrowUserMapping.UserId != null)
            {
                var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.EscrowUserMapping.UserId);
                output.UserName = _lookupUser?.Name?.ToString();
            }

		    if (output.EscrowUserMapping.EscrowClientId != null)
            {
                var _lookupEscrowClient = await _lookup_escrowClientRepository.FirstOrDefaultAsync((int)output.EscrowUserMapping.EscrowClientId);
                output.EscrowClientName = _lookupEscrowClient?.Name?.ToString();
            }
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditEscrowUserMappingDto input)
         {
            try
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
            catch (Exception ex)
            {

            }
         }

		 //[AbpAuthorize(AppPermissions.Pages_EscrowUserMappings_Create)]
		 protected virtual async Task Create(CreateOrEditEscrowUserMappingDto input)
         {
            var escrowUserMapping = ObjectMapper.Map<EscrowUserMapping>(input);


            //await _escrowUserMappingRepository.GetAsync(escrowUserMapping);
            await _escrowUserMappingRepository.InsertAsync(escrowUserMapping);
         }

		 [AbpAuthorize(AppPermissions.Pages_EscrowUserMappings_Edit)]
		 protected virtual async Task Update(CreateOrEditEscrowUserMappingDto input)
         {
            var escrowUserMapping = await _escrowUserMappingRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, escrowUserMapping);
         }

		 [AbpAuthorize(AppPermissions.Pages_EscrowUserMappings_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _escrowUserMappingRepository.DeleteAsync(input.Id);
         } 

		[AbpAuthorize(AppPermissions.Pages_EscrowUserMappings)]
         public async Task<PagedResultDto<EscrowUserMappingUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_userRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Name != null && e.Name.Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var userList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<EscrowUserMappingUserLookupTableDto>();
			foreach(var user in userList){
				lookupTableDtoList.Add(new EscrowUserMappingUserLookupTableDto
				{
					Id = user.Id,
					DisplayName = user.Name?.ToString()
				});
			}

            return new PagedResultDto<EscrowUserMappingUserLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_EscrowUserMappings)]
         public async Task<PagedResultDto<EscrowUserMappingEscrowClientLookupTableDto>> GetAllEscrowClientForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_escrowClientRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Name != null && e.Name.Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var escrowClientList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<EscrowUserMappingEscrowClientLookupTableDto>();
			foreach(var escrowClient in escrowClientList){
				lookupTableDtoList.Add(new EscrowUserMappingEscrowClientLookupTableDto
				{
					Id = escrowClient.Id,
					DisplayName = escrowClient.Name?.ToString()
				});
			}

            return new PagedResultDto<EscrowUserMappingEscrowClientLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
    }
}