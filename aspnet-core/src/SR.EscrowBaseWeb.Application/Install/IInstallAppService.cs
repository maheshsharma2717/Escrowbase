using System.Threading.Tasks;
using Abp.Application.Services;
using SR.EscrowBaseWeb.Install.Dto;

namespace SR.EscrowBaseWeb.Install
{
    public interface IInstallAppService : IApplicationService
    {
        Task Setup(InstallDto input);

        AppSettingsJsonDto GetAppSettingsJson();

        CheckDatabaseOutput CheckDatabase();
    }
}