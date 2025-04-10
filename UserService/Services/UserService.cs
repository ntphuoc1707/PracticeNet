using DB;
using DB.DAO;
using DB.Entities;
using DB.DTO;
using MessageQueue;
using Utility;

namespace UserService.Services
{
    public class UserService
    {
        private UserDAO _userDAO = new UserDAO();

        private readonly IRabbitMQPublisher<object> _rabbitMqPublisher;

        public UserService(IRabbitMQPublisher<object> rabbitMqPublisher)
        {
            _rabbitMqPublisher = rabbitMqPublisher;
        }

        public UserService()
        {
        }

        public int AddUser(UserLoginDTO userCreateModel)
        {
            if (_userDAO.FindUserByUsername(userCreateModel.UserName) != null)
            {
                return -1;
            }

            User user = new User()
            {
                UserID = Guid.NewGuid().ToString(),
                UserName = userCreateModel.UserName,
                Password = Utility.Common.HashData(userCreateModel.Password)
            };
            _userDAO.AddUser(user);
            
            return user.Id;
        }

        public User FindUserById(int id)
        {
            return _userDAO.FindUserById(id);
        }

        public User FindUserByUsername(string username)
        {
            return _userDAO.FindUserByUsername(username);
        }

        public User FindUserByUsernameAndPassword(string username, string password)
        {
            return _userDAO.FindUserByUsernameAndPassword(username,password);
        }
        public User FindUserByUserCreateModel(UserLoginDTO user)
        {
            return _userDAO.FindUserByUsernameAndPassword(user.UserName, user.Password);
        }

        public bool SaveRefreshToken(UserRefreshTokenDTO userTokenModel)
        {
            return _userDAO.SaveRefreshToken(userTokenModel.UserID, userTokenModel.RefreshToken);
        }

        public UserToken GetRefreshToken(string userID)
        {
            return _userDAO.GetRefreshToken(userID);
        }
    }
}
