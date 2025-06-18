using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.EscrowFileReminder.Dtos;
using SR.EscrowBaseWeb.Dto;
using System.Collections.Generic;
using SR.EscrowBaseWeb.SrAssignedFilesDetails;

namespace SR.EscrowBaseWeb.EscrowFileReminder
{
    public interface ISrEscrowFileRemindersAppService : IApplicationService
    {
        Task<PagedResultDto<GetSrEscrowFileReminderForViewDto>> GetAll(GetAllSrEscrowFileRemindersInput input);

        Task<GetSrEscrowFileReminderForViewDto> GetSrEscrowFileReminderForView(long id);

        Task<GetSrEscrowFileReminderForEditOutput> GetSrEscrowFileReminderForEdit(EntityDto<long> input);

        Task Delete(EntityDto<long> input);
        Task<PagedResultDto<SrEscrowFileReminderSREscrowFileMasterLookupTableDto>> GetAllSREscrowFileMasterForLookupTable(GetAllForLookupTableInput input);
        Task<PagedResultDto<SrEscrowFileReminderUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input);
        Task<List<AssignedFileUser>> CreateOrEdit(CreateOrEditSrEscrowFileReminderDto input);
    }
}