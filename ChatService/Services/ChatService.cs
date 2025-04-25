using ChatService.Interfaces;
using DB.DAO;
using DB.DTO.Chat;

namespace ChatService.Services
{
    public class ChatService: IChatService
    {
        private ConversationDAO _conversationDAO = new ConversationDAO();

        public ChatService()
        {

        }

        public async Task<string> StartConversation(StartConversationRequestDTO startConversationRequest)
        {
            return await _conversationDAO.CreateConversation(startConversationRequest);
        }
    }
}
