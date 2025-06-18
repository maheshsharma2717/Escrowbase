namespace SR.EscrowBaseWeb.Configuration
{
    public interface IAppConfigurationWriter
    {
        void Write(string key, string value);
    }
}
