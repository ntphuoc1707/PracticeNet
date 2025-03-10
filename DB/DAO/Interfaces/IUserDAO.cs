using DB.Entities;
using DB.Model;
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
    }
}
