using DB;
using DB.DAO;
using DB.Entities;
using DB.Model;
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

        public int AddUser(UserCreateModel userCreateModel)
        {
            if (_userDAO.FindUserByUsername(userCreateModel.UserName) != null)
            {
                return -1;
            }

            User user = new User()
            {
                UserID = Guid.NewGuid().ToString(),
                UserName = userCreateModel.UserName,
                Password = Common.HashData(userCreateModel.Password)
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
    }
}
