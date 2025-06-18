using System.Threading.Tasks;

namespace SR.EscrowBaseWeb.Security
{
    public interface IPasswordComplexitySettingStore
    {
        Task<PasswordComplexitySetting> GetSettingsAsync();
    }
}
