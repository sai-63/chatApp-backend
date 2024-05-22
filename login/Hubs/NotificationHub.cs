using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using login.Common.Models;

namespace login.Hubs
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            string userId = Context.GetHttpContext().Request.Query["userId"];
            string groupName = GetGroupName(userId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await base.OnConnectedAsync();
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendToUser(string senderId, string receiverId, Chat chat)
        {
            string groupName = GetGroupName(receiverId);
            string mygroupName = GetGroupName(senderId);
            await Clients.Group(groupName).SendAsync("ReceiveMessage", senderId, chat);
            await Clients.Group(mygroupName).SendAsync("ReceiveMessage", senderId, chat);
        }

        public async Task RemoveMessage(string receiverId,string messageId,string chatDate)
        {
            // Perform deletion logic here, e.g., remove message from data store

            // Broadcast message removal to all clients
            string userId = Context.GetHttpContext().Request.Query["userId"];
            string groupName = GetGroupName(receiverId);
            string mygroupName = GetGroupName(userId);
            await Clients.Group(groupName).SendAsync("MessageRemoved", messageId,chatDate);
            await Clients.Group(mygroupName).SendAsync("MessageRemoved", messageId,chatDate);
        }

        public async Task EditMessage(string receiverId, string messageId, string newMessage, string chatDate)
        {
            // Perform deletion logic here, e.g., remove message from data store

            // Broadcast message removal to all clients
            string userId = Context.GetHttpContext().Request.Query["userId"];
            string groupName = GetGroupName(receiverId);
            string mygroupName = GetGroupName(userId);
            await Clients.Group(groupName).SendAsync("MessageEdited", messageId, newMessage, chatDate);
            await Clients.Group(mygroupName).SendAsync("MessageEdited", messageId, newMessage, chatDate);
        }

        public async Task MarkAsRead(string receiverId,List<string> messageIds)
        {
            string groupName = GetGroupName(receiverId);
            await Clients.Group(groupName).SendAsync("MessageRead", messageIds);
        }

        private string GetGroupName(string userId)
        {
            return $"User_{userId}";
        }

    }
}
