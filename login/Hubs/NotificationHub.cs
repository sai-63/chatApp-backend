using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using login.Common.Models;
using Common.Models;

namespace login.Hubs
{
    public class NotificationHub : Hub
    {
        //For storing grp connection ids
        private static readonly Dictionary<string, List<string>> UserConnections = new Dictionary<string, List<string>>();
        public override async Task OnConnectedAsync()
        {
            string userId = Context.GetHttpContext().Request.Query["userId"];
            string groupName = GetGroupName(userId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            lock (UserConnections)
            {
                if (!UserConnections.ContainsKey(userId))
                {
                    UserConnections[userId] = new List<string>();
                }
                UserConnections[userId].Add(Context.ConnectionId);
            }

            await base.OnConnectedAsync();
        }
        //When user disconn
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string userId = Context.GetHttpContext().Request.Query["userId"];

            lock (UserConnections)
            {
                if (UserConnections.ContainsKey(userId))
                {
                    UserConnections[userId].Remove(Context.ConnectionId);
                    if (UserConnections[userId].Count == 0)
                    {
                        UserConnections.Remove(userId);
                    }
                }
            }

            await base.OnDisconnectedAsync(exception);
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
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
        public async Task SendToGroup(string groupid, string senderId, Grpmsg groupmsg)
        {
            if (UserConnections.TryGetValue(senderId, out var connectionIds))
            {
                foreach (var connectionId in connectionIds)
                {
                    await Clients.Client(connectionId).SendAsync("ReceiveGrpMessage", senderId, groupmsg);
                }
            }
            // await Groups.AddToGroupAsync(Context.ConnectionId,groupmsg.message);
            await Clients.Group(groupid).SendAsync("ReceiveGrpMessage", senderId, groupmsg);
        }

        public async Task RemoveMessageFromGroup(string groupName, string messageId, string chatDate)
        {
            // Perform deletion logic here, e.g., remove message from data store

            // Broadcast message removal to all clients in the group
            await Clients.Group(groupName).SendAsync("MessageRemoved", messageId, chatDate);
        }

    }
}
