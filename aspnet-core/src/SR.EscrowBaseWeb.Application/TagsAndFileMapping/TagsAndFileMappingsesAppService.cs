using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using SR.EscrowBaseWeb.TagsAndFileMapping.Dtos;
using SR.EscrowBaseWeb.Dto;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using SR.EscrowBaseWeb.Storage;

namespace SR.EscrowBaseWeb.TagsAndFileMapping
{
    public class TagsAndFileMappingsesAppService : EscrowBaseWebAppServiceBase, ITagsAndFileMappingsesAppService
    {
        private readonly IRepository<TagsAndFileMappings> _tagsAndFileMappingsRepository;

        public TagsAndFileMappingsesAppService(IRepository<TagsAndFileMappings> tagsAndFileMappingsRepository)
        {
            _tagsAndFileMappingsRepository = tagsAndFileMappingsRepository;

        }

        public async Task<PagedResultDto<GetTagsAndFileMappingsForViewDto>> GetAll(GetAllTagsAndFileMappingsesInput input)
        {

            var filteredTagsAndFileMappingses = _tagsAndFileMappingsRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.FileName.Contains(input.Filter));

            var pagedAndFilteredTagsAndFileMappingses = filteredTagsAndFileMappingses
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var tagsAndFileMappingses = from o in pagedAndFilteredTagsAndFileMappingses
                                        select new
                                        {

                                            Id = o.Id
                                        };

            var totalCount = await filteredTagsAndFileMappingses.CountAsync();

            var dbList = await tagsAndFileMappingses.ToListAsync();
            var results = new List<GetTagsAndFileMappingsForViewDto>();

            foreach (var o in dbList)
            {
                var res = new GetTagsAndFileMappingsForViewDto()
                {
                    TagsAndFileMappings = new TagsAndFileMappingsDto
                    {

                        Id = o.Id,
                    }
                };

                results.Add(res);
            }

            return new PagedResultDto<GetTagsAndFileMappingsForViewDto>(
                totalCount,
                results
            );

        }

        public async Task<GetTagsAndFileMappingsForEditOutput> GetTagsAndFileMappingsForEdit(EntityDto input)
        {
            var tagsAndFileMappings = await _tagsAndFileMappingsRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetTagsAndFileMappingsForEditOutput { TagsAndFileMappings = ObjectMapper.Map<CreateOrEditTagsAndFileMappingsDto>(tagsAndFileMappings) };

            return output;
        }

        public async Task<responseBack2> CreateOrEdit(CreateOrEditTagsAndFileMappingsDto input)
        {
            if (input.Id == 0)
            {
                return await Create(input);
            }
            else
            {
                return await Update(input);
            }
        }



        protected virtual async Task<responseBack2> Create(CreateOrEditTagsAndFileMappingsDto input)
        {
            // Normalize the file name for comparison
            var normalizedFileName = input.FileName?.Trim().ToLower();

            // Check if the same TagId is already assigned to this file
            var existingMapping = await _tagsAndFileMappingsRepository.FirstOrDefaultAsync(
                x => x.TagId == input.TagId && x.FileName.ToLower().Trim() == normalizedFileName);

            if (existingMapping != null)
            {
                return new responseBack2
                {
                    Success = false,
                    Message = "This tag is already assigned to the file."
                };
            }

            var tagsAndFileMappings = ObjectMapper.Map<TagsAndFileMappings>(input);
            await _tagsAndFileMappingsRepository.InsertAsync(tagsAndFileMappings);

            return new responseBack2
            {
                Success = true,
                Message = "Tag successfully assigned to file."
            };
        }


        protected virtual async Task<responseBack2> Update(CreateOrEditTagsAndFileMappingsDto input)
        {
            var mapping = await _tagsAndFileMappingsRepository.GetAsync((int)input.Id);

            ObjectMapper.Map(input, mapping);
            return new responseBack2
            {
                Success = true,
                Message = "Tag updated successfully."
            };
        }


        public async Task Delete(EntityDto input)
        {
            await _tagsAndFileMappingsRepository.DeleteAsync(input.Id);
        }

        public async Task DeleteTagByFileNameAndTagId(DeleteTagsAndFileMappingsDto input)
        {
            var entity = await _tagsAndFileMappingsRepository.FirstOrDefaultAsync(x => x.TagId == input.TagId && x.FileName == input.FileName);

            if (entity != null)
            {
                await _tagsAndFileMappingsRepository.DeleteAsync(entity.Id);
            }
            else
            {
                throw new Exception("Tag with specified Id and FileName not found.");
            }
        }
       
    }
    public interface ITagsAndFileMappingsesAppService
    {
        Task<responseBack2> CreateOrEdit(CreateOrEditTagsAndFileMappingsDto input);

    }

    public class responseBack2
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}