using Abp.Modules;
using Abp.Reflection.Extensions;

namespace SR.EscrowBaseWeb
{
    public class EscrowBaseWebCoreSharedModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EscrowBaseWebCoreSharedModule).GetAssembly());
        }
    }
}