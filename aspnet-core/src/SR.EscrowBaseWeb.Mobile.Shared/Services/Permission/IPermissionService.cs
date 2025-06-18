namespace SR.EscrowBaseWeb.Services.Permission
{
    public interface IPermissionService
    {
        bool HasPermission(string key);
    }
}