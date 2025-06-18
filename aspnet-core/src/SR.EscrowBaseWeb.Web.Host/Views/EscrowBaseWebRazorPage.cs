using Abp.AspNetCore.Mvc.Views;

namespace SR.EscrowBaseWeb.Web.Views
{
    public abstract class EscrowBaseWebRazorPage<TModel> : AbpRazorPage<TModel>
    {
        protected EscrowBaseWebRazorPage()
        {
            LocalizationSourceName = EscrowBaseWebConsts.LocalizationSourceName;
        }
    }
}
