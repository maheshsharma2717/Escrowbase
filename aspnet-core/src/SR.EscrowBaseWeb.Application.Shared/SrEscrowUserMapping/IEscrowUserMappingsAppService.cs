using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.SrEscrowUserMapping.Dtos;
using SR.EscrowBaseWeb.Dto;


namespace SR.EscrowBaseWeb.SrEscrowUserMapping
{
    public interface IEscrowUserMappingsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetEscrowUserMappingForViewDto>> GetAll(GetAllEscrowUserMappingsInput input);

		Task<GetEscrowUserMappingForEditOutput> GetEscrowUserMappingForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditEscrowUserMappingDto input);

		Task Delete(EntityDto input);

		
		Task<PagedResultDto<EscrowUserMappingUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<EscrowUserMappingEscrowClientLookupTableDto>> GetAllEscrowClientForLookupTable(GetAllForLookupTableInput input);
		
    }
}