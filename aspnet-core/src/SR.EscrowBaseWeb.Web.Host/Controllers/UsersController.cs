using Abp.AspNetCore.Mvc.Authorization;
using SR.EscrowBaseWeb.Authorization;
using SR.EscrowBaseWeb.Storage;
using Abp.BackgroundJobs;

namespace SR.EscrowBaseWeb.Web.Controllers
{
    [AbpMvcAuthorize(AppPermissions.Pages_Administration_Users)]
    public class UsersController : UsersControllerBase
    {
        public UsersController(IBinaryObjectManager binaryObjectManager, IBackgroundJobManager backgroundJobManager)
            : base(binaryObjectManager, backgroundJobManager)
        {
        }
    }
}