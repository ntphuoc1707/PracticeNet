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
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using Security;

namespace AuthenticationService.Controllers
{
    [ApiController]
    [Route("~/authen/[action]")]
    public class AuthenticationController : Controller
    {

        private readonly GrpcProviderClient _grpcClient;


        private readonly ILogger<CustomException> _logger;
        private readonly IRabbitMQPublisher<object> authenticationServicePublisher;
        private readonly IConfiguration _configuration;
        int ExpiredCookie = 1;
        int ExpiredToken = 10;//seconds
        public AuthenticationController(IRabbitMQPublisher<object> authenticationServicePublisher, GrpcProviderClient grpcClient, IConfiguration configuration)
        {
            this.authenticationServicePublisher = authenticationServicePublisher;
            _grpcClient = grpcClient;
            _configuration = configuration;
            ExpiredCookie= int.Parse(_configuration.GetSection("ExpiredCookie").Value);
            ExpiredToken = int.Parse(_configuration.GetSection("ExpiredToken").Value);
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
                return StatusCode(401, "Username or password is wrong");
            }
            else
            {
                UserRefreshTokenDTO userTokenModel = new UserRefreshTokenDTO { UserID = user.UserID, RefreshToken = JwtAuthentication.GenerateRefreshToken() };
                var saveRefreshToken = await _grpcClient.HandleMessageAsync(new GrpcProvider.Protos.Request
                {
                    FullClassName = "UserService.Services.UserService",
                    Funct = "SaveRefreshToken",
                    Data = JsonConvert.SerializeObject(userTokenModel)
                });

                Response.Cookies.Append("accessToken", JwtAuthentication.GenerateJwtToken(user.UserID), new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTimeOffset.UtcNow.AddHours(ExpiredCookie)
                }); 
                Response.Cookies.Append("refreshToken", userTokenModel.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTimeOffset.UtcNow.AddHours(ExpiredCookie)
                });
                return Ok(new { Message="Successfully" });
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
                if (existedUserToken.RefreshToken.Equals(userToken.RefreshToken) && existedUserToken.DateCreated.AddHours(ExpiredRefreshToken) > DateTime.Now)
                {
                    Response.Cookies.Append("accessToken", JwtAuthentication.GenerateJwtToken(existedUserToken.UserID), new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        Expires = DateTimeOffset.UtcNow.AddHours(ExpiredCookie)
                    });
                    Response.Cookies.Append("refreshToken", existedUserToken.RefreshToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        Expires = DateTimeOffset.UtcNow.AddHours(ExpiredCookie)
                    });
                    return Ok(new { Message="Successfully" });
                }
            }
            return StatusCode(401, "Refresh Token is invalid");
        }

        [HttpPost(Name = "RefreshAccessTokenCookie")]
        public async Task<IActionResult> RefreshAccessTokenCookie()
        {
            string refreshToken=Request.Cookies["refreshToken"];
            string accessToken = Request.Cookies["accessToken"];

            var userId = JwtAuthentication.GetClaims(accessToken, "sub");

            UserRefreshTokenDTO userToken = new UserRefreshTokenDTO { RefreshToken = refreshToken, UserID = userId };
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
                if (existedUserToken.RefreshToken.Equals(userToken.RefreshToken) && existedUserToken.DateCreated.AddHours(ExpiredRefreshToken) > DateTime.Now)
                {
                    Response.Cookies.Append("accessToken", JwtAuthentication.GenerateJwtToken(existedUserToken.UserID), new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        Expires = DateTimeOffset.UtcNow.AddHours(ExpiredCookie)
                    });
                    Response.Cookies.Append("refreshToken", existedUserToken.RefreshToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        Expires = DateTimeOffset.UtcNow.AddHours(ExpiredCookie)
                    });
                    return Ok(new { Message = "Successfully" });
                } else return StatusCode(401, new { Message = "Refresh Token is expired" });
            }
            return StatusCode(401, new { Message = "Refresh Token is invalid" });
        }


    }
}
