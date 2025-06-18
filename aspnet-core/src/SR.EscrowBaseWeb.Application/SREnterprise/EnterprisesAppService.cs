

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using SR.EscrowBaseWeb.SREnterprise.Exporting;
using SR.EscrowBaseWeb.SREnterprise.Dtos;
using SR.EscrowBaseWeb.Dto;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace SR.EscrowBaseWeb.SREnterprise
{
	[AbpAuthorize(AppPermissions.Pages_Enterprises)]
    public class EnterprisesAppService : EscrowBaseWebAppServiceBase, IEnterprisesAppService
    {
		 private readonly IRepository<Enterprise> _enterpriseRepository;
		 private readonly IEnterprisesExcelExporter _enterprisesExcelExporter;
		 

		  public EnterprisesAppService(IRepository<Enterprise> enterpriseRepository, IEnterprisesExcelExporter enterprisesExcelExporter ) 
		  {
			_enterpriseRepository = enterpriseRepository;
			_enterprisesExcelExporter = enterprisesExcelExporter;
			
		  }

		 public async Task<PagedResultDto<GetEnterpriseForViewDto>> GetAll(GetAllEnterprisesInput input)
         {
			
			var filteredEnterprises = _enterpriseRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.EnterpriseName.Contains(input.Filter) || e.Email.Contains(input.Filter) || e.Phone.Contains(input.Filter) || e.Address1.Contains(input.Filter) || e.Address2.Contains(input.Filter) || e.City.Contains(input.Filter) || e.State.Contains(input.Filter) || e.PinCode.Contains(input.Filter) || e.Contry.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.EnterpriseNameFilter),  e => e.EnterpriseName == input.EnterpriseNameFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.EmailFilter),  e => e.Email == input.EmailFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.PhoneFilter),  e => e.Phone == input.PhoneFilter);

			var pagedAndFilteredEnterprises = filteredEnterprises
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var enterprises = from o in pagedAndFilteredEnterprises
                              select new GetEnterpriseForViewDto() {
                                  Enterprise = new EnterpriseDto
                                  {
                                      EnterpriseName = o.EnterpriseName,
                                      Email = o.Email,
                                      Phone = o.Phone,
                                      Address1 = o.Address1,
                                      Address2 = o.Address2,
                                      City = o.City,
                                      State = o.State,
                                      PinCode = o.PinCode,
                                      Contry = o.Contry,
                                      Id = o.Id,
                                      EnterpriseExt = o.EnterpriseExt,
                                      PrimaryContact = o.PrimaryContact,
                                      OfficePhone = o.OfficePhone,
                                      SecondaryEnterpriseEmail = o.SecondaryEnterpriseEmail,
                                      DefaultTitle = o.DefaultTitle,
                                      LicenesNo = o.LicenesNo,
                                      EnterpriseId =o.EnterpriseId
                                  }
						};

            var totalCount = await filteredEnterprises.CountAsync();

            return new PagedResultDto<GetEnterpriseForViewDto>(
                totalCount,
                await enterprises.ToListAsync()
            );
         }
		 
		 public async Task<GetEnterpriseForViewDto> GetEnterpriseForView(int id)
         {
            var enterprise = await _enterpriseRepository.GetAsync(id);

            var output = new GetEnterpriseForViewDto { Enterprise = ObjectMapper.Map<EnterpriseDto>(enterprise) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Enterprises_Edit)]
		 public async Task<GetEnterpriseForEditOutput> GetEnterpriseForEdit(EntityDto input)
         {
            var enterprise = await _enterpriseRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetEnterpriseForEditOutput {Enterprise = ObjectMapper.Map<CreateOrEditEnterpriseDto>(enterprise)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditEnterpriseDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Enterprises_Create)]
		 protected virtual async Task Create(CreateOrEditEnterpriseDto input)
         {
            var enterprise = ObjectMapper.Map<Enterprise>(input);

			
			if (AbpSession.TenantId != null)
			{
				enterprise.TenantId = (int?) AbpSession.TenantId;
			}
		if(enterprise.ParentId == null)
            {
                enterprise.DBName = "sre" +enterprise.EnterpriseName;
            }

            await _enterpriseRepository.InsertAsync(enterprise);
         }

		 [AbpAuthorize(AppPermissions.Pages_Enterprises_Edit)]
		 protected virtual async Task Update(CreateOrEditEnterpriseDto input)
         {
            var enterprise = await _enterpriseRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, enterprise);
         }

		 [AbpAuthorize(AppPermissions.Pages_Enterprises_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _enterpriseRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetEnterprisesToExcel(GetAllEnterprisesForExcelInput input)
         {
			
			var filteredEnterprises = _enterpriseRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.EnterpriseName.Contains(input.Filter) || e.Email.Contains(input.Filter) || e.Phone.Contains(input.Filter) || e.Address1.Contains(input.Filter) || e.Address2.Contains(input.Filter) || e.City.Contains(input.Filter) || e.State.Contains(input.Filter) || e.PinCode.Contains(input.Filter) || e.Contry.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.EnterpriseNameFilter),  e => e.EnterpriseName == input.EnterpriseNameFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.EmailFilter),  e => e.Email == input.EmailFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.PhoneFilter),  e => e.Phone == input.PhoneFilter);

			var query = (from o in filteredEnterprises
                         select new GetEnterpriseForViewDto() { 
							Enterprise = new EnterpriseDto
							{
                                EnterpriseName = o.EnterpriseName,
                                Email = o.Email,
                                Phone = o.Phone,
                                Address1 = o.Address1,
                                Address2 = o.Address2,
                                City = o.City,
                                State = o.State,
                                PinCode = o.PinCode,
                                Contry = o.Contry,
                                Id = o.Id
							}
						 });


            var enterpriseListDtos = await query.ToListAsync();

            return _enterprisesExcelExporter.ExportToFile(enterpriseListDtos);
         }


    }
}