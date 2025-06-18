using Abp.Configuration;

namespace SR.EscrowBaseWeb.Timing.Dto
{
    public class GetTimezonesInput
    {
        public SettingScopes DefaultTimezoneScope { get; set; }
    }
}
