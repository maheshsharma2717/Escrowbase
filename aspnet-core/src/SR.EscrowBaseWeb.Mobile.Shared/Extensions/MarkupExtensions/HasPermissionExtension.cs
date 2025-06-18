using System;
using SR.EscrowBaseWeb.Core;
using SR.EscrowBaseWeb.Core.Dependency;
using SR.EscrowBaseWeb.Services.Permission;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SR.EscrowBaseWeb.Extensions.MarkupExtensions
{
    [ContentProperty("Text")]
    public class HasPermissionExtension : IMarkupExtension
    {
        public string Text { get; set; }
        
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (ApplicationBootstrapper.AbpBootstrapper == null || Text == null)
            {
                return false;
            }

            var permissionService = DependencyResolver.Resolve<IPermissionService>();
            return permissionService.HasPermission(Text);
        }
    }
}