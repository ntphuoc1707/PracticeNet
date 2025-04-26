using ChatService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChatService.Hubs
{
    [Authorize]
    public class ChatHub : Hub, IChatHub
    {
        private IChatService _chatService;

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task SendPrivateMessage(string senderId, string receiverId, string message)
        {
            //await Clients.User(receiverId.ToString()).SendAsync("ReceivePrivateMessage", senderId, message);
        }

        public async Task SendGroupMessage(string groupId, string message)
        {
            await Clients.Group(groupId.ToString()).SendAsync("ReceiveGroupMessage", Context.ConnectionId, message);
            _chatService.SaveMessage(groupId, Context.UserIdentifier, message, DateTime.Now);
        }

        public async Task JoinGroup(string groupId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
        }

        public async Task LeaveGroup(string groupId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId);
        }


        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            var connections = await _chatService.GetListConversationsByUserId(userId);
            foreach (var gr in connections)
            {
                JoinGroup(gr);
            }

        }
    }
}
