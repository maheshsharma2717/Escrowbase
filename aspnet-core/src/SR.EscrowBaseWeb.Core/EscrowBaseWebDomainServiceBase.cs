using Abp.Domain.Services;

namespace SR.EscrowBaseWeb
{
    public abstract class EscrowBaseWebDomainServiceBase : DomainService
    {
        /* Add your common members for all your domain services. */

        protected EscrowBaseWebDomainServiceBase()
        {
            LocalizationSourceName = EscrowBaseWebConsts.LocalizationSourceName;
        }
    }
}
