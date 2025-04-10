using DB.DAO.Interfaces;
using DB.Entities;
using DB.DTO;
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

        public bool SaveRefreshToken(string UserID, string refreshToken)
        {
            UserToken userToken = new UserToken { UserID = UserID, RefreshToken = refreshToken, DateCreated = DateTime.Now };
            UserToken existedUser = _appDbContext.UserToken.FirstOrDefault(u => u.UserID == UserID);
            if (existedUser == null)
                _appDbContext.UserToken.Add(userToken);
            else
            {
                existedUser.RefreshToken = refreshToken;
                existedUser.DateCreated = DateTime.Now;
                _appDbContext.UserToken.Update(existedUser);
            }
            return (_appDbContext.SaveChanges()) > 0;
        }

        public UserToken GetRefreshToken(string UserID)
        {
            return _appDbContext.UserToken.FirstOrDefault(u => u.UserID == UserID);
        }
    }
}
