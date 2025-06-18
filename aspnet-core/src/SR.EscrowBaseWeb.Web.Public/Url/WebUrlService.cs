using Abp.Dependency;
using SR.EscrowBaseWeb.Configuration;
using SR.EscrowBaseWeb.Url;
using SR.EscrowBaseWeb.Web.Url;

namespace SR.EscrowBaseWeb.Web.Public.Url
{
    public class WebUrlService : WebUrlServiceBase, IWebUrlService, ITransientDependency
    {
        public WebUrlService(
            IAppConfigurationAccessor appConfigurationAccessor) :
            base(appConfigurationAccessor)
        {
        }

        public override string WebSiteRootAddressFormatKey => "App:WebSiteRootAddress";

        public override string ServerRootAddressFormatKey => "App:AdminWebSiteRootAddress";
    }
}