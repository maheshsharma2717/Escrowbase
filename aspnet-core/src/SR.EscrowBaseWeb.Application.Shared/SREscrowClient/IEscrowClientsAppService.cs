using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.SREscrowClient.Dtos;
using SR.EscrowBaseWeb.Dto;


namespace SR.EscrowBaseWeb.SREscrowClient
{
    public interface IEscrowClientsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetEscrowClientForViewDto>> GetAll(GetAllEscrowClientsInput input);

        Task<GetEscrowClientForViewDto> GetEscrowClientForView(int id);

		Task<GetEscrowClientForEditOutput> GetEscrowClientForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditEscrowClientDto input);

		Task Delete(EntityDto input);

		
    }
}