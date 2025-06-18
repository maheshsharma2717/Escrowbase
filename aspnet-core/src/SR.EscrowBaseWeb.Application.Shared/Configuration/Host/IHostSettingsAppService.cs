using System.Threading.Tasks;
using Abp.Application.Services;
using SR.EscrowBaseWeb.Configuration.Host.Dto;

namespace SR.EscrowBaseWeb.Configuration.Host
{
    public interface IHostSettingsAppService : IApplicationService
    {
        Task<HostSettingsEditDto> GetAllSettings();

        Task UpdateAllSettings(HostSettingsEditDto input);

        Task SendTestEmail(SendTestEmailInput input);
    }
}
