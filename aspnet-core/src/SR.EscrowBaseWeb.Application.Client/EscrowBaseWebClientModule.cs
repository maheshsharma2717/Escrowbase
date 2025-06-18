using Abp.Modules;
using Abp.Reflection.Extensions;

namespace SR.EscrowBaseWeb
{
    public class EscrowBaseWebClientModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EscrowBaseWebClientModule).GetAssembly());
        }
    }
}
