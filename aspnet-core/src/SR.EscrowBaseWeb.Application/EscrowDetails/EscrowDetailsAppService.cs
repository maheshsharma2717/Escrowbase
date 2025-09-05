using SR.EscrowBaseWeb.Authorization.Users;


using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using SR.EscrowBaseWeb.EscrowDetails.Dtos;
using SR.EscrowBaseWeb.Dto;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace SR.EscrowBaseWeb.EscrowDetails
{
	//[AbpAuthorize(AppPermissions.Pages_EscrowDetails)]
    public class EscrowDetailsAppService : EscrowBaseWebAppServiceBase, IEscrowDetailsAppService
    {
		 private readonly IRepository<EscrowDetail, long> _escrowDetailRepository;
		 private readonly IRepository<User,long> _lookup_userRepository;
		 

		  public EscrowDetailsAppService(IRepository<EscrowDetail, long> escrowDetailRepository , IRepository<User, long> lookup_userRepository) 
		  {
			_escrowDetailRepository = escrowDetailRepository;
			_lookup_userRepository = lookup_userRepository;
		
		  }

		 public async Task<PagedResultDto<GetEscrowDetailForViewDto>> GetAll(GetAllEscrowDetailsInput input)
         {
			
			var filteredEscrowDetails = _escrowDetailRepository.GetAll()
						.Include( e => e.UserFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Name.Contains(input.Filter) || e.Email.Contains(input.Filter) || e.Company.Contains(input.Filter) || e.EscrowId.Contains(input.Filter) || e.Usertype.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter),  e => e.Name == input.NameFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.EmailFilter),  e => e.Email == input.EmailFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.CompanyFilter),  e => e.Company == input.CompanyFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.EscrowIdFilter),  e => e.EscrowId == input.EscrowIdFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.UsertypeFilter),  e => e.Usertype == input.UsertypeFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserFk != null && e.UserFk.Name == input.UserNameFilter);

			var pagedAndFilteredEscrowDetails = filteredEscrowDetails
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var escrowDetails = from o in pagedAndFilteredEscrowDetails
                         join o1 in _lookup_userRepository.GetAll() on o.UserId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         select new GetEscrowDetailForViewDto() {
							EscrowDetail = new EscrowDetailDto
							{
                                Name = o.Name,
                                Email = o.Email,
                                Company = o.Company,
                                EscrowId = o.EscrowId,
                                Usertype = o.Usertype,
                                Id = o.Id
							},
                         	UserName = s1 == null || s1.Name == null ? "" : s1.Name.ToString()
						};

            var totalCount = await filteredEscrowDetails.CountAsync();

            return new PagedResultDto<GetEscrowDetailForViewDto>(
                totalCount,
                await escrowDetails.ToListAsync()
            );
         }



        public async Task<List<EscrowDetail>> GetEscrowAll()
        {



            return  _escrowDetailRepository.GetAll().ToList();

             
        }


        public async Task<EscrowDetail> GetEscrowOfficerDetails(string EscrowId)
        {

            return _escrowDetailRepository.GetAll().Where(x => x.EscrowId == EscrowId && x.Usertype == "EOX").FirstOrDefault();
        }



        [AbpAuthorize(AppPermissions.Pages_EscrowDetails_Edit)]
		 public async Task<GetEscrowDetailForEditOutput> GetEscrowDetailForEdit(EntityDto<long> input)
         {
            var escrowDetail = await _escrowDetailRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetEscrowDetailForEditOutput {EscrowDetail = ObjectMapper.Map<CreateOrEditEscrowDetailDto>(escrowDetail)};

		    if (output.EscrowDetail.UserId != null)
            {
                var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.EscrowDetail.UserId);
                output.UserName = _lookupUser?.Name?.ToString();
            }
			
            return output;
         }

        [AbpAuthorize(AppPermissions.Pages_EscrowDetails_Edit)]
        public async Task<GetEscrowDetailForEditOutput> GetEscrowDetailForByUserId(long UserId,string escrow,string userType)
        {
            var escrowDetail = await _escrowDetailRepository.FirstOrDefaultAsync(x=> x.UserId == UserId && x.EscrowId == escrow && x.Usertype == userType);

            var output = new GetEscrowDetailForEditOutput { EscrowDetail = ObjectMapper.Map<CreateOrEditEscrowDetailDto>(escrowDetail) };

            if (output.EscrowDetail.UserId != null)
            {
                var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.EscrowDetail.UserId);
                output.UserName = _lookupUser?.Name?.ToString();
            }

            return output;
        }
        public async Task<GetEscrowDetailForEditOutput> GetEscrowDetailsForByStringUserId(string userId, string escrow, string userType)
        {
            var escrowDetail = await _escrowDetailRepository
       .FirstOrDefaultAsync(x => x.UserId.HasValue && x.UserId.Value.ToString() == userId && x.EscrowId == escrow && x.Usertype == userType);

            var output = new GetEscrowDetailForEditOutput
            {
                EscrowDetail = ObjectMapper.Map<CreateOrEditEscrowDetailDto>(escrowDetail)
            };

            if (output.EscrowDetail.UserId != null)
            {
                var _lookupUser = await _lookup_userRepository
                    .FirstOrDefaultAsync(x => x.Id == (long)output.EscrowDetail.UserId);

                output.UserName = _lookupUser?.Name?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditEscrowDetailDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 //[AbpAuthorize(AppPermissions.Pages_EscrowDetails_Create)]
		 protected virtual async Task Create(CreateOrEditEscrowDetailDto input)
         {
            var escrowDetail = ObjectMapper.Map<EscrowDetail>(input);

			

            await _escrowDetailRepository.InsertAsync(escrowDetail);
         }

		 //[AbpAuthorize(AppPermissions.Pages_EscrowDetails_Edit)]
		 protected virtual async Task Update(CreateOrEditEscrowDetailDto input)
         {
            var escrowDetail = await _escrowDetailRepository.FirstOrDefaultAsync((long)input.Id);
             ObjectMapper.Map(input, escrowDetail);
         }

		 [AbpAuthorize(AppPermissions.Pages_EscrowDetails_Delete)]
         public async Task Delete(EntityDto<long> input)
         {
            await _escrowDetailRepository.DeleteAsync(input.Id);
         } 

		[AbpAuthorize(AppPermissions.Pages_EscrowDetails)]
         public async Task<PagedResultDto<EscrowDetailUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_userRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Name != null && e.Name.Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var userList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<EscrowDetailUserLookupTableDto>();
			foreach(var user in userList){
				lookupTableDtoList.Add(new EscrowDetailUserLookupTableDto
				{
					Id = user.Id,
					DisplayName = user.Name?.ToString()
				});
			}

            return new PagedResultDto<EscrowDetailUserLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

        public PagedResultDto<GetEscrowDetailForViewDto> GetAllSync(GetAllEscrowDetailsInput input)
        {

            var filteredEscrowDetails = _escrowDetailRepository.GetAll()
                        .Include(e => e.UserFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter) || e.Email.Contains(input.Filter) || e.Company.Contains(input.Filter) || e.EscrowId.Contains(input.Filter) || e.Usertype.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name == input.NameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.EmailFilter), e => e.Email == input.EmailFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CompanyFilter), e => e.Company == input.CompanyFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.EscrowIdFilter), e => e.EscrowId == input.EscrowIdFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UsertypeFilter), e => e.Usertype == input.UsertypeFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserFk != null && e.UserFk.Name == input.UserNameFilter);

            var pagedAndFilteredEscrowDetails = filteredEscrowDetails
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var escrowDetails = from o in pagedAndFilteredEscrowDetails
                                join o1 in _lookup_userRepository.GetAll() on o.UserId equals o1.Id into j1
                                from s1 in j1.DefaultIfEmpty()

                                select new GetEscrowDetailForViewDto()
                                {
                                    EscrowDetail = new EscrowDetailDto
                                    {
                                        Name = o.Name,
                                        Email = o.Email,
                                        Company = o.Company,
                                        EscrowId = o.EscrowId,
                                        Usertype = o.Usertype,
                                        Id = o.Id
                                    },
                                    UserName = s1 == null || s1.Name == null ? "" : s1.Name.ToString()
                                };

            var totalCount = filteredEscrowDetails.Count();

            return new PagedResultDto<GetEscrowDetailForViewDto>(
                totalCount,
                 escrowDetails.ToList()
            );
        }

    }
}