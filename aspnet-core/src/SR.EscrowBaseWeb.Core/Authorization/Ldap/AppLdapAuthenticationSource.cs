using Abp.Zero.Ldap.Authentication;
using Abp.Zero.Ldap.Configuration;
using SR.EscrowBaseWeb.Authorization.Users;
using SR.EscrowBaseWeb.MultiTenancy;

namespace SR.EscrowBaseWeb.Authorization.Ldap
{
    public class AppLdapAuthenticationSource : LdapAuthenticationSource<Tenant, User>
    {
        public AppLdapAuthenticationSource(ILdapSettings settings, IAbpZeroLdapModuleConfig ldapModuleConfig)
            : base(settings, ldapModuleConfig)
        {
        }
    }
}