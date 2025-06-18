using System.Threading.Tasks;
using SR.EscrowBaseWeb.Security.Recaptcha;

namespace SR.EscrowBaseWeb.Test.Base.Web
{
    public class FakeRecaptchaValidator : IRecaptchaValidator
    {
        public Task ValidateAsync(string captchaResponse)
        {
            return Task.CompletedTask;
        }
    }
}
