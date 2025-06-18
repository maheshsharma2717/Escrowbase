using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.EsignRoleMapping.Dtos;
using SR.EscrowBaseWeb.Dto;

namespace SR.EscrowBaseWeb.EsignRoleMapping
{
    public interface IEsignRoleMappingsesAppService : IApplicationService
    {
        Task<PagedResultDto<GetEsignRoleMappingsForViewDto>> GetAll(GetAllEsignRoleMappingsesInput input);

        Task<GetEsignRoleMappingsForEditOutput> GetEsignRoleMappingsForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditEsignRoleMappingsDto input);

        Task Delete(EntityDto input);

    }
}