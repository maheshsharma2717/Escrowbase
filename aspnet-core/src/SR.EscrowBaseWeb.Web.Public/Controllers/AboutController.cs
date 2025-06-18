using Microsoft.AspNetCore.Mvc;
using SR.EscrowBaseWeb.Web.Controllers;

namespace SR.EscrowBaseWeb.Web.Public.Controllers
{
    public class AboutController : EscrowBaseWebControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}