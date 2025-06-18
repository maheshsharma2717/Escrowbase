using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using SR.EscrowBaseWeb.SRFileMapping.Dtos;
using SR.EscrowBaseWeb.Dto;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace SR.EscrowBaseWeb.SRFileMapping
{
 
    public class SrFileMappingsAppService : EscrowBaseWebAppServiceBase, ISrFileMappingsAppService
    {
		 private readonly IRepository<SrFileMapping> _srFileMappingRepository;
		 

		  public SrFileMappingsAppService(IRepository<SrFileMapping> srFileMappingRepository ) 
		  {
			_srFileMappingRepository = srFileMappingRepository;
			
		  }

		 public   List<GetSrFileMappingForViewDto> GetAll(GetAllSrFileMappingsInput input)
         {
			
			var filteredSrFileMappings = _srFileMappingRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e =>  e.UserId == int.Parse(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.FileNameFilter),  e => e.FileName == input.FileNameFilter)
						.WhereIf(input.MinUserIdFilter != null, e => e.UserId >= input.MinUserIdFilter)
						.WhereIf(input.MaxUserIdFilter != null, e => e.UserId <= input.MaxUserIdFilter)
						.WhereIf(input.IsActiveFilter.HasValue && input.IsActiveFilter > -1,  e => (input.IsActiveFilter == 1 && e.IsActive) || (input.IsActiveFilter == 0 && !e.IsActive) )
						.WhereIf(!string.IsNullOrWhiteSpace(input.ActionFilter),  e => e.Action == input.ActionFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.EscrowiIdFilter),  e => e.EscrowiId == input.EscrowiIdFilter);

            var pagedAndFilteredSrFileMappings = filteredSrFileMappings
                .OrderBy(input.Sorting ?? "id asc");
                

			var srFileMappings = from o in pagedAndFilteredSrFileMappings
                         select new GetSrFileMappingForViewDto() {
							SrFileMapping = new SrFileMappingDto
							{
                                FileName = o.FileName,
                                UserId = o.UserId,
                                IsActive = o.IsActive,
                                Action = o.Action,
                                EscrowiId = o.EscrowiId,
                                Id = o.Id
							}
						};

            var totalCount =   filteredSrFileMappings.Count();

            return srFileMappings.ToList();
            
         }
		 
		 public async Task<GetSrFileMappingForViewDto> GetSrFileMappingForView(int id)
         {
            var srFileMapping = await _srFileMappingRepository.GetAsync(id);

            var output = new GetSrFileMappingForViewDto { SrFileMapping = ObjectMapper.Map<SrFileMappingDto>(srFileMapping) };
			
            return output;
         }
       

        [AbpAuthorize(AppPermissions.Pages_SrFileMappings_Edit)]
		 public async Task<GetSrFileMappingForEditOutput> GetSrFileMappingForEdit(EntityDto input)
         {
            var srFileMapping = await _srFileMappingRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetSrFileMappingForEditOutput {SrFileMapping = ObjectMapper.Map<CreateOrEditSrFileMappingDto>(srFileMapping)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditSrFileMappingDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }
        public CreateOrEditSrFileMappingDto CreateOrEditdata(CreateOrEditSrFileMappingDto input)
        {
            if (input.Id == null)
            {
                return Createdata(input);
            }
            else
            {
                return Updatedata(input);
            }
        }

        //[AbpAuthorize(AppPermissions.Pages_SrFileMappings_Create)]
        protected virtual async Task Create(CreateOrEditSrFileMappingDto input)
         {
            var srFileMapping = ObjectMapper.Map<SrFileMapping>(input);

            await _srFileMappingRepository.InsertAsync(srFileMapping);
         }
        protected virtual CreateOrEditSrFileMappingDto Createdata(CreateOrEditSrFileMappingDto input)
        {
            var srFileMapping = ObjectMapper.Map<SrFileMapping>(input);

            var Id = _srFileMappingRepository.InsertAndGetId(srFileMapping);
            CurrentUnitOfWork.SaveChanges();
            input.Id = Id;
            return input;
        }

        [AbpAuthorize(AppPermissions.Pages_SrFileMappings_Edit)]
		 protected virtual async Task Update(CreateOrEditSrFileMappingDto input)
         {
            var srFileMapping = await _srFileMappingRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, srFileMapping);
         }
        protected virtual CreateOrEditSrFileMappingDto Updatedata(CreateOrEditSrFileMappingDto input)
        {
            var srFileMapping =  _srFileMappingRepository.FirstOrDefault((int)input.Id);
            ObjectMapper.Map(input, srFileMapping);
            return input;
        }

        [AbpAuthorize(AppPermissions.Pages_SrFileMappings_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _srFileMappingRepository.DeleteAsync(input.Id);
         } 
    }
}