using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.E_SignRecords.Dtos;
using SR.EscrowBaseWeb.Dto;

namespace SR.EscrowBaseWeb.E_SignRecords
{
    public interface IE_SignRecordsAppService : IApplicationService
    {
        Task<PagedResultDto<GetE_SignRecordForViewDto>> GetAll(GetAllE_SignRecordsInput input);

        Task<GetE_SignRecordForViewDto> GetE_SignRecordForView(long id);

        Task<GetE_SignRecordForEditOutput> GetE_SignRecordForEdit(EntityDto<long> input);

        Task<string> CreateOrEdit(CreateOrEditE_SignRecordDto input);

        Task Delete(EntityDto<long> input);

    }
}