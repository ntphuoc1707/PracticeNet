using DB.DAO;
using DB.Entities;
using DB.DTO;
using Grpc.Net.Client;
using GrpcProvider.Protos;
using MessageQueue;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Utility;
using static GrpcProvider.Protos.GrpcProvider;
using Common;

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

        private readonly GrpcProviderClient _grpcClient;


        private readonly ILogger<CustomException> _logger;
        private readonly IRabbitMQPublisher<object> authenticationServicePublisher;
        private readonly IConfiguration _configuration;

        public AuthenticationController(IRabbitMQPublisher<object> authenticationServicePublisher, GrpcProviderClient grpcClient, IConfiguration configuration)
        {
            this.authenticationServicePublisher = authenticationServicePublisher;
            _grpcClient = grpcClient;
            _configuration = configuration;
            //_userService = new Services.UserService(userServicePublisher);
        }


        [HttpPost(Name = "Login")]
        public async Task<IActionResult> Login(UserLoginDTO userCreateModel)
        {
            userCreateModel.Password = Utility.Common.HashData(userCreateModel.Password);
            //var result=await authenticationServicePublisher.PublishMessageAsyncWithQueue(userCreateModel, RabbitMQQueues.UserServiceQueue, "FindUserByUserCreateModel");

            var result = await _grpcClient.HandleMessageAsync(new GrpcProvider.Protos.Request
            {
                FullClassName = "UserService.Services.UserService",
                Funct = "FindUserByUserCreateModel",
                Data = JsonConvert.SerializeObject(userCreateModel)
            });

            User user = JsonConvert.DeserializeObject<User>(result.Data);
            if (user == null)
            {
                return StatusCode(500, "Username or password is wrong");
            }
            else
            {
                UserRefreshTokenDTO userTokenModel = new UserRefreshTokenDTO { UserID = user.UserID, RefreshToken = GenerateRefreshToken() };
                var saveRefreshToken = await _grpcClient.HandleMessageAsync(new GrpcProvider.Protos.Request
                {
                    FullClassName = "UserService.Services.UserService",
                    Funct = "SaveRefreshToken",
                    Data = JsonConvert.SerializeObject(userTokenModel)
                });
                return Ok(new { Token = GenerateJwtToken(user.UserID), RefeshToken = userTokenModel.RefreshToken });
            }
        }

        [HttpPost(Name = "RefreshAccessToken")]
        public async Task<IActionResult> RefreshAccessToken(UserRefreshTokenDTO userToken)
        {
            var result = await _grpcClient.HandleMessageAsync(new GrpcProvider.Protos.Request
            {
                FullClassName = "UserService.Services.UserService",
                Funct = "GetRefreshToken",
                Data = JsonConvert.SerializeObject(userToken.UserID)
            });
            UserToken existedUserToken = JsonConvert.DeserializeObject<UserToken>(result.Data);
            if (existedUserToken != null)
            {
                int ExpiredRefreshToken = int.Parse(_configuration.GetSection("ExpiredRefreshToken").Value);
                if (existedUserToken.RefreshToken.Equals(userToken.RefreshToken) && existedUserToken.DateCreated.AddHours(ExpiredRefreshToken) < DateTime.Now)
                {
                    return Ok(new { Token = GenerateJwtToken(existedUserToken.UserID), RefeshToken = existedUserToken.RefreshToken });
                }
            }
            return StatusCode(500, "Refresh Token is invalid");
        }

        private string GenerateJwtToken(string userID)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userID)
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

        private string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
