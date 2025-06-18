using Abp.Modules;
using Abp.Reflection.Extensions;

namespace SR.EscrowBaseWeb
{
    [DependsOn(typeof(EscrowBaseWebXamarinSharedModule))]
    public class EscrowBaseWebXamarinIosModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EscrowBaseWebXamarinIosModule).GetAssembly());
        }
    }
}