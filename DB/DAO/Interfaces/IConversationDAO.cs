using DB.DTO.Chat;
using DB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.DAO.Interfaces
{
    public interface IConversationDAO
    {
        public Task<string> CreateConversation(StartConversationRequestDTO startConversationRequest);
        public Task<List<string>> GetListConversationsByUserId(string Userid);

        public Task<List<Conversation>> GetConversationByGroupId(string GroupdId);

        public Task<string> GetPrivateConversation(string userId1, string userId2);
        //public Task<string> CreateOrGetConversation(StartConversationRequestDTO startConversationRequest);
        //public Task<bool> CheckExistedConversation(string SenderId, string ReceiverId);

        //public Task<List<string>> GetConversation(string SenderId, string ReceiverId, string type);

        //public Task<Conversation> GetConversation(string GroupId, string SenderId);
    }
}
