using System.Collections.Generic;
using SR.EscrowBaseWeb.Authorization.Permissions.Dto;

namespace SR.EscrowBaseWeb.Authorization.Users.Dto
{
    public class GetUserPermissionsForEditOutput
    {
        public List<FlatPermissionDto> Permissions { get; set; }

        public List<string> GrantedPermissionNames { get; set; }
    }
}