using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Security
{
    public static class JwtAuthentication
    {
        private static byte[] SecretKey = Encoding.ASCII.GetBytes("your-secret-key-that-is-long-enough");
        private static JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(SecretKey),
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Cookies["accessToken"];
                        if (!string.IsNullOrEmpty(token))
                        {
                            var jwtToken = handler.ReadJwtToken(token);
                            var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
                            if (expClaim == null)
                                return Task.CompletedTask;

                            var expUnix = long.Parse(expClaim);
                            var expDateTime = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
                            if (DateTime.UtcNow > expDateTime)
                            {
                                context.Response.StatusCode = 401;
                                context.Response.ContentType = "application/json";
                                var result = JsonSerializer.Serialize(new { message = "Token has expired" });
                                return context.Response.WriteAsync(result);
                            }
                            context.Token = token;
                        }
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";
                            var result = JsonSerializer.Serialize(new { message = "Token has expired" });
                            return context.Response.WriteAsync(result);
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }
        public static string GenerateJwtToken(string userID, int ExpiredToken = 3600, string Issuer = "",string Audience="")
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userID)
            };

            var key = new SymmetricSecurityKey(SecretKey);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                Issuer,
                Audience,
                claims,
                expires: DateTime.Now.AddSeconds(ExpiredToken),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString("N");
        }

        public static string GetClaims(string token,string key = "")
        {
            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken.Claims.FirstOrDefault(c => c.Type == key)?.Value; ;
        }
    }
}
