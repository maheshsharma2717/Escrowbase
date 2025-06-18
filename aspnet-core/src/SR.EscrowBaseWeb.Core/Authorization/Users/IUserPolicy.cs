using System.Threading.Tasks;
using Abp.Domain.Policies;

namespace SR.EscrowBaseWeb.Authorization.Users
{
    public interface IUserPolicy : IPolicy
    {
        Task CheckMaxUserCountAsync(int tenantId);
    }
}
