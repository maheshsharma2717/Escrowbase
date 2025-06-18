using Abp.Application.Services;
using SR.EscrowBaseWeb.Dto;
using SR.EscrowBaseWeb.Logging.Dto;

namespace SR.EscrowBaseWeb.Logging
{
    public interface IWebLogAppService : IApplicationService
    {
        GetLatestWebLogsOutput GetLatestWebLogs();

        FileDto DownloadWebLogs();
    }
}
