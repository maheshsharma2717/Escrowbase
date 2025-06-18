using System.Threading.Tasks;
using Abp.Application.Services;
using SR.EscrowBaseWeb.Sessions.Dto;

namespace SR.EscrowBaseWeb.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();

        Task<UpdateUserSignInTokenOutput> UpdateUserSignInToken();
    }
}
