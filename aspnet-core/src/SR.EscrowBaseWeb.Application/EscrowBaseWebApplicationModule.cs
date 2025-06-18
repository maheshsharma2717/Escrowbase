using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Timing;
using SR.EscrowBaseWeb.Authorization;

namespace SR.EscrowBaseWeb
{
    /// <summary>
    /// Application layer module of the application.
    /// </summary>
    [DependsOn(
        typeof(EscrowBaseWebApplicationSharedModule),
        typeof(EscrowBaseWebCoreModule)
        )]
    public class EscrowBaseWebApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            //Adding authorization providers
            Configuration.Authorization.Providers.Add<AppAuthorizationProvider>();

            //Adding custom AutoMapper configuration
            Configuration.Modules.AbpAutoMapper().Configurators.Add(CustomDtoMapper.CreateMappings);

            // Enable multiple time zone support
            Abp.Timing.Clock.Provider = ClockProviders.Utc;
             
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EscrowBaseWebApplicationModule).GetAssembly());
        }
    }
}