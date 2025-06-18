 
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using SR.EscrowBaseWeb.SRSecurityQuestion.Dtos;
using SR.EscrowBaseWeb.Dto;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace SR.EscrowBaseWeb.SRSecurityQuestion
{

    public class SecurityQuestionsAppService : EscrowBaseWebAppServiceBase, ISecurityQuestionsAppService
    {
		 private readonly IRepository<SecurityQuestion> _securityQuestionRepository;
		 

		  public SecurityQuestionsAppService(IRepository<SecurityQuestion> securityQuestionRepository ) 
		  {
			_securityQuestionRepository = securityQuestionRepository;
			
		  }

		 public async Task<PagedResultDto<GetSecurityQuestionForViewDto>> GetAll(GetAllSecurityQuestionsInput input)
         {
			
			var filteredSecurityQuestions = _securityQuestionRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Question.Contains(input.Filter));

			var pagedAndFilteredSecurityQuestions = filteredSecurityQuestions
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var securityQuestions = from o in pagedAndFilteredSecurityQuestions
                         select new GetSecurityQuestionForViewDto() {
							SecurityQuestion = new SecurityQuestionDto
							{
                                Question = o.Question,
                                Id = o.Id
							}
						};

            var totalCount = await filteredSecurityQuestions.CountAsync();

            return new PagedResultDto<GetSecurityQuestionForViewDto>(
                totalCount,
                await securityQuestions.ToListAsync()
            );
         }
		 
		 public async Task<GetSecurityQuestionForViewDto> GetSecurityQuestionForView(int id)
         {
            var securityQuestion = await _securityQuestionRepository.GetAsync(id);

            var output = new GetSecurityQuestionForViewDto { SecurityQuestion = ObjectMapper.Map<SecurityQuestionDto>(securityQuestion) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_SecurityQuestions_Edit)]
		 public async Task<GetSecurityQuestionForEditOutput> GetSecurityQuestionForEdit(EntityDto input)
         {
            var securityQuestion = await _securityQuestionRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetSecurityQuestionForEditOutput {SecurityQuestion = ObjectMapper.Map<CreateOrEditSecurityQuestionDto>(securityQuestion)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditSecurityQuestionDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_SecurityQuestions_Create)]
		 protected virtual async Task Create(CreateOrEditSecurityQuestionDto input)
         {
            var securityQuestion = ObjectMapper.Map<SecurityQuestion>(input);

			

            await _securityQuestionRepository.InsertAsync(securityQuestion);
         }

		 [AbpAuthorize(AppPermissions.Pages_SecurityQuestions_Edit)]
		 protected virtual async Task Update(CreateOrEditSecurityQuestionDto input)
         {
            var securityQuestion = await _securityQuestionRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, securityQuestion);
         }

		 [AbpAuthorize(AppPermissions.Pages_SecurityQuestions_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _securityQuestionRepository.DeleteAsync(input.Id);
         } 
    }
}