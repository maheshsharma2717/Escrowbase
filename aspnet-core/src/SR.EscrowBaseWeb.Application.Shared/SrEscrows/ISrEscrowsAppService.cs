using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.SrEscrows.Dtos;
using SR.EscrowBaseWeb.Dto;

namespace SR.EscrowBaseWeb.SrEscrows
{
    public interface ISrEscrowsAppService : IApplicationService
    {
        Task<PagedResultDto<GetSrEscrowForViewDto>> GetAll(GetAllSrEscrowsInput input);

        Task<GetSrEscrowForEditOutput> GetSrEscrowForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditSrEscrowDto input);

        Task Delete(EntityDto input);

    }
}