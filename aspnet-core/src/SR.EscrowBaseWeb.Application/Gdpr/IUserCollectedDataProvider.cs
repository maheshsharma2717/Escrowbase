using System.Collections.Generic;
using System.Threading.Tasks;
using Abp;
using SR.EscrowBaseWeb.Dto;

namespace SR.EscrowBaseWeb.Gdpr
{
    public interface IUserCollectedDataProvider
    {
        Task<List<FileDto>> GetFiles(UserIdentifier user);
    }
}
