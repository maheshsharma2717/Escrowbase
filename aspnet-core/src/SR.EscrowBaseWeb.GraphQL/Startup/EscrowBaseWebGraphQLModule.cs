using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace SR.EscrowBaseWeb.Startup
{
    [DependsOn(typeof(EscrowBaseWebCoreModule))]
    public class EscrowBaseWebGraphQLModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EscrowBaseWebGraphQLModule).GetAssembly());
        }

        public override void PreInitialize()
        {
            base.PreInitialize();

            //Adding custom AutoMapper configuration
            Configuration.Modules.AbpAutoMapper().Configurators.Add(CustomDtoMapper.CreateMappings);
        }
    }
}