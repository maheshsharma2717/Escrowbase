using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using SR.EscrowBaseWeb.Configuration.Dto;

namespace SR.EscrowBaseWeb.Configuration
{
    public interface IUiCustomizationSettingsAppService : IApplicationService
    {
        Task<List<ThemeSettingsDto>> GetUiManagementSettings();

        Task UpdateUiManagementSettings(ThemeSettingsDto settings);

        Task UpdateDefaultUiManagementSettings(ThemeSettingsDto settings);

        Task UseSystemDefaultSettings();
    }
}
