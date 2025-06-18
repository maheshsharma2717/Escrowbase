using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.SRUserAnswer.Dtos;
using SR.EscrowBaseWeb.Dto;


namespace SR.EscrowBaseWeb.SRUserAnswer
{
    public interface IUserAnswersAppService : IApplicationService 
    {
        Task<PagedResultDto<GetUserAnswerForViewDto>> GetAll(GetAllUserAnswersInput input);

        Task<GetUserAnswerForViewDto> GetUserAnswerForView(int id);

		Task<GetUserAnswerForEditOutput> GetUserAnswerForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditUserAnswerDto input);

		Task Delete(EntityDto input);

		
		Task<PagedResultDto<UserAnswerUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input);
		
    }
}