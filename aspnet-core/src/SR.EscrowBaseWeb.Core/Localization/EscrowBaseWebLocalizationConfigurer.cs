using System.Reflection;
using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Reflection.Extensions;

namespace SR.EscrowBaseWeb.Localization
{
    public static class EscrowBaseWebLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    EscrowBaseWebConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(EscrowBaseWebLocalizationConfigurer).GetAssembly(),
                        "SR.EscrowBaseWeb.Localization.EscrowBaseWeb"
                    )
                )
            );
        }
    }
}