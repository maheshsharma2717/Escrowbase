using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SR.EscrowBaseWeb.Configuration;
using SR.EscrowBaseWeb.Web;

namespace SR.EscrowBaseWeb.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class EscrowBaseWebDbContextFactory : IDesignTimeDbContextFactory<EscrowBaseWebDbContext>
    {
        public EscrowBaseWebDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<EscrowBaseWebDbContext>();
            var configuration = AppConfigurations.Get(
                WebContentDirectoryFinder.CalculateContentRootFolder(),
                addUserSecrets: true
            );

            EscrowBaseWebDbContextConfigurer.Configure(builder, configuration.GetConnectionString(EscrowBaseWebConsts.ConnectionStringName));

            return new EscrowBaseWebDbContext(builder.Options);
        }
    }
}