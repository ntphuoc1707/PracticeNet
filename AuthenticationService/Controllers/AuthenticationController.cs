using DB.DAO;
using DB.Entities;
using DB.Model;
using MessageQueue;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Utility;

namespace AuthenticationService.Controllers
{
    [ApiController]
    [Route("~/authen/[action]")]
    public class AuthenticationController : Controller
    {
        private const string SecretKey = "your-secret-key-that-is-long-enough";
        private const string Issuer = "phuoc123";
        private const string Audience = "phuoc123";
        //private UserService.Services.UserService _userService = new UserService.Services.UserService();
        private readonly IRabbitMQPublisher<object> authenticationServicePublisher;

        public AuthenticationController(IRabbitMQPublisher<object> authenticationServicePublisher)
        {
            this.authenticationServicePublisher = authenticationServicePublisher;
            //_userService = new Services.UserService(userServicePublisher);
        }


        [HttpPost(Name ="Login")]
        public async Task<IActionResult> Login(UserCreateModel userCreateModel)
        {
            userCreateModel.Password = Common.HashData(userCreateModel.Password);
            var result=await authenticationServicePublisher.PublishMessageAsyncWithQueue(userCreateModel, RabbitMQQueues.UserServiceQueue, "FindUserByUserCreateModel");
            User user = JsonConvert.DeserializeObject<User>(result);
            if (user == null)
            {
                return StatusCode(500, "Username or password is wrong");
            }
            else
            {
                return Ok(new { Token = GenerateJwtToken(user.UserID) });
            }
        }

        private string GenerateJwtToken(string username)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                Issuer,
                Audience,
                claims,
                expires: DateTime.Now.AddHours(100),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
