using System.Threading.Tasks;
using Abp.Application.Services;
using SR.EscrowBaseWeb.Editions.Dto;
using SR.EscrowBaseWeb.MultiTenancy.Dto;

namespace SR.EscrowBaseWeb.MultiTenancy
{
    public interface ITenantRegistrationAppService: IApplicationService
    {
        Task<RegisterTenantOutput> RegisterTenant(RegisterTenantInput input);

        Task<EditionsSelectOutput> GetEditionsForSelect();

        Task<EditionSelectDto> GetEdition(int editionId);
    }
}