using DB;
using DB.DAO;
using DB.DTO;
using Grpc.Net.Client;
using MessageQueue;
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
        private UserService.Services.UserService _userService;

        private readonly IRabbitMQPublisher<object> userServicePublisher;

        public UserController(IRabbitMQPublisher<object> userServicePublisher)
        {
            this.userServicePublisher = userServicePublisher;
            _userService=new Services.UserService(userServicePublisher);
        }

        [HttpPost(Name = "AddUser")]
        public ActionResult AddUser(UserLoginDTO userCreateModel)
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

        [HttpGet("{id}",Name ="GetUsers")]
        public ActionResult GetUser([FromRoute]int id)
        {
            var result = _userService.FindUserById(id);         
            if (result == null)
            {
                return StatusCode(500, "User is not existed");
            }
            else return StatusCode(200, result);
        }
    }
}
