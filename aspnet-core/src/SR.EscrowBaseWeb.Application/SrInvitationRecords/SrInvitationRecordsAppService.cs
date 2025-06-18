using SR.EscrowBaseWeb.Authorization.Users;


using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using SR.EscrowBaseWeb.SrInvitationRecords.Dtos;
using SR.EscrowBaseWeb.Dto;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace SR.EscrowBaseWeb.SrInvitationRecords
{
	//[AbpAuthorize(AppPermissions.Pages_SrInvitationRecords)]
    public class SrInvitationRecordsAppService : EscrowBaseWebAppServiceBase, ISrInvitationRecordsAppService
    {
		 private readonly IRepository<SrInvitationRecord, long> _srInvitationRecordRepository;
		 private readonly IRepository<User,long> _lookup_userRepository;
		 

		  public SrInvitationRecordsAppService(IRepository<SrInvitationRecord, long> srInvitationRecordRepository , IRepository<User, long> lookup_userRepository) 
		  {
			_srInvitationRecordRepository = srInvitationRecordRepository;
			_lookup_userRepository = lookup_userRepository;
		
		  }

		 public async Task<PagedResultDto<GetSrInvitationRecordForViewDto>> GetAll(GetAllSrInvitationRecordsInput input)
         {
			
			var filteredSrInvitationRecords = _srInvitationRecordRepository.GetAll()
						.Include( e => e.UserFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Email.Contains(input.Filter) || e.DomainAccessInstance.Contains(input.Filter) || e.EscrowCompany.Contains(input.Filter) || e.EscrowOfficer.Contains(input.Filter) || e.EscrowContactEmail.Contains(input.Filter) || e.Usertype.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.EmailFilter),  e => e.Email == input.EmailFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.DomainAccessInstanceFilter),  e => e.DomainAccessInstance == input.DomainAccessInstanceFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.EscrowCompanyFilter),  e => e.EscrowCompany == input.EscrowCompanyFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.EscrowOfficerFilter),  e => e.EscrowOfficer == input.EscrowOfficerFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.EscrowContactEmailFilter),  e => e.EscrowContactEmail == input.EscrowContactEmailFilter)
						//.WhereIf(input.MinEscrowNumberFilter != null, e => e.EscrowNumber >= input.MinEscrowNumberFilter)
						//.WhereIf(input.MaxEscrowNumberFilter != null, e => e.EscrowNumber <= input.MaxEscrowNumberFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.UsertypeFilter),  e => e.Usertype == input.UsertypeFilter)
						//.WhereIf(input.MinEscrowOfficerPhoneNumberFilter != null, e => e.EscrowOfficerPhoneNumber >= input.MinEscrowOfficerPhoneNumberFilter)
						//.WhereIf(input.MaxEscrowOfficerPhoneNumberFilter != null, e => e.EscrowOfficerPhoneNumber <= input.MaxEscrowOfficerPhoneNumberFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserFk != null && e.UserFk.Name == input.UserNameFilter);

			var pagedAndFilteredSrInvitationRecords = filteredSrInvitationRecords
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var srInvitationRecords = from o in pagedAndFilteredSrInvitationRecords
                         join o1 in _lookup_userRepository.GetAll() on o.UserId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         select new GetSrInvitationRecordForViewDto() {
							SrInvitationRecord = new SrInvitationRecordDto
							{
                                Email = o.Email,
                                DomainAccessInstance = o.DomainAccessInstance,
                                EscrowCompany = o.EscrowCompany,
                                EscrowOfficer = o.EscrowOfficer,
                                EscrowContactEmail = o.EscrowContactEmail,
                                EscrowNumber = o.EscrowNumber,
                                Usertype = o.Usertype,
                                UserId = o.UserId,
                                EscrowOfficerPhoneNumber = o.EscrowOfficerPhoneNumber,
                                Id = o.Id
							},
                         	UserName = s1 == null || s1.Name == null ? "" : s1.Name.ToString()
						};

            var totalCount = await filteredSrInvitationRecords.CountAsync();

            return new PagedResultDto<GetSrInvitationRecordForViewDto>(
                totalCount,
                await srInvitationRecords.ToListAsync()
            );
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_SrInvitationRecords_Edit)]
		 public async Task<GetSrInvitationRecordForEditOutput> GetSrInvitationRecordForEdit(EntityDto<long> input)
         {
            var srInvitationRecord = await _srInvitationRecordRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetSrInvitationRecordForEditOutput {SrInvitationRecord = ObjectMapper.Map<CreateOrEditSrInvitationRecordDto>(srInvitationRecord)};

		    if (output.SrInvitationRecord.UserId != null)
            {
                var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.SrInvitationRecord.UserId);
                output.UserName = _lookupUser?.Name?.ToString();
            }
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditSrInvitationRecordDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 //[AbpAuthorize(AppPermissions.Pages_SrInvitationRecords_Create)]
		 protected virtual async Task Create(CreateOrEditSrInvitationRecordDto input)
         {
            var srInvitationRecord = ObjectMapper.Map<SrInvitationRecord>(input);

			

            await _srInvitationRecordRepository.InsertAsync(srInvitationRecord);
         }

		 [AbpAuthorize(AppPermissions.Pages_SrInvitationRecords_Edit)]
		 protected virtual async Task Update(CreateOrEditSrInvitationRecordDto input)
         {
            var srInvitationRecord = await _srInvitationRecordRepository.FirstOrDefaultAsync((long)input.Id);
             ObjectMapper.Map(input, srInvitationRecord);
         }

		 [AbpAuthorize(AppPermissions.Pages_SrInvitationRecords_Delete)]
         public async Task Delete(EntityDto<long> input)
         {
            await _srInvitationRecordRepository.DeleteAsync(input.Id);
         } 

		[AbpAuthorize(AppPermissions.Pages_SrInvitationRecords)]
         public async Task<PagedResultDto<SrInvitationRecordUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_userRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Name != null && e.Name.Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var userList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<SrInvitationRecordUserLookupTableDto>();
			foreach(var user in userList){
				lookupTableDtoList.Add(new SrInvitationRecordUserLookupTableDto
				{
					Id = user.Id,
					DisplayName = user.Name?.ToString()
				});
			}

            return new PagedResultDto<SrInvitationRecordUserLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
    }
}