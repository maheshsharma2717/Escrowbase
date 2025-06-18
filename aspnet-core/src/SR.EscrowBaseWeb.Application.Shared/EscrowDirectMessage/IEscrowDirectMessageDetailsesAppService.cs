using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.EscrowDirectMessage.Dtos;
using SR.EscrowBaseWeb.Dto;

namespace SR.EscrowBaseWeb.EscrowDirectMessage
{
    public interface IEscrowDirectMessageDetailsesAppService : IApplicationService
    {
        Task<PagedResultDto<GetEscrowDirectMessageDetailsForViewDto>> GetAll(GetAllEscrowDirectMessageDetailsesInput input);

        Task<GetEscrowDirectMessageDetailsForViewDto> GetEscrowDirectMessageDetailsForView(long id);

        Task<GetEscrowDirectMessageDetailsForEditOutput> GetEscrowDirectMessageDetailsForEdit(EntityDto<long> input);

        Task CreateOrEdit(CreateOrEditEscrowDirectMessageDetailsDto input);

        Task Delete(EntityDto<long> input);

        Task<PagedResultDto<EscrowDirectMessageDetailsUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input);

    }
}