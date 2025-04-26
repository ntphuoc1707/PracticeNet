using DB.DTO.Chat;
using MongoDbProvider.Entities;

namespace ChatService.Interfaces
{
    public interface IChatService
    {
        public Task<string> StartConversation(StartConversationRequestDTO startConversationRequest);

        public Task<List<string>> GetListConversationsByUserId(string UserId);
        public Task SaveMessage(string groupId, string senderId, string content, DateTime dateCreated);

        public Task<List<ChatMessage>> GetHistoryAsync(string groupId, DateTime? before = null, int pageSize = 50);
    }
}
