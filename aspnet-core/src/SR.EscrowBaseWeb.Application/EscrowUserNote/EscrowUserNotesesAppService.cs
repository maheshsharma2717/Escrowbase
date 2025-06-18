using SR.EscrowBaseWeb.Authorization.Users;

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using SR.EscrowBaseWeb.EscrowUserNote.Dtos;
using SR.EscrowBaseWeb.Dto;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using SR.EscrowBaseWeb.Storage;

namespace SR.EscrowBaseWeb.EscrowUserNote
{
    public class EscrowUserNotesesAppService : EscrowBaseWebAppServiceBase, IEscrowUserNotesesAppService
    {
        private readonly IRepository<EscrowUserNotes> _escrowUserNotesRepository;
        private readonly IRepository<User, long> _lookup_userRepository;

        public EscrowUserNotesesAppService(IRepository<EscrowUserNotes> escrowUserNotesRepository, IRepository<User, long> lookup_userRepository)
        {
            _escrowUserNotesRepository = escrowUserNotesRepository;
            _lookup_userRepository = lookup_userRepository;

        }

        public async Task<PagedResultDto<GetEscrowUserNotesForViewDto>> GetAll(GetAllEscrowUserNotesesInput input)
        {

            var filteredEscrowUserNoteses = _escrowUserNotesRepository.GetAll()
                        .Include(e => e.CreatedByFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Message.Contains(input.Filter) || e.EscrowNumber.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.CreatedByFk != null && e.CreatedByFk.Name == input.UserNameFilter);

            var pagedAndFilteredEscrowUserNoteses = filteredEscrowUserNoteses
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var escrowUserNoteses = from o in pagedAndFilteredEscrowUserNoteses
                                    join o1 in _lookup_userRepository.GetAll() on o.CreatedBy equals o1.Id into j1
                                    from s1 in j1.DefaultIfEmpty()

                                    select new
                                    {

                                        o.Message,
                                        o.EscrowNumber,
                                        o.CreatedAt,
                                        Id = o.Id,
                                        UserName = s1 == null || s1.Name == null ? "" : s1.Name.ToString()
                                    };

            var totalCount = await filteredEscrowUserNoteses.CountAsync();

            var dbList = await escrowUserNoteses.ToListAsync();
            var results = new List<GetEscrowUserNotesForViewDto>();

            foreach (var o in dbList)
            {
                var res = new GetEscrowUserNotesForViewDto()
                {
                    EscrowUserNotes = new EscrowUserNotesDto
                    {

                        Message = o.Message,
                        EscrowNumber = o.EscrowNumber,
                        CreatedAt = o.CreatedAt,
                        Id = o.Id,
                    },
                    UserName = o.UserName
                };

                results.Add(res);
            }

            return new PagedResultDto<GetEscrowUserNotesForViewDto>(
                totalCount,
                results
            );

        }

        public async Task<GetEscrowUserNotesForViewDto> GetEscrowUserNotesForView(int id)
        {
            var escrowUserNotes = await _escrowUserNotesRepository.GetAsync(id);

            var output = new GetEscrowUserNotesForViewDto { EscrowUserNotes = ObjectMapper.Map<EscrowUserNotesDto>(escrowUserNotes) };

            if (output.EscrowUserNotes.CreatedBy != null)
            {
                var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.EscrowUserNotes.CreatedBy);
                output.UserName = _lookupUser?.Name?.ToString();
            }

            return output;
        }

        public async Task<List<EscrowUserNotesresponseDto>> GetEscrowUserNotesByUserId(int id)
        {
            var escrowUserNotesWithUser = from note in _escrowUserNotesRepository.GetAll()
                                          join user in _lookup_userRepository.GetAll()
                                          on note.CreatedBy equals user.Id into userGroup
                                          from user in userGroup.DefaultIfEmpty()
                                          where note.CreatedBy == id
                                          select new EscrowUserNotesresponseDto
                                          {
                                              Id = note.Id,
                                              Message = note.Message,
                                              CreatedBy = note.CreatedBy,
                                              UserName = user.EmailAddress, 
                                              CreatedAt = note.CreatedAt
                                          };

            // Execute the query
            return await escrowUserNotesWithUser.ToListAsync();
        }

        public async Task<GetEscrowUserNotesForEditOutput> GetEscrowUserNotesForEdit(EntityDto input)
        {
            var escrowUserNotes = await _escrowUserNotesRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetEscrowUserNotesForEditOutput { EscrowUserNotes = ObjectMapper.Map<CreateOrEditEscrowUserNotesDto>(escrowUserNotes) };

            if (output.EscrowUserNotes.CreatedBy != null)
            {
                var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.EscrowUserNotes.CreatedBy);
                output.UserName = _lookupUser?.Name?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditEscrowUserNotesDto input)
        {
            if (input.Id == 0)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        protected virtual async Task Create(CreateOrEditEscrowUserNotesDto input)
        {
            try
            {
                var escrowUserNotes = ObjectMapper.Map<EscrowUserNotes>(input);
                escrowUserNotes.CreatedAt = DateTime.Now;

                if (AbpSession.TenantId != null)
                {
                    escrowUserNotes.TenantId = (int?)AbpSession.TenantId;
                }

                await _escrowUserNotesRepository.InsertAsync(escrowUserNotes);
            }
            catch (Exception ex)
            {
                Logger.Error("An error occurred while creating EscrowUserNotes", ex);
                throw;
            }
        }


        protected virtual async Task Update(CreateOrEditEscrowUserNotesDto input)
        {
            var escrowUserNotes = await _escrowUserNotesRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, escrowUserNotes);

        }

        public async Task Delete(EntityDto input)
        {
            await _escrowUserNotesRepository.DeleteAsync(input.Id);
        }

        public async Task<PagedResultDto<EscrowUserNotesUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_userRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Name != null && e.Name.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var userList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<EscrowUserNotesUserLookupTableDto>();
            foreach (var user in userList)
            {
                lookupTableDtoList.Add(new EscrowUserNotesUserLookupTableDto
                {
                    Id = user.Id,
                    DisplayName = user.Name?.ToString()
                });
            }

            return new PagedResultDto<EscrowUserNotesUserLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

    }
}