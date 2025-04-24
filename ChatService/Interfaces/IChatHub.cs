namespace ChatService.Interfaces
{
    public interface IChatHub
    {
        public Task SendPrivateMessage(Guid senderId, Guid receiverId, string message);
        public Task SendGroupMessage(Guid groupId, Guid senderId, string message);
        public Task JoinGroup(Guid groupId);
        public Task LeaveGroup(Guid groupId);

    }
}
