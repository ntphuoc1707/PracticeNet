using ChatService.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChatService.Hubs
{
    public class ChatHub: Hub, IChatHub
    {
        public async Task SendPrivateMessage(Guid senderId, Guid receiverId, string message)
        {
            await Clients.User(receiverId.ToString()).SendAsync("ReceivePrivateMessage", senderId, message);
        }

        public async Task SendGroupMessage(Guid groupId, Guid senderId, string message)
        {
            await Clients.Group(groupId.ToString()).SendAsync("ReceiveGroupMessage", senderId, message);
        }

        public async Task JoinGroup(Guid groupId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToString());
        }

        public async Task LeaveGroup(Guid groupId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId.ToString());
        }

        public override async Task OnConnectedAsync()
        {
            //await Clients.All.SendAsync()
        }
    }
}
