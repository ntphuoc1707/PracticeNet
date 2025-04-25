using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.DAO.Interfaces
{
    public interface IConversationDetailDAO
    {
        public Task<string> CreateGroup(string type, int MemberNum = 2);
    }
}
