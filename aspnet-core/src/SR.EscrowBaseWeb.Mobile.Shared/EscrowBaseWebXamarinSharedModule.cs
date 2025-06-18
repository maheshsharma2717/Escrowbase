using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace SR.EscrowBaseWeb
{
    [DependsOn(typeof(EscrowBaseWebClientModule), typeof(AbpAutoMapperModule))]
    public class EscrowBaseWebXamarinSharedModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Localization.IsEnabled = false;
            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EscrowBaseWebXamarinSharedModule).GetAssembly());
        }
    }
}