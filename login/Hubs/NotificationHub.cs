using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;
using login.Common.Models;
using System.Text.RegularExpressions;

namespace login.Hubs
{
    public class NotificationHub : Hub
    {
        // For storing group connection IDs
        private static readonly Dictionary<string, List<string>> GroupConnections = new Dictionary<string, List<string>>();
        private static readonly Dictionary<string, List<Grpmsg>> GroupMessages = new Dictionary<string, List<Grpmsg>>();

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
            await HandleUserDisconnectAsync(connectionId);
        }

        private Task HandleUserDisconnectAsync(string connectionId)
        {
            // Your logic to handle the user disconnection
            return Task.CompletedTask;
        }

        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            string userId = Context.GetHttpContext().Request.Query["userId"];
            await HandleUserDisconnectAsync(Context.ConnectionId);
            RemoveConnectionFromGroups(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        private void RemoveConnectionFromGroups(string connectionId)
        {
            lock (GroupConnections)
            {
                foreach (var group in GroupConnections.Values)
                {
                    group.Remove(connectionId);
                }
            }
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendToUser(string senderId, string receiverId, Chat chat)
        {
            string groupName = GetGroupName(receiverId);
            string mygroupName = GetGroupName(senderId);
            await Clients.Group(groupName).SendAsync("ReceiveMessage", chat);
            await Clients.Group(mygroupName).SendAsync("ReceiveMessage", chat);
        }

        public async Task RemoveMessage(string receiverId, string messageId, string chatDate)
        {
            string userId = Context.GetHttpContext().Request.Query["userId"];
            string groupName = GetGroupName(receiverId);
            string mygroupName = GetGroupName(userId);
            await Clients.Group(groupName).SendAsync("MessageRemoved", messageId, chatDate);
            await Clients.Group(mygroupName).SendAsync("MessageRemoved", messageId, chatDate);
        }

        public async Task RemoveGrpMessage(string groupid, string messageId, string chatDate)
        {
            await Clients.Group(groupid).SendAsync("GrpMessageRemoved", messageId, chatDate);
        }

        public async Task EditMessage(string receiverId, string messageId, string newMessage, string chatDate)
        {
            string userId = Context.GetHttpContext().Request.Query["userId"];
            string groupName = GetGroupName(receiverId);
            string mygroupName = GetGroupName(userId);
            await Clients.Group(groupName).SendAsync("MessageEdited", messageId, newMessage, chatDate);
            await Clients.Group(mygroupName).SendAsync("MessageEdited", messageId, newMessage, chatDate);
        }

        public async Task MarkAsRead(string receiverId, List<string> messageIds)
        {
            string groupName = GetGroupName(receiverId);
            await Clients.Group(groupName).SendAsync("MessageRead", messageIds);
        }

        public async Task UserOnline(string username)
        {
            await Clients.All.SendAsync("UserOnline", username);
        }

        public async Task UserOffline(string username)
        {
            await Clients.All.SendAsync("UserOffline", username);
        }

        public async Task IncrementUnseenMessages(string receiverId, string username, string seen = null)
        {
            string groupName = GetGroupName(receiverId);
            await Clients.Group(groupName).SendAsync("IncrementUnseenMessages", username, seen);
        }

        public async Task SortChats(string receiverId, string username, string timestamp)
        {
            string groupName = GetGroupName(receiverId);
            await Clients.Group(groupName).SendAsync("SortChats", username, timestamp);
        }

        private string GetGroupName(string userId)
        {
            return $"User_{userId}";
        }

        public async Task LeaveGroup(string groupName)
        {
            try
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
                lock (GroupConnections)
                {
                    if (GroupConnections.ContainsKey(groupName))
                    {
                        GroupConnections[groupName].Remove(Context.ConnectionId);
                        if (GroupConnections[groupName].Count == 0)
                        {
                            GroupConnections.Remove(groupName);
                            GroupMessages.Remove(groupName);
                        }
                    }
                }
                ClearGroupMessages(groupName);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error leaving group '{groupName}': {ex.Message}");
                throw;
            }
        }

        public async Task JoinGroup(string groupid)
        {
            try
            {
                Console.WriteLine($"Attempting to join group: {groupid}, ConnectionId: {Context.ConnectionId}");
                await Groups.AddToGroupAsync(Context.ConnectionId, groupid);
                lock (GroupConnections)
                {
                    if (!GroupConnections.ContainsKey(groupid))
                    {
                        GroupConnections[groupid] = new List<string>();
                    }
                    GroupConnections[groupid].Add(Context.ConnectionId);
                }
                if (GroupMessages.TryGetValue(groupid, out var messages))
                {
                    foreach (var msg in messages)
                    {
                        await Clients.Caller.SendAsync("ReceiveGrpMessage", msg.SenderId, msg);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error joining group '{groupid}': {ex.Message}");
                throw;
            }
        }

        public async Task SendToGroup(string senderId, string groupid, Grpmsg groupmsg)
        {
            if (GroupConnections.TryGetValue(groupid, out var connectionIds))
            {
                foreach (var connectionId in connectionIds)
                {
                    await Clients.Client(connectionId).SendAsync("ReceiveGrpMessage", senderId, groupmsg);
                }
            }

            lock (GroupMessages)
            {
                if (!GroupMessages.ContainsKey(groupid))
                {
                    GroupMessages[groupid] = new List<Grpmsg>();
                }
                GroupMessages[groupid].Add(groupmsg);
            }
        }
        private void ClearGroupMessages(string groupName)
        {
            lock (GroupMessages)
            {
                if (GroupMessages.ContainsKey(groupName))
                {
                    GroupMessages.Remove(groupName);
                }
            }
        }
    }
}
