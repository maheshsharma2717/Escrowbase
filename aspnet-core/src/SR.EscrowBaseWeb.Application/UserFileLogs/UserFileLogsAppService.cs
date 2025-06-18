using SR.EscrowBaseWeb.Authorization.Users;


using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using SR.EscrowBaseWeb.UserFileLogs.Dtos;
using SR.EscrowBaseWeb.Dto;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace SR.EscrowBaseWeb.UserFileLogs
{
	//[AbpAuthorize(AppPermissions.Pages_UserFileLogs)]
    public class UserFileLogsAppService : EscrowBaseWebAppServiceBase, IUserFileLogsAppService
    {
		 private readonly IRepository<UserFileLog, long> _userFileLogRepository;
		 private readonly IRepository<User,long> _lookup_userRepository;
		 

		  public UserFileLogsAppService(IRepository<UserFileLog, long> userFileLogRepository , IRepository<User, long> lookup_userRepository) 
		  {
			_userFileLogRepository = userFileLogRepository;
			_lookup_userRepository = lookup_userRepository;
		
		  }

		 public async Task<PagedResultDto<GetUserFileLogForViewDto>> GetAll(GetAllUserFileLogsInput input)
         {
			
			var filteredUserFileLogs = _userFileLogRepository.GetAll()

						.Include( e => e.UserFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Name.Contains(input.Filter) || e.FileName.Contains(input.Filter) || e.Status.Contains(input.Filter) || e.Usertype.Contains(input.Filter) || e.Accesslevel.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter),  e => e.Name == input.NameFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.FileNameFilter),  e => e.FileName == input.FileNameFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.StatusFilter),  e => e.Status == input.StatusFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.UsertypeFilter),  e => e.Usertype == input.UsertypeFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.AccesslevelFilter),  e => e.Accesslevel == input.AccesslevelFilter)
						.WhereIf(input.MinUpdateOnFilter != null, e => e.UpdateOn >= input.MinUpdateOnFilter)
						.WhereIf(input.MaxUpdateOnFilter != null, e => e.UpdateOn <= input.MaxUpdateOnFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserFk != null && e.UserFk.Name == input.UserNameFilter);

			var pagedAndFilteredUserFileLogs = filteredUserFileLogs
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var userFileLogs = from o in pagedAndFilteredUserFileLogs
                         join o1 in _lookup_userRepository.GetAll() on o.UserId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         select new GetUserFileLogForViewDto() {
							UserFileLog = new UserFileLogDto
							{
                                Name = o.Name,
                                FileName = o.FileName,
                                Status = o.Status,
                                Usertype = o.Usertype,
                                Accesslevel = o.Accesslevel,
                                UpdateOn = o.UpdateOn,
                                Id = o.Id
							},
                         	UserName = s1 == null || s1.Name == null ? "" : s1.Name.ToString()
						};

            var totalCount = await filteredUserFileLogs.CountAsync();

            return new PagedResultDto<GetUserFileLogForViewDto>(
                totalCount,
                await userFileLogs.ToListAsync()
            );
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_UserFileLogs_Edit)]
		 public async Task<GetUserFileLogForEditOutput> GetUserFileLogForEdit(EntityDto<long> input)
         {
            var userFileLog = await _userFileLogRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetUserFileLogForEditOutput {UserFileLog = ObjectMapper.Map<CreateOrEditUserFileLogDto>(userFileLog)};

		    if (output.UserFileLog.UserId != null)
            {
                var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.UserFileLog.UserId);
                output.UserName = _lookupUser?.Name?.ToString();
            }
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditUserFileLogDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 //[AbpAuthorize(AppPermissions.Pages_UserFileLogs_Create)]
		 protected virtual async Task Create(CreateOrEditUserFileLogDto input)
         {
            var userFileLog = ObjectMapper.Map<UserFileLog>(input);

			

            await _userFileLogRepository.InsertAsync(userFileLog);
         }

		 [AbpAuthorize(AppPermissions.Pages_UserFileLogs_Edit)]
		 protected virtual async Task Update(CreateOrEditUserFileLogDto input)
         {
            var userFileLog = await _userFileLogRepository.FirstOrDefaultAsync((long)input.Id);
             ObjectMapper.Map(input, userFileLog);
         }

		 [AbpAuthorize(AppPermissions.Pages_UserFileLogs_Delete)]
         public async Task Delete(EntityDto<long> input)
         {
            await _userFileLogRepository.DeleteAsync(input.Id);
         } 

		[AbpAuthorize(AppPermissions.Pages_UserFileLogs)]
         public async Task<PagedResultDto<UserFileLogUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_userRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Name != null && e.Name.Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var userList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<UserFileLogUserLookupTableDto>();
			foreach(var user in userList){
				lookupTableDtoList.Add(new UserFileLogUserLookupTableDto
				{
					Id = user.Id,
					DisplayName = user.Name?.ToString()
				});
			}

            return new PagedResultDto<UserFileLogUserLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
    }
}