using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.EsignCompany.Dtos;
using SR.EscrowBaseWeb.Dto;

namespace SR.EscrowBaseWeb.EsignCompany
{
    public interface IEsignCompanyMappingsAppService : IApplicationService
    {
        Task<PagedResultDto<GetEsignCompanyMappingForViewDto>> GetAll(GetAllEsignCompanyMappingsInput input);

        Task<GetEsignCompanyMappingForViewDto> GetEsignCompanyMappingForView(int id);

        Task<GetEsignCompanyMappingForEditOutput> GetEsignCompanyMappingForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditEsignCompanyMappingDto input);

        Task Delete(EntityDto input);

    }
}