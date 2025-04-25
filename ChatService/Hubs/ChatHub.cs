using ChatService.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChatService.Hubs
{
    public class ChatHub: Hub, IChatHub
    {
        public async Task SendPrivateMessage(string senderId, string receiverId, string message)
        {
            await Clients.User(receiverId.ToString()).SendAsync("ReceivePrivateMessage", senderId, message);
        }

        public async Task SendGroupMessage(string groupId, string senderId, string message)
        {
            await Clients.Group(groupId.ToString()).SendAsync("ReceiveGroupMessage", senderId, message);
        }

        public async Task JoinGroup( string groupId, string userId)
        {
            await Groups.AddToGroupAsync(userId, groupId.ToString());
        }

        public async Task LeaveGroup(string groupId, string userId)
        {
            await Groups.RemoveFromGroupAsync(userId, groupId.ToString());
        }

        public override async Task OnConnectedAsync()
        {
            //await Clients.All.SendAsync()
        }
    }
}
