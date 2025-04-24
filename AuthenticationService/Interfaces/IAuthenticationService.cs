using DB.DTO;
using DB.Entities;

namespace AuthenticationService.Interfaces
{
    public interface IAuthenticationService
    {
        public Task<User> GetUser(UserLoginDTO userCreateModel);
        public Task<UserRefreshTokenDTO> CreateRefreshToken(string userID);
        public Task<UserToken> GetRefreshToken(string userID);
    }
}
