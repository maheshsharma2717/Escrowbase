using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.SRFileMapping.Dtos;
using SR.EscrowBaseWeb.Dto;
using System.Collections.Generic;

namespace SR.EscrowBaseWeb.SRFileMapping
{
    public interface ISrFileMappingsAppService : IApplicationService 
    {
        List<GetSrFileMappingForViewDto> GetAll(GetAllSrFileMappingsInput input);

        Task<GetSrFileMappingForViewDto> GetSrFileMappingForView(int id);

		Task<GetSrFileMappingForEditOutput> GetSrFileMappingForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditSrFileMappingDto input);

		Task Delete(EntityDto input);

		
    }
}