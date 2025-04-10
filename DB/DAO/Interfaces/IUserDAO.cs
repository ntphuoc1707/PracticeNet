using DB.Entities;
using DB.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.DAO.Interfaces
{
    public interface IUserDAO
    {
        public User AddUser(User user);
        public User FindUserById(int id);
        public User FindUserByUsername(string username);
        public User FindUserByUsernameAndPassword(string username, string password);
        public bool SaveRefreshToken(string UserID, string refreshToken);

        public UserToken GetRefreshToken(string UserID);
    }
}
