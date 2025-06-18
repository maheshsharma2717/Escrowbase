using System.Threading.Tasks;

namespace SR.EscrowBaseWeb.Security.Recaptcha
{
    public interface IRecaptchaValidator
    {
        Task ValidateAsync(string captchaResponse);
    }
}