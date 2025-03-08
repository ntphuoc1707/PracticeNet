using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UserService.Controllers
{
    [Route("~/user/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        [HttpGet(Name = "User")]
        public String Authenticate()
        {
            return "user";
        }
    }
}
