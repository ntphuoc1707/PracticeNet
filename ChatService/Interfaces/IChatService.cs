using DB.DTO.Chat;

namespace ChatService.Interfaces
{
    public interface IChatService
    {
        public Task<string> StartConversation(StartConversationRequestDTO startConversationRequest);

    }
}
