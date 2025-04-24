using AuthenticationService.Interfaces;
using Common;
using DB.DTO;
using DB.Entities;
using Microsoft.AspNetCore.Mvc;
using Security;

namespace AuthenticationService.Controllers
{
    [ApiController]
    [Route("~/authen/[action]")]
    public class AuthenticationController : Controller
    {
        private IAuthenticationService _authenService;
        private readonly ILogger<CustomException> _logger;
        private readonly IConfiguration _configuration;
        int ExpiredCookie = 1;
        public AuthenticationController(IConfiguration configuration, IAuthenticationService authenticationService)
        {
            this._authenService = authenticationService;
            _configuration = configuration;
            ExpiredCookie = int.Parse(_configuration.GetSection("ExpiredCookie").Value);
        }


        [HttpPost(Name = "Login")]
        public async Task<IActionResult> Login(UserLoginDTO userCreateModel)
        {
            User user = await _authenService.GetUser(userCreateModel);
            if (user == null)
            {
                return StatusCode(401, "Username or password is wrong");
            }
            else
            {
                UserRefreshTokenDTO userTokenModel = await _authenService.CreateRefreshToken(user.UserID);
                string accessToken = JwtAuthentication.GenerateJwtToken(user.UserID);

                Response.Cookies.Append("accessToken", accessToken, new CookieOptions
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
                return Ok(new { Message = "Successfully", Token= accessToken ,RefreshToken= userTokenModel.RefreshToken });
            }
        }

        [HttpPost(Name = "RefreshAccessToken")]
        public async Task<IActionResult> RefreshAccessToken(UserRefreshTokenDTO userToken)
        {

            UserToken existedUserToken = await _authenService.GetRefreshToken(userToken.UserID);
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
                }
            }
            return StatusCode(401, "Refresh Token is invalid");
        }

        [HttpPost(Name = "RefreshAccessTokenCookie")]
        public async Task<IActionResult> RefreshAccessTokenCookie()
        {
            string refreshToken = Request.Cookies["refreshToken"];
            string accessToken = Request.Cookies["accessToken"];

            var userId = JwtAuthentication.GetClaims(accessToken, "sub");

            UserToken existedUserToken = await _authenService.GetRefreshToken(userId);
            if (existedUserToken != null)
            {
                int ExpiredRefreshToken = int.Parse(_configuration.GetSection("ExpiredRefreshToken").Value);
                if (existedUserToken.RefreshToken.Equals(refreshToken) && existedUserToken.DateCreated.AddHours(ExpiredRefreshToken) > DateTime.Now)
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
                }
                else return StatusCode(401, new { Message = "Refresh Token is expired" });
            }
            return StatusCode(401, new { Message = "Refresh Token is invalid" });
        }


    }
}
