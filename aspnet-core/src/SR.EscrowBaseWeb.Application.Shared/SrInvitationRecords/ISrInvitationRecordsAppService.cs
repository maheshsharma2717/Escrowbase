using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.SrInvitationRecords.Dtos;
using SR.EscrowBaseWeb.Dto;


namespace SR.EscrowBaseWeb.SrInvitationRecords
{
    public interface ISrInvitationRecordsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetSrInvitationRecordForViewDto>> GetAll(GetAllSrInvitationRecordsInput input);

		Task<GetSrInvitationRecordForEditOutput> GetSrInvitationRecordForEdit(EntityDto<long> input);

		Task CreateOrEdit(CreateOrEditSrInvitationRecordDto input);

		Task Delete(EntityDto<long> input);

		
		Task<PagedResultDto<SrInvitationRecordUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input);
		
    }
}