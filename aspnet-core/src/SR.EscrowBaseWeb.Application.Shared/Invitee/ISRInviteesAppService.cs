using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.Invitee.Dtos;
using SR.EscrowBaseWeb.Dto;


namespace SR.EscrowBaseWeb.Invitee
{
    public interface ISRInviteesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetSRInviteeForViewDto>> GetAll(GetAllSRInviteesInput input);

		Task<GetSRInviteeForEditOutput> GetSRInviteeForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditSRInviteeDto input);

		Task Delete(EntityDto input);

		
		Task<PagedResultDto<SRInviteeUserTypeLookupTableDto>> GetAllUserTypeForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<SRInviteeEscrowClientLookupTableDto>> GetAllEscrowClientForLookupTable(GetAllForLookupTableInput input);
		
    }
}