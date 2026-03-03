using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Collections.Generic;
using SR.EscrowBaseWeb.EscrowHistory.Dtos;
using SR.EscrowBaseWeb.Dto;

namespace SR.EscrowBaseWeb.EscrowHistory
{
    public interface IEscrowAccessHistoriesAppService : IApplicationService
    {
        Task<PagedResultDto<GetEscrowAccessHistoryForViewDto>> GetAll(GetAllEscrowAccessHistoriesInput input);

        Task<GetEscrowAccessHistoryForViewDto> GetEscrowAccessHistoryForView(int id);

        Task<GetEscrowAccessHistoryForEditOutput> GetEscrowAccessHistoryForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditEscrowAccessHistoryDto input);

        Task Delete(EntityDto input);

        Task<PagedResultDto<EscrowAccessHistoryUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input);

        Task<PagedResultDto<EscrowAccessHistoryEscrowClientLookupTableDto>> GetAllEscrowClientForLookupTable(GetAllForLookupTableInput input);

        Task LogAccess(LogAccessInput input);

        Task<System.Collections.Generic.List<RecentEscrowDto>> GetRecentEscrows();

    }
}