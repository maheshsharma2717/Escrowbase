using Abp.Authorization;
using SR.EscrowBaseWeb.Authorization.Roles;
using SR.EscrowBaseWeb.Authorization.Users;

namespace SR.EscrowBaseWeb.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {

        }
    }
}
