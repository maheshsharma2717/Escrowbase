using Abp.AspNetCore.Mvc.Views;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc.Razor.Internal;

namespace SR.EscrowBaseWeb.Web.Public.Views
{
    public abstract class EscrowBaseWebRazorPage<TModel> : AbpRazorPage<TModel>
    {
        [RazorInject]
        public IAbpSession AbpSession { get; set; }

        protected EscrowBaseWebRazorPage()
        {
            LocalizationSourceName = EscrowBaseWebConsts.LocalizationSourceName;
        }
    }
}
