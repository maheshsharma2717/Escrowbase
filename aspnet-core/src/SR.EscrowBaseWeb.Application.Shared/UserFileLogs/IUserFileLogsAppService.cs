using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.UserFileLogs.Dtos;
using SR.EscrowBaseWeb.Dto;


namespace SR.EscrowBaseWeb.UserFileLogs
{
    public interface IUserFileLogsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetUserFileLogForViewDto>> GetAll(GetAllUserFileLogsInput input);

		Task<GetUserFileLogForEditOutput> GetUserFileLogForEdit(EntityDto<long> input);

		Task CreateOrEdit(CreateOrEditUserFileLogDto input);

		Task Delete(EntityDto<long> input);

		
		Task<PagedResultDto<UserFileLogUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input);
		
    }
}