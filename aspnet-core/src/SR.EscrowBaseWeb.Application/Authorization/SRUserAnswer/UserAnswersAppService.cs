using SR.EscrowBaseWeb.Authorization.Users;


using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using SR.EscrowBaseWeb.SRUserAnswer.Dtos;
using SR.EscrowBaseWeb.Dto;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace SR.EscrowBaseWeb.SRUserAnswer
{
	
    public class UserAnswersAppService : EscrowBaseWebAppServiceBase, IUserAnswersAppService
    {
		 private readonly IRepository<UserAnswer> _userAnswerRepository;
		 private readonly IRepository<User,long> _lookup_userRepository;
		 

		  public UserAnswersAppService(IRepository<UserAnswer> userAnswerRepository , IRepository<User, long> lookup_userRepository) 
		  {
			_userAnswerRepository = userAnswerRepository;
			_lookup_userRepository = lookup_userRepository;
		
		  }

		 public async Task<PagedResultDto<GetUserAnswerForViewDto>> GetAll(GetAllUserAnswersInput input)
         {
			
			var filteredUserAnswers = _userAnswerRepository.GetAll()
						.Include( e => e.UserFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Question.Contains(input.Filter) || e.Answer.Contains(input.Filter) || e.UserId == Convert.ToInt32(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserFk != null && e.UserFk.Name == input.UserNameFilter);

			var pagedAndFilteredUserAnswers = filteredUserAnswers
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var userAnswers = from o in pagedAndFilteredUserAnswers
                         join o1 in _lookup_userRepository.GetAll() on o.UserId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         select new GetUserAnswerForViewDto() {
							UserAnswer = new UserAnswerDto
							{
                                Question = o.Question,
                                Answer = o.Answer,
                                Id = o.Id
							},
                         	UserName = s1 == null ? "" : s1.Name.ToString()
						};

            var totalCount = await filteredUserAnswers.CountAsync();

            return new PagedResultDto<GetUserAnswerForViewDto>(
                totalCount,
                await userAnswers.ToListAsync()
            );
         }
		 
		 public async Task<GetUserAnswerForViewDto> GetUserAnswerForView(int id)
         {
            var userAnswer = await _userAnswerRepository.GetAsync(id);

            var output = new GetUserAnswerForViewDto { UserAnswer = ObjectMapper.Map<UserAnswerDto>(userAnswer) };

		    if (output.UserAnswer.UserId != null)
            {
                var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.UserAnswer.UserId);
                output.UserName = _lookupUser.Name.ToString();
            }
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_UserAnswers_Edit)]
		 public async Task<GetUserAnswerForEditOutput> GetUserAnswerForEdit(EntityDto input)
         {
            var userAnswer = await _userAnswerRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetUserAnswerForEditOutput {UserAnswer = ObjectMapper.Map<CreateOrEditUserAnswerDto>(userAnswer)};

		    if (output.UserAnswer.UserId != null)
            {
                var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.UserAnswer.UserId);
                output.UserName = _lookupUser.Name.ToString();
            }
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditUserAnswerDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		
		 protected virtual async Task Create(CreateOrEditUserAnswerDto input)
         {
            var userAnswer = ObjectMapper.Map<UserAnswer>(input);

			

            await _userAnswerRepository.InsertAsync(userAnswer);
         }

	
		 protected virtual async Task Update(CreateOrEditUserAnswerDto input)
         {
            var userAnswer = await _userAnswerRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, userAnswer);
         }

		 [AbpAuthorize(AppPermissions.Pages_UserAnswers_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _userAnswerRepository.DeleteAsync(input.Id);
         } 

		[AbpAuthorize(AppPermissions.Pages_UserAnswers)]
         public async Task<PagedResultDto<UserAnswerUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_userRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Name != null && e.Name.Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var userList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<UserAnswerUserLookupTableDto>();
			foreach(var user in userList){
				lookupTableDtoList.Add(new UserAnswerUserLookupTableDto
				{
					Id = user.Id,
					DisplayName = user.Name?.ToString()
				});
			}

            return new PagedResultDto<UserAnswerUserLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
        public  async Task<CreateOrEditUserAnswerDto> CreateAll(List<CreateOrEditUserAnswerDto> data)
        {
            foreach (var input in data) {


                UserAnswer obj = new UserAnswer();
                obj.Answer = input.Answer;
                obj.Question = input.Question;
                obj.UserId = input.UserId;




            await _userAnswerRepository.InsertAsync(obj);
            }
            return null;
        }

    }
}