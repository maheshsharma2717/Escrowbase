using System.Threading.Tasks;
using SR.EscrowBaseWeb.Sessions.Dto;

namespace SR.EscrowBaseWeb.Web.Session
{
    public interface IPerRequestSessionCache
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformationsAsync();
    }
}
