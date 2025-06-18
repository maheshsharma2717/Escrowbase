using Microsoft.Extensions.Configuration;

namespace SR.EscrowBaseWeb.Configuration
{
    public interface IAppConfigurationAccessor
    {
        IConfigurationRoot Configuration { get; }
    }
}
