using ChatService.Interfaces;
using DB.DAO;
using DB.DTO.Chat;
using MongoDB.Driver;
using MongoDbProvider;
using MongoDbProvider.Entities;

namespace ChatService.Services
{
    public class ChatService: IChatService
    {
        private ConversationDAO _conversationDAO = new ConversationDAO();
        private readonly IMongoCollection<ChatMessage> _chatMessages;
        public ChatService(MongoDbService mongoDbService)
        {
            _chatMessages = mongoDbService.Database?.GetCollection<ChatMessage>("ChatMessage");
        }

        public async Task<List<string>> GetListConversationsByUserId(string UserId)
        {
            return await _conversationDAO.GetListConversationsByUserId(UserId);
        }

        public async Task<string> StartConversation(StartConversationRequestDTO startConversationRequest)
        {
            return await _conversationDAO.CreateConversation(startConversationRequest);
        }

        public async Task SaveMessage(string groupId, string senderId, string content, DateTime dateCreated)
        {
            var message = new ChatMessage
            {
                GroupId = groupId,
                SenderId = senderId,
                Content = content,
                DateCreated = dateCreated
            };
            _chatMessages.InsertOneAsync(message);
        }

        public async Task<List<ChatMessage>> GetHistoryAsync(string groupId, DateTime? before = null, int pageSize = 20)
        {
            var filter = Builders<ChatMessage>.Filter.Eq(m => m.GroupId, groupId);

            if (before.HasValue)
            {
                filter = Builders<ChatMessage>.Filter.And(
                    filter,
                    Builders<ChatMessage>.Filter.Lt(m => m.DateCreated, before.Value)
                );
            }

            return await _chatMessages
                .Find(filter)
                .SortByDescending(m => m.DateCreated)
                .Limit(pageSize)
                .ToListAsync();
        }

    }
}
