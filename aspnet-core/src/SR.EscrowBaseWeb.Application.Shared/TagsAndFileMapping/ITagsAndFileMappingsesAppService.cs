using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.TagsAndFileMapping.Dtos;
using SR.EscrowBaseWeb.Dto;

namespace SR.EscrowBaseWeb.TagsAndFileMapping
{
    public interface ITagsAndFileMappingsesAppService : IApplicationService
    {
        Task<PagedResultDto<GetTagsAndFileMappingsForViewDto>> GetAll(GetAllTagsAndFileMappingsesInput input);

        Task<GetTagsAndFileMappingsForEditOutput> GetTagsAndFileMappingsForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditTagsAndFileMappingsDto input);

        Task Delete(EntityDto input);

    }
}