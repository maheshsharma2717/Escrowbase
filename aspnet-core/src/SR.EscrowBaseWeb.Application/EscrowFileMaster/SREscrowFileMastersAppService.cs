using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using SR.EscrowBaseWeb.EscrowFileMaster.Dtos;
using SR.EscrowBaseWeb.Dto;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using SR.EscrowBaseWeb.Storage;
using SR.EscrowBaseWeb.SrAssignedFilesDetails;
using SR.EscrowBaseWeb.EscrowDetails;
using SR.EscrowBaseWeb.Authorization.Users;
using SR.EscrowBaseWeb.SRFileMapping;


namespace SR.EscrowBaseWeb.EscrowFileMaster
{
    // [AbpAuthorize(AppPermissions.Pages_SREscrowFileMasters)]
    public class SREscrowFileMastersAppService : EscrowBaseWebAppServiceBase, ISREscrowFileMastersAppService    {
        private readonly IRepository<SREscrowFileMaster, long> _srEscrowFileMasterRepository;
        private readonly IRepository<SrAssignedFilesDetail, long> _srAssignedFilesDetailRepository;
        private readonly IRepository<EscrowDetail, long> _escrowDetailRepository;
        private readonly IRepository<User, long> _userRepository;
        public SREscrowFileMastersAppService(IRepository<SREscrowFileMaster, long> srEscrowFileMasterRepository,        
            IRepository<SrAssignedFilesDetail, long> srAssignedFilesDetailRepository,
             IRepository<EscrowDetail, long> escrowDetailRepository,
             IRepository<User, long> userRepository
            )
        {
            _srEscrowFileMasterRepository = srEscrowFileMasterRepository;
           
            _srAssignedFilesDetailRepository = srAssignedFilesDetailRepository;
            _escrowDetailRepository = escrowDetailRepository;
            _userRepository = userRepository;
        }

        public async Task<PagedResultDto<GetSREscrowFileMasterForViewDto>> GetAll(GetAllSREscrowFileMastersInput input)
        {
            var filteredSREscrowFileMasters = _srEscrowFileMasterRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.FileFullName.Contains(input.Filter) || e.FileShortName.Contains(input.Filter));

            //     var filteredSREscrowFileMasters = _srEscrowFileMasterRepository.GetAll()
            //.Where(x => x.UserId == currentUserId)
            //.WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
            //    e => e.FileFullName.Contains(input.Filter) || e.FileShortName.Contains(input.Filter));

            var pagedAndFilteredSREscrowFileMasters = filteredSREscrowFileMasters
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var srEscrowFileMasters = from o in pagedAndFilteredSREscrowFileMasters
                                      select new
                                      {
                                          Id = o.Id,
                                          OtherAction = o.OtherAction,
                                          OtherActionNote = o.OtherActionNote
                                      };

            var totalCount = await filteredSREscrowFileMasters.CountAsync();

            var dbList = await srEscrowFileMasters.ToListAsync();
            var results = new List<GetSREscrowFileMasterForViewDto>();

            foreach (var o in dbList)
            {
                var res = new GetSREscrowFileMasterForViewDto()
                {
                    SREscrowFileMaster = new SREscrowFileMasterDto
                    {
                        Id = o.Id,
                        OtherAction = o.OtherAction,
                        OtherActionNote = o.OtherActionNote
                    }
                };

                results.Add(res);
            }

            return new PagedResultDto<GetSREscrowFileMasterForViewDto>(
                totalCount,
                results
            );

        }

        //public async Task<PagedResultDto<GetSREscrowFileMasterForViewDto>> GetAll(GetAllSREscrowFileMastersInput input)
        //{
        //    var currentUserId = AbpSession.UserId;

        //    if (currentUserId == null)
        //    {
        //        throw new UserFriendlyException("User not logged in.");
        //    }

        //    // Get file IDs assigned to this user and active
        //    var assignedFileIds = await _srFileMappingRepository.GetAll()
        //        .Where(m => m.UserId == currentUserId && m.IsActive && m.SrEscrowFileMasterId != null)
        //        .Select(m => m.SrEscrowFileMasterId.Value)
        //        .ToListAsync();

        //    // Filter escrow files by assigned IDs
        //    var filteredFiles = _srEscrowFileMasterRepository.GetAll()
        //        .Where(f => assignedFileIds.Contains(f.Id))
        //        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
        //            f => f.FileFullName.Contains(input.Filter) || f.FileShortName.Contains(input.Filter));

        //    var totalCount = await filteredFiles.CountAsync();

        //    var pagedFiles = await filteredFiles
        //        .OrderBy(input.Sorting ?? "id asc")
        //        .PageBy(input)
        //        .ToListAsync();

        //    var result = pagedFiles.Select(file => new GetSREscrowFileMasterForViewDto
        //    {
        //        SREscrowFileMaster = new SREscrowFileMasterDto
        //        {
        //            Id = file.Id,
        //            OtherAction = file.OtherAction,
        //            OtherActionNote = file.OtherActionNote
        //        }
        //    }).ToList();

        //    return new PagedResultDto<GetSREscrowFileMasterForViewDto>(totalCount, result);
        //}


        public async Task<GetSREscrowFileMasterForViewDto> GetSREscrowFileMasterForView(long id)
        {
            var srEscrowFileMaster = await _srEscrowFileMasterRepository.GetAsync(id);

            var output = new GetSREscrowFileMasterForViewDto { SREscrowFileMaster = ObjectMapper.Map<SREscrowFileMasterDto>(srEscrowFileMaster) };

            return output;
        }

        public async Task<GetSREscrowFileMasterForEditOutput> GetSREscrowFileMasterForEdit(EntityDto<long> input)
        {
            var srEscrowFileMaster = await _srEscrowFileMasterRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetSREscrowFileMasterForEditOutput { SREscrowFileMaster = ObjectMapper.Map<CreateOrEditSREscrowFileMasterDto>(srEscrowFileMaster) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditSREscrowFileMasterDto input)
        {
            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        protected virtual async Task Create(CreateOrEditSREscrowFileMasterDto input)
        {
            var srEscrowFileMaster = ObjectMapper.Map<SREscrowFileMaster>(input);
            await _srEscrowFileMasterRepository.InsertAsync(srEscrowFileMaster);

        }

        protected virtual async Task Update(CreateOrEditSREscrowFileMasterDto input)
        {
            var srEscrowFileMaster = await _srEscrowFileMasterRepository.FirstOrDefaultAsync((long)input.Id);
            ObjectMapper.Map(input, srEscrowFileMaster);

        }

   
        public async Task Delete(EntityDto<long> input)
        {
            await _srEscrowFileMasterRepository.DeleteAsync(input.Id);
        }

        public async Task<List<AssignedFileUser>> GetFileUserList(long srEscrowFileMasterId, string userType, string escrowNumber)
        {
            try
            {
                var dbAssignedUser = await _srAssignedFilesDetailRepository.GetAll()
                    .Where(x => x.SrEscrowFileMasterId == srEscrowFileMasterId)
                    .Select(x => x.UserId)
                    .ToListAsync();
                var userIdSet = new HashSet<long>(dbAssignedUser);

                var dbEscrowUserList = await (from escrow in _escrowDetailRepository.GetAll()


                                              join user in _userRepository.GetAll()
                                              on escrow.UserId equals user.Id


                                              where escrow.EscrowId == escrowNumber
                                                  && userIdSet.Contains(escrow.UserId.Value)
                                                  && (userType == "EOX" ? escrow.Usertype != "EOX" : escrow.Usertype == "EOX")
                                              select new AssignedFileUser
                                              {
                                                  Email = user.EmailAddress,
                                                  UserType = escrow.Usertype,
                                                  SrEscrowFileMasterId = srEscrowFileMasterId,
                                                  IsChecked = userType == "EOX" ? false : true,
                                                  FullName = user.FullName,
                                                  PhoneNo = !string.IsNullOrWhiteSpace(user.PhoneNumber) ? user.PhoneNumber : "Not Registered",
                                                  UserId = escrow.UserId
                                              }).ToListAsync();
                return dbEscrowUserList;
            }
            catch (Exception ex)
            {
                // Log the exception
                return new List<AssignedFileUser>();
            }
        }


    }

}