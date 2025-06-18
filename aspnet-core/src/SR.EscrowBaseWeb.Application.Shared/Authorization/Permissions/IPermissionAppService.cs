using Abp.Application.Services;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.Authorization.Permissions.Dto;

namespace SR.EscrowBaseWeb.Authorization.Permissions
{
    public interface IPermissionAppService : IApplicationService
    {
        ListResultDto<FlatPermissionWithLevelDto> GetAllPermissions();
    }
}
