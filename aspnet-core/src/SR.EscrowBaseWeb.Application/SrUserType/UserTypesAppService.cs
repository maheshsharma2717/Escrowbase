

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using SR.EscrowBaseWeb.SrUserType.Dtos;
using SR.EscrowBaseWeb.Dto;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace SR.EscrowBaseWeb.SrUserType
{
	 
    public class UserTypesAppService : EscrowBaseWebAppServiceBase, IUserTypesAppService
    {
		 private readonly IRepository<UserType> _userTypeRepository;
		 

		  public UserTypesAppService(IRepository<UserType> userTypeRepository ) 
		  {
			_userTypeRepository = userTypeRepository;
			
		  }

		 public async Task<PagedResultDto<GetUserTypeForViewDto>> GetAll(GetAllUserTypesInput input)
         {
			
			var filteredUserTypes = _userTypeRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Type.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.TypeFilter),  e => e.Type == input.TypeFilter);

			var pagedAndFilteredUserTypes = filteredUserTypes
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var userTypes = from o in pagedAndFilteredUserTypes
                         select new GetUserTypeForViewDto() {
							UserType = new UserTypeDto
							{
                                Type = o.Type,
                                Id = o.Id
							}
						};

            var totalCount = await filteredUserTypes.CountAsync();

            return new PagedResultDto<GetUserTypeForViewDto>(
                totalCount,
                await userTypes.ToListAsync()
            );
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_UserTypes_Edit)]
		 public async Task<GetUserTypeForEditOutput> GetUserTypeForEdit(EntityDto input)
         {
            var userType = await _userTypeRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetUserTypeForEditOutput {UserType = ObjectMapper.Map<CreateOrEditUserTypeDto>(userType)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditUserTypeDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_UserTypes_Create)]
		 protected virtual async Task Create(CreateOrEditUserTypeDto input)
         {
            var userType = ObjectMapper.Map<UserType>(input);

			

            await _userTypeRepository.InsertAsync(userType);
         }

		 [AbpAuthorize(AppPermissions.Pages_UserTypes_Edit)]
		 protected virtual async Task Update(CreateOrEditUserTypeDto input)
         {
            var userType = await _userTypeRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, userType);
         }

		 [AbpAuthorize(AppPermissions.Pages_UserTypes_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _userTypeRepository.DeleteAsync(input.Id);
         } 
    }
}