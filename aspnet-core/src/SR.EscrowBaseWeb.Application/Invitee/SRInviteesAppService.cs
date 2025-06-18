using SR.EscrowBaseWeb.SrUserType;
using SR.EscrowBaseWeb.SREscrowClient;


using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using SR.EscrowBaseWeb.Invitee.Dtos;
using SR.EscrowBaseWeb.Dto;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace SR.EscrowBaseWeb.Invitee
{
	//[AbpAuthorize(AppPermissions.Pages_SRInvitees)]
    public class SRInviteesAppService : EscrowBaseWebAppServiceBase, ISRInviteesAppService
    {
		 private readonly IRepository<SRInvitee> _srInviteeRepository;
		 private readonly IRepository<UserType,int> _lookup_userTypeRepository;
		 private readonly IRepository<EscrowClient,int> _lookup_escrowClientRepository;
		 

		  public SRInviteesAppService(IRepository<SRInvitee> srInviteeRepository , IRepository<UserType, int> lookup_userTypeRepository, IRepository<EscrowClient, int> lookup_escrowClientRepository) 
		  {
			_srInviteeRepository = srInviteeRepository;
			_lookup_userTypeRepository = lookup_userTypeRepository;
		_lookup_escrowClientRepository = lookup_escrowClientRepository;
		
		  }

		 public async Task<PagedResultDto<GetSRInviteeForViewDto>> GetAll(GetAllSRInviteesInput input)
         {
			
			var filteredSRInvitees = _srInviteeRepository.GetAll()
						.Include( e => e.UserTypeFk)
						.Include( e => e.EscrowClientFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Email.Contains(input.Filter) || e.Name.Contains(input.Filter) || e.Phone.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.UserTypeTypeFilter), e => e.UserTypeFk != null && e.UserTypeFk.Type == input.UserTypeTypeFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.EscrowClientNameFilter), e => e.EscrowClientFk != null && e.EscrowClientFk.Name == input.EscrowClientNameFilter);

			var pagedAndFilteredSRInvitees = filteredSRInvitees
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var srInvitees = from o in pagedAndFilteredSRInvitees
                         join o1 in _lookup_userTypeRepository.GetAll() on o.UserTypeId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         join o2 in _lookup_escrowClientRepository.GetAll() on o.EscrowClientId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()
                         
                         select new GetSRInviteeForViewDto() {
							SRInvitee = new SRInviteeDto
							{
                                Email = o.Email,
                                Name = o.Name,
                                Phone = o.Phone,
                                IsSignedUp = o.IsSignedUp,
                                Id = o.Id
							},
                         	UserTypeType = s1 == null || s1.Type == null ? "" : s1.Type.ToString(),
                         	EscrowClientName = s2 == null || s2.Name == null ? "" : s2.Name.ToString()
						};

            var totalCount = await filteredSRInvitees.CountAsync();

            return new PagedResultDto<GetSRInviteeForViewDto>(
                totalCount,
                await srInvitees.ToListAsync()
            );
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_SRInvitees_Edit)]
		 public async Task<GetSRInviteeForEditOutput> GetSRInviteeForEdit(EntityDto input)
         {
            var srInvitee = await _srInviteeRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetSRInviteeForEditOutput {SRInvitee = ObjectMapper.Map<CreateOrEditSRInviteeDto>(srInvitee)};

		    if (output.SRInvitee.UserTypeId != null)
            {
                var _lookupUserType = await _lookup_userTypeRepository.FirstOrDefaultAsync((int)output.SRInvitee.UserTypeId);
                output.UserTypeType = _lookupUserType?.Type?.ToString();
            }

		    if (output.SRInvitee.EscrowClientId != null)
            {
                var _lookupEscrowClient = await _lookup_escrowClientRepository.FirstOrDefaultAsync((int)output.SRInvitee.EscrowClientId);
                output.EscrowClientName = _lookupEscrowClient?.Name?.ToString();
            }
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditSRInviteeDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 //[AbpAuthorize(AppPermissions.Pages_SRInvitees_Create)]
		 protected virtual async Task Create(CreateOrEditSRInviteeDto input)
         {
            var srInvitee = ObjectMapper.Map<SRInvitee>(input);

			

            await _srInviteeRepository.InsertAsync(srInvitee);
         }

		 [AbpAuthorize(AppPermissions.Pages_SRInvitees_Edit)]
		 protected virtual async Task Update(CreateOrEditSRInviteeDto input)
         {
            var srInvitee = await _srInviteeRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, srInvitee);
         }

		 [AbpAuthorize(AppPermissions.Pages_SRInvitees_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _srInviteeRepository.DeleteAsync(input.Id);
         } 

		[AbpAuthorize(AppPermissions.Pages_SRInvitees)]
         public async Task<PagedResultDto<SRInviteeUserTypeLookupTableDto>> GetAllUserTypeForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_userTypeRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Type != null && e.Type.Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var userTypeList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<SRInviteeUserTypeLookupTableDto>();
			foreach(var userType in userTypeList){
				lookupTableDtoList.Add(new SRInviteeUserTypeLookupTableDto
				{
					Id = userType.Id,
					DisplayName = userType.Type?.ToString()
				});
			}

            return new PagedResultDto<SRInviteeUserTypeLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_SRInvitees)]
         public async Task<PagedResultDto<SRInviteeEscrowClientLookupTableDto>> GetAllEscrowClientForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_escrowClientRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Name != null && e.Name.Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var escrowClientList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<SRInviteeEscrowClientLookupTableDto>();
			foreach(var escrowClient in escrowClientList){
				lookupTableDtoList.Add(new SRInviteeEscrowClientLookupTableDto
				{
					Id = escrowClient.Id,
					DisplayName = escrowClient.Name?.ToString()
				});
			}

            return new PagedResultDto<SRInviteeEscrowClientLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
    }
}