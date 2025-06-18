using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.SrUserType.Dtos;
using SR.EscrowBaseWeb.Dto;


namespace SR.EscrowBaseWeb.SrUserType
{
    public interface IUserTypesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetUserTypeForViewDto>> GetAll(GetAllUserTypesInput input);

		Task<GetUserTypeForEditOutput> GetUserTypeForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditUserTypeDto input);

		Task Delete(EntityDto input);

		
    }
}