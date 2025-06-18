using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using SR.EscrowBaseWeb.SrEscrows.Dtos;
using SR.EscrowBaseWeb.Dto;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace SR.EscrowBaseWeb.SrEscrows
{
    //[AbpAuthorize(AppPermissions.Pages_SrEscrows)]
    public class SrEscrowsAppService : EscrowBaseWebAppServiceBase, ISrEscrowsAppService
    {
        private readonly IRepository<SrEscrow> _srEscrowRepository;
        //private readonly IRepository<enterpris> _srEscrowRepository;

        public SrEscrowsAppService(IRepository<SrEscrow> srEscrowRepository)
        {
            _srEscrowRepository = srEscrowRepository;

        }

        public async Task<PagedResultDto<GetSrEscrowForViewDto>> GetAll(GetAllSrEscrowsInput input)
        {

            var filteredSrEscrows = _srEscrowRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.EscrowNo.Contains(input.Filter) || e.PropertyAddress.Contains(input.Filter) || e.EscrowOfficerName.Contains(input.Filter) || e.EOEmail.Contains(input.Filter) || e.EOPhone.Contains(input.Filter) || e.EoPhoneExt.Contains(input.Filter) || e.EoPhoneCell.Contains(input.Filter) || e.SubCompanyName.Contains(input.Filter) || e.CustomDetails.Contains(input.Filter) || e.Logo.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.EscrowNoFilter), e => e.EscrowNo == input.EscrowNoFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.PropertyAddressFilter), e => e.PropertyAddress == input.PropertyAddressFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.EscrowOfficerNameFilter), e => e.EscrowOfficerName == input.EscrowOfficerNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.EOEmailFilter), e => e.EOEmail == input.EOEmailFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.EOPhoneFilter), e => e.EOPhone == input.EOPhoneFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.EoPhoneExtFilter), e => e.EoPhoneExt == input.EoPhoneExtFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.EoPhoneCellFilter), e => e.EoPhoneCell == input.EoPhoneCellFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.SubCompanyNameFilter), e => e.SubCompanyName == input.SubCompanyNameFilter)
                        .WhereIf(input.MinEnterpriseIdFilter != null, e => e.EnterpriseId >= input.MinEnterpriseIdFilter)
                        .WhereIf(input.MaxEnterpriseIdFilter != null, e => e.EnterpriseId <= input.MaxEnterpriseIdFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CustomDetailsFilter), e => e.CustomDetails == input.CustomDetailsFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.LogoFilter), e => e.Logo == input.LogoFilter);

            var pagedAndFilteredSrEscrows = filteredSrEscrows
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var srEscrows = from o in pagedAndFilteredSrEscrows
                            select new GetSrEscrowForViewDto()
                            {
                                SrEscrow = new SrEscrowDto
                                {
                                    EscrowNo = o.EscrowNo,
                                    PropertyAddress = o.PropertyAddress,
                                    EscrowOfficerName = o.EscrowOfficerName,
                                    EOEmail = o.EOEmail,
                                    EOPhone = o.EOPhone,
                                    EoPhoneExt = o.EoPhoneExt,
                                    EoPhoneCell = o.EoPhoneCell,
                                    SubCompanyName = o.SubCompanyName,
                                    EnterpriseId = o.EnterpriseId,
                                    CustomDetails = o.CustomDetails,
                                    Logo = o.Logo,
                                    Id = o.Id
                                }
                            };

            var totalCount = await filteredSrEscrows.CountAsync();

            return new PagedResultDto<GetSrEscrowForViewDto>(
                totalCount,
                await srEscrows.ToListAsync()
            );
        }

        [AbpAuthorize(AppPermissions.Pages_SrEscrows_Edit)]
        public async Task<GetSrEscrowForEditOutput> GetSrEscrowForEdit(EntityDto input)
        {
            var srEscrow = await _srEscrowRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetSrEscrowForEditOutput { SrEscrow = ObjectMapper.Map<CreateOrEditSrEscrowDto>(srEscrow) };

            return output;
        }

        public async Task<GetSrEscrowForEditOutput> GetSrEscrowJoinwithEnterprise(EntityDto input)
        {
            
                //var person = (from p in _srEscrowRepository
                //              join e in db.EmailAddresses
                //              on p.BusinessEntityID equals e.BusinessEntityID
                //              where p.FirstName == "KEN"
                //              select new
                //              {
                //                  ID = p.BusinessEntityID,
                //                  FirstName = p.FirstName,
                //                  MiddleName = p.MiddleName,
                //                  LastName = p.LastName,
                //                  EmailID = e.EmailAddress1
                //              }).ToList();

                //foreach (var p in person)
                //{
                //    Console.WriteLine("{0} {1} {2} {3} {4}", p.ID, p.FirstName, p.MiddleName, p.LastName, p.EmailID);
                //}
            
            var srEscrow = await _srEscrowRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetSrEscrowForEditOutput { SrEscrow = ObjectMapper.Map<CreateOrEditSrEscrowDto>(srEscrow) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditSrEscrowDto input)
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

        //[AbpAuthorize(AppPermissions.Pages_SrEscrows_Create)]
        protected virtual async Task Create(CreateOrEditSrEscrowDto input)
        {
            var srEscrow = ObjectMapper.Map<SrEscrow>(input);

            await _srEscrowRepository.InsertAsync(srEscrow);
        }

        //[AbpAuthorize(AppPermissions.Pages_SrEscrows_Edit)]
        protected virtual async Task Update(CreateOrEditSrEscrowDto input)
        {
            var srEscrow = await _srEscrowRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, srEscrow);
        }

        [AbpAuthorize(AppPermissions.Pages_SrEscrows_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _srEscrowRepository.DeleteAsync(input.Id);
        }
    }
}