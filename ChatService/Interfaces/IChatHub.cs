using MongoDbProvider.Entities;

namespace ChatService.Interfaces
{
    public interface IChatHub
    {
        public Task SendPrivateMessage(string senderId, string receiverId, string message);
        public Task SendGroupMessage(string groupId, string message);
        public Task JoinGroup(string groupId);
        public Task LeaveGroup(string groupId);

    }
}
