using DB.DAO.Interfaces;
using DB.Entities;
using DB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DB.DAO
{
    public class UserDAO : IUserDAO
    {
        private AppDbContext _appDbContext ;

        public UserDAO(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public UserDAO()
        {
            _appDbContext = new AppDbContext();
        }

        public User AddUser(User user)
        {
            _appDbContext.User.Add(user);
            _appDbContext.SaveChanges();

            return user;
        }

        public User FindUserById(int id)
        {
            return _appDbContext.User.Find(id);
        }

        public User FindUserByUsername(string username)
        {
            return _appDbContext.User.FirstOrDefault(p => p.UserName.ToLower()==username.ToLower());
        }

        public User FindUserByUsernameAndPassword(string username, string password)
        {
            return _appDbContext.User.FirstOrDefault(p => p.UserName.ToLower() == username.ToLower() && p.Password==password);
        }
    }
}
