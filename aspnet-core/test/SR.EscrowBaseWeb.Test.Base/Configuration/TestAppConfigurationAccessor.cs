using Abp.Dependency;
using Abp.Reflection.Extensions;
using Microsoft.Extensions.Configuration;
using SR.EscrowBaseWeb.Configuration;

namespace SR.EscrowBaseWeb.Test.Base.Configuration
{
    public class TestAppConfigurationAccessor : IAppConfigurationAccessor, ISingletonDependency
    {
        public IConfigurationRoot Configuration { get; }

        public TestAppConfigurationAccessor()
        {
            Configuration = AppConfigurations.Get(
                typeof(EscrowBaseWebTestBaseModule).GetAssembly().GetDirectoryPathOrNull()
            );
        }
    }
}
