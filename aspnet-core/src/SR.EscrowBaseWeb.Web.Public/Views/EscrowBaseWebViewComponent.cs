using Abp.AspNetCore.Mvc.ViewComponents;

namespace SR.EscrowBaseWeb.Web.Public.Views
{
    public abstract class EscrowBaseWebViewComponent : AbpViewComponent
    {
        protected EscrowBaseWebViewComponent()
        {
            LocalizationSourceName = EscrowBaseWebConsts.LocalizationSourceName;
        }
    }
}