using System.Globalization;

namespace SR.EscrowBaseWeb.Localization
{
    public interface IApplicationCulturesProvider
    {
        CultureInfo[] GetAllCultures();
    }
}