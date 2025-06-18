using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.EscrowFileMaster.Dtos;
using SR.EscrowBaseWeb.Dto;

namespace SR.EscrowBaseWeb.EscrowFileMaster
{
    public interface ISREscrowFileMastersAppService : IApplicationService
    {
        Task<PagedResultDto<GetSREscrowFileMasterForViewDto>> GetAll(GetAllSREscrowFileMastersInput input);

        Task<GetSREscrowFileMasterForViewDto> GetSREscrowFileMasterForView(long id);

        Task<GetSREscrowFileMasterForEditOutput> GetSREscrowFileMasterForEdit(EntityDto<long> input);

        Task CreateOrEdit(CreateOrEditSREscrowFileMasterDto input);

        Task Delete(EntityDto<long> input);

    }
}