using DB;
using DB.DAO;
using DB.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UserService.Controllers
{
    [Authorize]
    [Route("~/user/[action]")]
    [ApiController]
    public class UserController : Controller
    {
        private UserService.Services.UserService _userService = new UserService.Services.UserService();


        [HttpPost(Name = "AddUser")]
        public ActionResult AddUser(UserCreateModel userCreateModel)
        {
            if (userCreateModel == null)
            {
                return BadRequest();
            }
            var result = _userService.AddUser(userCreateModel);
            if (result == -1)
            {
                return StatusCode(500, "Username is existed");
            }
            return Ok("Add user successfully");
        }
    }
}
