using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace SR.EscrowBaseWeb.EntityFrameworkCore
{
    public static class EscrowBaseWebDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<EscrowBaseWebDbContext> builder, string connectionString)
        {
            builder.UseMySql(connectionString, MySqlServerVersion.LatestSupportedServerVersion);
        }

        public static void Configure(DbContextOptionsBuilder<EscrowBaseWebDbContext> builder, DbConnection connection)
        {
            builder.UseMySql(connection, MySqlServerVersion.LatestSupportedServerVersion);
        }
    }
}