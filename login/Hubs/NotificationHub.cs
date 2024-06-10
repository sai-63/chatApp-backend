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

        public async Task UserClosingTab()
        {
            var connectionId = Context.ConnectionId;

            // Perform necessary actions, e.g., update user status, log the event, etc.
            await HandleUserDisconnectAsync(connectionId);
        }

        private Task HandleUserDisconnectAsync(string connectionId)
        {
            // Your logic to handle the user disconnection
            // For example, updating the user's status in the database
            // or notifying other users about this disconnection
            return Task.CompletedTask;
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Handle the disconnect event when it occurs
            await HandleUserDisconnectAsync(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendToUser(string senderId, string receiverId, string senderName, Chat chat)
        {
            string groupName = GetGroupName(receiverId);
            string mygroupName = GetGroupName(senderId);
            await Clients.Group(groupName).SendAsync("ReceiveMessage", chat, senderName);
            await Clients.Group(mygroupName).SendAsync("ReceiveMessage", chat, senderName);
        }

        public async Task RemoveMessage(string receiverId,string messageId,string chatDate,string senderName)
        {
            // Perform deletion logic here, e.g., remove message from data store

            // Broadcast message removal to all clients
            string userId = Context.GetHttpContext().Request.Query["userId"];
            string groupName = GetGroupName(receiverId);
            string mygroupName = GetGroupName(userId);
            await Clients.Group(groupName).SendAsync("MessageRemoved", messageId, chatDate, senderName);
            await Clients.Group(mygroupName).SendAsync("MessageRemoved", messageId, chatDate, senderName);
        }

        public async Task EditMessage(string receiverId, string messageId, string newMessage, string chatDate, string senderName)
        {
            // Perform deletion logic here, e.g., remove message from data store

            // Broadcast message removal to all clients
            string userId = Context.GetHttpContext().Request.Query["userId"];
            string groupName = GetGroupName(receiverId);
            string mygroupName = GetGroupName(userId);
            await Clients.Group(groupName).SendAsync("MessageEdited", messageId, newMessage, chatDate, senderName);
            await Clients.Group(mygroupName).SendAsync("MessageEdited", messageId, newMessage, chatDate, senderName);
        }   

        public async Task MarkAsRead(string receiverId,List<string> messageIds, string senderName)
        {
            string groupName = GetGroupName(receiverId);
            await Clients.Group(groupName).SendAsync("MessageRead", messageIds, senderName);
        }

        public async Task UserOnline(string username)
        {
            await Clients.All.SendAsync("UserOnline", username);
        }

        public async Task UserOffline(string username,DateTime time)
        {
            await Clients.All.SendAsync("UserOffline", username, time);
        }

        public async Task UserTyping(string receiverId,string username, string status)
        {
            string groupName = GetGroupName(receiverId);
            await Clients.Group(groupName).SendAsync("UserTyping", username, status);
        }

        public async Task IncrementUnseenMessages(string receiverId,string username,string seen = null)
        {
            string groupName = GetGroupName(receiverId);
            await Clients.Group(groupName).SendAsync("IncrementUnseenMessages",username,seen);
        }

        public async Task SortChats(string receiverId,string username,string timestamp)
        {
            string groupName = GetGroupName(receiverId);
            await Clients.Group(groupName).SendAsync("SortChats",username,timestamp);
        }

        private string GetGroupName(string userId)
        {
            return $"User_{userId}";
        }

    }
}
