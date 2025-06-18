using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.SrAssignedFilesDetails.Dtos;
using SR.EscrowBaseWeb.Dto;

namespace SR.EscrowBaseWeb.SrAssignedFilesDetails
{
    public interface ISrAssignedFilesDetailsAppService : IApplicationService
    {
        Task<PagedResultDto<GetSrAssignedFilesDetailForViewDto>> GetAll(GetAllSrAssignedFilesDetailsInput input);

        Task<GetSrAssignedFilesDetailForViewDto> GetSrAssignedFilesDetailForView(long id);

        Task<GetSrAssignedFilesDetailForEditOutput> GetSrAssignedFilesDetailForEdit(EntityDto<long> input);

        Task CreateOrEdit(CreateOrEditSrAssignedFilesDetailDto input);

        Task Delete(EntityDto<long> input);

    }
}