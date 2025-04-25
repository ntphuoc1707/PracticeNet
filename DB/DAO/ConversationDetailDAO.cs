using DB.DAO.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.DAO
{
    public class ConversationDetailDAO : IConversationDetailDAO
    {
        private AppDbContext _appDbContext;

        public ConversationDetailDAO(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public ConversationDetailDAO()
        {
            _appDbContext = new AppDbContext();
        }
        public async Task<string> CreateGroup(string type, int MemberNum = 2)
        {
            string GroupId = Guid.NewGuid().ToString();
            _appDbContext.ConversationDetail.Add(new Entities.ConversationDetail { GroupId = GroupId, Type = type, MemberNum = MemberNum, DateCreated=DateTime.Now });
            return GroupId;
        }
    }
}
