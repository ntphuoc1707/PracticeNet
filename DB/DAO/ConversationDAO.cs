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

        //public async Task<string> CreateOrGetConversation(StartConversationRequestDTO startConversationRequest)
        //{
        //    var result = await GetConversation(startConversationRequest.SenderId, startConversationRequest.ReceiverId, "P");
        //    string GroupId = Guid.NewGuid().ToString();
        //    if (result != null && result.Count > 0)
        //    {
        //        GroupId = result[0];
        //        return GroupId;
        //    }
        //    Conversation conversationSender = new Conversation
        //    {
        //        GroupId = GroupId,
        //        SenderId = startConversationRequest.SenderId,
        //        ReceiverId = startConversationRequest.ReceiverId,
        //        Type = "P"
        //    };
        //    Conversation conversationReceiver = new Conversation
        //    {
        //        GroupId = GroupId,
        //        SenderId = startConversationRequest.ReceiverId,
        //        ReceiverId = startConversationRequest.SenderId,
        //        Type = "P"
        //    };

        //    _appDbContext.Conversation.Add(conversationSender);
        //    _appDbContext.Conversation.Add(conversationReceiver);

        //    return GroupId;
        //}

        //public async Task<bool> CheckExistedConversation(string SenderId, string ReceiverId)
        //{
        //    var result = await GetConversation(SenderId, ReceiverId, "P");
        //    return result != null && result.Count > 0;
        //}
        //public async Task<List<string>> GetConversation(string SenderId, string ReceiverId, string type)
        //{
        //    return _appDbContext.Conversation.Where(c => c.Type == type && c.SenderId.Equals(SenderId) && c.ReceiverId.Equals(ReceiverId)).Select(c => c.GroupId).ToList();
        //}

        //public async Task<Conversation> GetConversation(string GroupId, string SenderId)
        //{
        //    return _appDbContext.Conversation.FirstOrDefault(c => c.GroupId.Equals(GroupId) && c.SenderId.Equals(SenderId));
        //}

        public async Task<string> CreateConversation(StartConversationRequestDTO startConversationRequest)
        {
            var checkExist = await GetPrivateConversation(startConversationRequest.SenderId, startConversationRequest.ReceiverId);
            if (!string.IsNullOrEmpty(checkExist)) return checkExist;
            string GroupId = Guid.NewGuid().ToString();
            Conversation conversationSender = new Conversation
            {
                GroupId = GroupId,
                UserId = startConversationRequest.SenderId
            };
            Conversation conversationReceiver = new Conversation
            {
                GroupId = GroupId,
                UserId = startConversationRequest.ReceiverId
            };
            ConversationDetail conDetail = new ConversationDetail
            {
                GroupId = GroupId,
                MemberNum=2,
                Type="P"
            };
            _appDbContext.ConversationDetail.Add(conDetail);
            _appDbContext.Conversation.Add(conversationSender);
            _appDbContext.Conversation.Add(conversationReceiver);
            _appDbContext.SaveChanges();
            return GroupId;
        }

        public async Task<List<string>> GetListConversationsByUserId(string UserId)
        {
            return _appDbContext.Conversation.Where(c => c.UserId.Equals(UserId)).Select(c => c.GroupId).ToList();
        }

        public async Task<List<Conversation>> GetConversationByGroupId(string GroupdId)
        {
            return _appDbContext.Conversation.Where(c => c.GroupId.Equals(GroupdId)).ToList();
        }

        public async Task<string> GetPrivateConversation(string userId1, string userId2)
        {
            return _appDbContext.Conversation.Where(c => c.UserId.Equals(userId1) || c.UserId.Equals(userId2)).GroupBy(c => c.GroupId).Where(g => g.Select(c => c.UserId).Count() == 2).Select(c=>c.Key).FirstOrDefault();
        }
    }
}
