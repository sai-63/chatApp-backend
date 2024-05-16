using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using login.Common.Models;
using Common.Models;

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

        public async Task SendToOwnGroup(string senderId,Chat chat)
        {
            string groupName = GetGroupName(senderId);
            await Clients.Group(groupName).SendAsync("ReceiveMessage", senderId, chat);
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

        private string GetGroupName(string userId)
        {
            return $"User_{userId}";
        }


        //Group SignalR
        public async Task SendToGroup(string groupName, string senderId, Grpmsg group)
        {
            string groupas = GetGroupName(senderId);
            string groupNameas = GetGroupName(groupName);
            await Clients.Group(groupas).SendAsync("ReceiveGrpMessage", senderId, group);
            await Clients.Group(groupNameas).SendAsync("ReceiveGrpMessage", senderId, group);
        }

        public async Task RemoveMessageFromGroup(string groupName, string messageId, string chatDate)
        {
            // Perform deletion logic here, e.g., remove message from data store

            // Broadcast message removal to all clients in the group
            await Clients.Group(groupName).SendAsync("MessageRemoved", messageId, chatDate);
        }

    }
}
