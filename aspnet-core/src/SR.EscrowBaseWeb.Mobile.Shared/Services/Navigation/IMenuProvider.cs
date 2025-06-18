using System.Collections.Generic;
using MvvmHelpers;
using SR.EscrowBaseWeb.Models.NavigationMenu;

namespace SR.EscrowBaseWeb.Services.Navigation
{
    public interface IMenuProvider
    {
        ObservableRangeCollection<NavigationMenuItem> GetAuthorizedMenuItems(Dictionary<string, string> grantedPermissions);
    }
}