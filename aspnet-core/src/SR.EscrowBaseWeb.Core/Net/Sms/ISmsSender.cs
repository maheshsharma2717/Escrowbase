using System.Threading.Tasks;

namespace SR.EscrowBaseWeb.Net.Sms
{
    public interface ISmsSender
    {
        Task SendAsync(string number, string message);
    }
}