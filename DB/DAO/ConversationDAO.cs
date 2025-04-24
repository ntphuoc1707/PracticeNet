using DB.DAO.Interfaces;
using DB.DTO.Chat;
using DB.Entities;

namespace DB.DAO
{
    public class ConversationDAO : IConversationDAO
    {
        private AppDbContext _appDbContext;

        public ConversationDAO(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public ConversationDAO()
        {
            _appDbContext = new AppDbContext();
        }

        public async Task<string> CreateOrGetConversation(StartConversationRequestDTO startConversationRequest)
        {
            var result = await GetConversation(startConversationRequest.SenderId, startConversationRequest.ReceiverId, "P");
            string GroupId = Guid.NewGuid().ToString();
            if (result != null && result.Count > 0)
            {
                GroupId = result[0];
                return GroupId;
            }
            Conversation conversationSender = new Conversation
            {
                GroupId = GroupId,
                SenderId = startConversationRequest.SenderId,
                ReceiverId = startConversationRequest.ReceiverId,
                Type = "P"
            };
            Conversation conversationReceiver = new Conversation
            {
                GroupId = GroupId,
                SenderId = startConversationRequest.ReceiverId,
                ReceiverId = startConversationRequest.SenderId,
                Type = "P"
            };

            _appDbContext.Conversation.Add(conversationSender);
            _appDbContext.Conversation.Add(conversationReceiver);

            return GroupId;
        }

        public async Task<bool> CheckExistedConversation(string SenderId, string ReceiverId)
        {
            var result = await GetConversation(SenderId, ReceiverId, "P");
            return result != null && result.Count > 0;
        }
        public async Task<List<string>> GetConversation(string SenderId, string ReceiverId, string type)
        {
            return _appDbContext.Conversation.Where(c => c.Type == type && c.SenderId.Equals(SenderId) && c.ReceiverId.Equals(ReceiverId)).Select(c => c.GroupId).ToList();
        }

        public async Task<Conversation> GetConversation(string GroupId, string SenderId)
        {
            return _appDbContext.Conversation.FirstOrDefault(c => c.GroupId.Equals(GroupId) && c.SenderId.Equals(SenderId));
        }
    }
}
