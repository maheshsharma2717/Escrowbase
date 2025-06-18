using SR.EscrowBaseWeb.EntityFrameworkCore;

namespace SR.EscrowBaseWeb.Migrations.Seed.Host
{
    public class InitialHostDbBuilder
    {
        private readonly EscrowBaseWebDbContext _context;

        public InitialHostDbBuilder(EscrowBaseWebDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            new DefaultEditionCreator(_context).Create();
            new DefaultLanguagesCreator(_context).Create();
            new HostRoleAndUserCreator(_context).Create();
            new DefaultSettingsCreator(_context).Create();

            _context.SaveChanges();
        }
    }
}
