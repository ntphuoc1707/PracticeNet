using Microsoft.AspNetCore.Mvc;

namespace AuthenticationService.Controllers
{
    [ApiController]
    [Route("~/authen/[controller]")]
    public class AuthenticationController : Controller
    {
        [HttpGet(Name = "Authen")]
        public String Authenticate()
        {
            return "authen";
        }
    }
}
