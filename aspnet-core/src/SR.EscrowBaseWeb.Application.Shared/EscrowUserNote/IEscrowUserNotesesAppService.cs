using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.EscrowUserNote.Dtos;
using SR.EscrowBaseWeb.Dto;

namespace SR.EscrowBaseWeb.EscrowUserNote
{
    public interface IEscrowUserNotesesAppService : IApplicationService
    {
        Task<PagedResultDto<GetEscrowUserNotesForViewDto>> GetAll(GetAllEscrowUserNotesesInput input);

        Task<GetEscrowUserNotesForViewDto> GetEscrowUserNotesForView(int id);

        Task<GetEscrowUserNotesForEditOutput> GetEscrowUserNotesForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditEscrowUserNotesDto input);

        Task Delete(EntityDto input);

        Task<PagedResultDto<EscrowUserNotesUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input);

    }
}