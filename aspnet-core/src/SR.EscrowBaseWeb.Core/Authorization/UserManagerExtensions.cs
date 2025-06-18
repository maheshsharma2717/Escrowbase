using System.Threading.Tasks;
using Abp.Authorization.Users;
using SR.EscrowBaseWeb.Authorization.Users;

namespace SR.EscrowBaseWeb.Authorization
{
    public static class UserManagerExtensions
    {
        public static async Task<User> GetAdminAsync(this UserManager userManager)
        {
            return await userManager.FindByNameAsync(AbpUserBase.AdminUserName);
        }
    }
}
