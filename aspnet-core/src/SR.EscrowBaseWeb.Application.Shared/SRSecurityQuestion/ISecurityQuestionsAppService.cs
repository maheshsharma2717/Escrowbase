using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.SRSecurityQuestion.Dtos;
using SR.EscrowBaseWeb.Dto;


namespace SR.EscrowBaseWeb.SRSecurityQuestion
{
    public interface ISecurityQuestionsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetSecurityQuestionForViewDto>> GetAll(GetAllSecurityQuestionsInput input);

        Task<GetSecurityQuestionForViewDto> GetSecurityQuestionForView(int id);

		Task<GetSecurityQuestionForEditOutput> GetSecurityQuestionForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditSecurityQuestionDto input);

		Task Delete(EntityDto input);

		
    }
}