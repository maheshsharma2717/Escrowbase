

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using SR.EscrowBaseWeb.SREscrowClient.Dtos;
using SR.EscrowBaseWeb.Dto;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace SR.EscrowBaseWeb.SREscrowClient
{
	[AbpAuthorize(AppPermissions.Pages_EscrowClients)]
    public class EscrowClientsAppService : EscrowBaseWebAppServiceBase, IEscrowClientsAppService
    {
		 private readonly IRepository<EscrowClient> _escrowClientRepository;
		 

		  public EscrowClientsAppService(IRepository<EscrowClient> escrowClientRepository ) 
		  {
			_escrowClientRepository = escrowClientRepository;
			
		  }

		 public async Task<PagedResultDto<GetEscrowClientForViewDto>> GetAll(GetAllEscrowClientsInput input)
         {
			
			var filteredEscrowClients = _escrowClientRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.EscrowNumber.Contains(input.Filter) || e.Name.Contains(input.Filter) || e.Email.Contains(input.Filter) || e.Phone.Contains(input.Filter) || e.Address.Contains(input.Filter) || e.State.Contains(input.Filter) || e.Pincode.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.EscrowNumberFilter),  e => e.EscrowNumber == input.EscrowNumberFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter),  e => e.Name == input.NameFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.EmailFilter),  e => e.Email == input.EmailFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.PhoneFilter),  e => e.Phone == input.PhoneFilter);

			var pagedAndFilteredEscrowClients = filteredEscrowClients
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var escrowClients = from o in pagedAndFilteredEscrowClients
                         select new GetEscrowClientForViewDto() {
							EscrowClient = new EscrowClientDto
							{
                                EscrowNumber = o.EscrowNumber,
                                Name = o.Name,
                                Email = o.Email,
                                Phone = o.Phone,
                                Address = o.Address,
                                State = o.State,
                                Pincode = o.Pincode,
                                Id = o.Id
							}
						};

            var totalCount = await filteredEscrowClients.CountAsync();

            return new PagedResultDto<GetEscrowClientForViewDto>(
                totalCount,
                await escrowClients.ToListAsync()
            );
         }
		 
		 public async Task<GetEscrowClientForViewDto> GetEscrowClientForView(int id)
         {
            var escrowClient = await _escrowClientRepository.GetAsync(id);

            var output = new GetEscrowClientForViewDto { EscrowClient = ObjectMapper.Map<EscrowClientDto>(escrowClient) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_EscrowClients_Edit)]
		 public async Task<GetEscrowClientForEditOutput> GetEscrowClientForEdit(EntityDto input)
         {
            var escrowClient = await _escrowClientRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetEscrowClientForEditOutput {EscrowClient = ObjectMapper.Map<CreateOrEditEscrowClientDto>(escrowClient)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditEscrowClientDto input)
         {
            try
            {
                if (input.Id == null)
                {
                    await Create(input);
                    //Create.Wait();
                }
                else
                {
                    await Update(input);
                }
            }
            catch(Exception ex)
            {

            }
         }

		 [AbpAuthorize(AppPermissions.Pages_EscrowClients_Create)]
		 protected virtual async Task Create(CreateOrEditEscrowClientDto input)
         {
            EscrowClient es = new EscrowClient();
            es.Name = input.Name;
            es.Email = input.Email;
            es.EscrowNumber = input.EscrowNumber;
            //var escrowClient = es.(input);

			

            await _escrowClientRepository.InsertAsync(es);
         }

		 [AbpAuthorize(AppPermissions.Pages_EscrowClients_Edit)]
		 protected virtual async Task Update(CreateOrEditEscrowClientDto input)
         {
            var escrowClient = await _escrowClientRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, escrowClient);
         }

		 [AbpAuthorize(AppPermissions.Pages_EscrowClients_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _escrowClientRepository.DeleteAsync(input.Id);
         } 
    }
}