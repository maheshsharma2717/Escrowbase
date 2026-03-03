using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.GetCurrentEscrow.Dtos;
using SR.EscrowBaseWeb.Dto;

namespace SR.EscrowBaseWeb.GetCurrentEscrow
{
    public interface ICurrentEscrowsAppService : IApplicationService
    {
        Task<PagedResultDto<GetCurrentEscrowForViewDto>> GetAll(GetAllCurrentEscrowsInput input);

        Task<GetCurrentEscrowForViewDto> GetCurrentEscrowForView(int id);

        Task<GetCurrentEscrowForEditOutput> GetCurrentEscrowForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditCurrentEscrowDto input);

        Task Delete(EntityDto input);
    }
}