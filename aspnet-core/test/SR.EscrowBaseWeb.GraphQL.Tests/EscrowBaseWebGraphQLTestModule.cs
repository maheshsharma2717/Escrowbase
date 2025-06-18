using Abp.Modules;
using Abp.Reflection.Extensions;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using SR.EscrowBaseWeb.Configure;
using SR.EscrowBaseWeb.Startup;
using SR.EscrowBaseWeb.Test.Base;

namespace SR.EscrowBaseWeb.GraphQL.Tests
{
    [DependsOn(
        typeof(EscrowBaseWebGraphQLModule),
        typeof(EscrowBaseWebTestBaseModule))]
    public class EscrowBaseWebGraphQLTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            IServiceCollection services = new ServiceCollection();
            
            services.AddAndConfigureGraphQL();

            WindsorRegistrationHelper.CreateServiceProvider(IocManager.IocContainer, services);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EscrowBaseWebGraphQLTestModule).GetAssembly());
        }
    }
}