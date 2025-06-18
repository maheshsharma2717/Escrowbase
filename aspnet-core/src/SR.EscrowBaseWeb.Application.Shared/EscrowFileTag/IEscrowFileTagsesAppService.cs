using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.EscrowFileTag.Dtos;
using SR.EscrowBaseWeb.Dto;

namespace SR.EscrowBaseWeb.EscrowFileTag
{
    public interface IEscrowFileTagsesAppService : IApplicationService
    {
        Task<PagedResultDto<GetEscrowFileTagsForViewDto>> GetAll(GetAllEscrowFileTagsesInput input);

        Task<GetEscrowFileTagsForViewDto> GetEscrowFileTagsForView(int id);

        Task<GetEscrowFileTagsForEditOutput> GetEscrowFileTagsForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditEscrowFileTagsDto input);

        Task Delete(EntityDto input);

    }
}