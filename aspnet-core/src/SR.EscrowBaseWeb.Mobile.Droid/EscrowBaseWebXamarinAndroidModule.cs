using Abp.Modules;
using Abp.Reflection.Extensions;

namespace SR.EscrowBaseWeb
{
    [DependsOn(typeof(EscrowBaseWebXamarinSharedModule))]
    public class EscrowBaseWebXamarinAndroidModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EscrowBaseWebXamarinAndroidModule).GetAssembly());
        }
    }
}