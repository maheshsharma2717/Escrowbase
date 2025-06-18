using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.EscrowDetails.Dtos;
using SR.EscrowBaseWeb.Dto;


namespace SR.EscrowBaseWeb.EscrowDetails
{
    public interface IEscrowDetailsAppService : IApplicationService
    {
        Task<PagedResultDto<GetEscrowDetailForViewDto>> GetAll(GetAllEscrowDetailsInput input);

        Task<GetEscrowDetailForEditOutput> GetEscrowDetailForEdit(EntityDto<long> input);

        Task CreateOrEdit(CreateOrEditEscrowDetailDto input);

        Task Delete(EntityDto<long> input);


        Task<PagedResultDto<EscrowDetailUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input);
        PagedResultDto<GetEscrowDetailForViewDto> GetAllSync(GetAllEscrowDetailsInput input);

    }
}