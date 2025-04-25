namespace ChatService.Interfaces
{
    public interface IChatHub
    {
        public Task SendPrivateMessage(string senderId, string receiverId, string message);
        public Task SendGroupMessage(string groupId, string senderId, string message);
        public Task JoinGroup(string groupId, string userId);
        public Task LeaveGroup(string groupId, string userId);

    }
}
