using System.Threading.Tasks;
using Abp.Application.Services;
using SR.EscrowBaseWeb.Configuration.Tenants.Dto;

namespace SR.EscrowBaseWeb.Configuration.Tenants
{
    public interface ITenantSettingsAppService : IApplicationService
    {
        Task<TenantSettingsEditDto> GetAllSettings();

        Task UpdateAllSettings(TenantSettingsEditDto input);

        Task ClearLogo();

        Task ClearCustomCss();
    }
}
