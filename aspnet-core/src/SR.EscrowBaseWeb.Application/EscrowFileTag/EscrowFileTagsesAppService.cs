using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using SR.EscrowBaseWeb.EscrowFileTag.Dtos;
using SR.EscrowBaseWeb.Dto;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using SR.EscrowBaseWeb.Storage;

namespace SR.EscrowBaseWeb.EscrowFileTag
{
   
    public class EscrowFileTagsesAppService : EscrowBaseWebAppServiceBase, IEscrowFileTagsesAppService
    {
        private readonly IRepository<EscrowFileTags> _escrowFileTagsRepository;

        public EscrowFileTagsesAppService(IRepository<EscrowFileTags> escrowFileTagsRepository)
        {
            _escrowFileTagsRepository = escrowFileTagsRepository;

        }

        public async Task<PagedResultDto<GetEscrowFileTagsForViewDto>> GetAll(GetAllEscrowFileTagsesInput input)
        {

            var filteredEscrowFileTagses = _escrowFileTagsRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.TagDescription.Contains(input.Filter) || e.TagColor.Contains(input.Filter) || e.EscrowNumber.Contains(input.Filter) || e.FileName.Contains(input.Filter));

            var pagedAndFilteredEscrowFileTagses = filteredEscrowFileTagses
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var escrowFileTagses = from o in pagedAndFilteredEscrowFileTagses
                                   select new
                                   {

                                       o.TagDescription,
                                       o.TagColor,
                                       o.EscrowNumber,
                                       o.CreatedAt,
                                       o.FileName,
                                       Id = o.Id
                                   };

            var totalCount = await filteredEscrowFileTagses.CountAsync();

            var dbList = await escrowFileTagses.ToListAsync();
            var results = new List<GetEscrowFileTagsForViewDto>();

            foreach (var o in dbList)
            {
                var res = new GetEscrowFileTagsForViewDto()
                {
                    EscrowFileTags = new EscrowFileTagsDto
                    {

                        TagDescription = o.TagDescription,
                        TagColor = o.TagColor,
                        EscrowNumber = o.EscrowNumber,
                       // CreatedAt = o.CreatedAt,
                        FileName = o.FileName,
                        Id = o.Id,
                    }
                };

                results.Add(res);
            }

            return new PagedResultDto<GetEscrowFileTagsForViewDto>(
                totalCount,
                results
            );

        }

        public async Task<GetEscrowFileTagsForViewDto> GetEscrowFileTagsForView(int id)
        {
            var escrowFileTags = await _escrowFileTagsRepository.GetAsync(id);

            var output = new GetEscrowFileTagsForViewDto { EscrowFileTags = ObjectMapper.Map<EscrowFileTagsDto>(escrowFileTags) };

            return output;
        }

       
        public async Task<GetEscrowFileTagsForEditOutput> GetEscrowFileTagsForEdit(EntityDto input)
        {
            var escrowFileTags = await _escrowFileTagsRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetEscrowFileTagsForEditOutput { EscrowFileTags = ObjectMapper.Map<CreateOrEditEscrowFileTagsDto>(escrowFileTags) };

            return output;
        }

        public async Task<responseBack> CreateOrEdit(CreateOrEditEscrowFileTagsDto input)
        {
            if (input.Id == null)
            {
                return await Create(input);
            }
            else
            {
                return await Update(input);
            }
        }

        protected virtual async Task<responseBack> Create(CreateOrEditEscrowFileTagsDto input)
        {
            var escrowFileTags = ObjectMapper.Map<EscrowFileTags>(input);

            if (AbpSession.TenantId != null)
            {
                escrowFileTags.TenantId = (int?)AbpSession.TenantId;
            }

            var normalizedDescription = escrowFileTags.TagDescription?.Trim().ToLower();

            var existingTag = await _escrowFileTagsRepository.FirstOrDefaultAsync(t =>
                t.TagDescription.ToLower().Trim() == normalizedDescription
            );

            if (existingTag != null)
            {
                return new responseBack
                {
                    Success = false,
                    Message = "A tag with the same description already exists."
                };
            }

            await _escrowFileTagsRepository.InsertAsync(escrowFileTags);

            return new responseBack
            {
                Success = true,
                Message = "Tag created successfully."
            };
        }

        protected virtual async Task<responseBack> Update(CreateOrEditEscrowFileTagsDto input)
        {
            var escrowFileTags = await _escrowFileTagsRepository.FirstOrDefaultAsync((int)input.Id);

            if (escrowFileTags == null)
            {
                return new responseBack
                {
                    Success = false,
                    Message = "Tag not found."
                };
            }

            ObjectMapper.Map(input, escrowFileTags);

            await _escrowFileTagsRepository.UpdateAsync(escrowFileTags);

            return new responseBack
            {
                Success = true,
                Message = "Tag updated successfully."
            };
        }


        public async Task Delete(EntityDto input)
        {
            await _escrowFileTagsRepository.DeleteAsync(input.Id);
        }

    }

    public interface IEscrowFileTagsesAppService
    {
        Task<responseBack> CreateOrEdit(CreateOrEditEscrowFileTagsDto input);
       
    }

    public class responseBack
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

}