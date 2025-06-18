using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.SREnterprise.Dtos;
using SR.EscrowBaseWeb.Dto;


namespace SR.EscrowBaseWeb.SREnterprise
{
    public interface IEnterprisesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetEnterpriseForViewDto>> GetAll(GetAllEnterprisesInput input);

        Task<GetEnterpriseForViewDto> GetEnterpriseForView(int id);

		Task<GetEnterpriseForEditOutput> GetEnterpriseForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditEnterpriseDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetEnterprisesToExcel(GetAllEnterprisesForExcelInput input);

		
    }
}