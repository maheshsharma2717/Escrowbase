using Microsoft.AspNetCore.Antiforgery;

namespace SR.EscrowBaseWeb.Web.Controllers
{
    public class AntiForgeryController : EscrowBaseWebControllerBase
    {
        private readonly IAntiforgery _antiforgery;

        public AntiForgeryController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        public void GetToken()
        {
            _antiforgery.SetCookieTokenAndHeader(HttpContext);
        }
    }
}
