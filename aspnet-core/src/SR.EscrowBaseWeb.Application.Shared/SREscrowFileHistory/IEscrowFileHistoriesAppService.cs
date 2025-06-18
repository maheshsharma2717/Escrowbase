using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.SREscrowFileHistory.Dtos;
using SR.EscrowBaseWeb.Dto;

namespace SR.EscrowBaseWeb.SREscrowFileHistory
{
    public interface IEscrowFileHistoriesAppService : IApplicationService
    {
        Task<PagedResultDto<GetEscrowFileHistoryForViewDto>> GetAll(GetAllEscrowFileHistoriesInput input);

        Task<GetEscrowFileHistoryForViewDto> GetEscrowFileHistoryForView(long id);

        Task<GetEscrowFileHistoryForEditOutput> GetEscrowFileHistoryForEdit(EntityDto<long> input);

        Task CreateOrEdit(CreateOrEditEscrowFileHistoryDto input);

        Task Delete(EntityDto<long> input);

        Task<PagedResultDto<EscrowFileHistoryUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input);

        Task<long?> GetUserIdFromSession();

    }
}