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
        //Store grpmsg from Footer for a specific groupid
        private static readonly Dictionary<string, List<Grpmsg>> GroupMessages = new Dictionary<string, List<Grpmsg>>();
        //Store messages that are deleted
        //private static readonly Dictionary<string, List<Grpmsg>> DelGroupMessages = new Dictionary<string, List<Grpmsg>>();
        private static readonly Dictionary<string, List<Abc>> DelGroupMessages = new Dictionary<string, List<Abc>>();

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
                            //DelGroupMessages.Remove(groupName);
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

        public async Task JoinGroup(string groupName)
        {
            try
            {
                Console.WriteLine($"Attempting to join group: {groupName}, ConnectionId: {Context.ConnectionId}");
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                lock (GroupConnections)
                {
                    if (!GroupConnections.ContainsKey(groupName))
                    {
                        GroupConnections[groupName] = new List<string>();
                    }
                    GroupConnections[groupName].Add(Context.ConnectionId);
                }
                if (GroupMessages.TryGetValue(groupName, out var messages))
                {
                    foreach (var msg in messages)
                    {
                        await Clients.Caller.SendAsync("ReceiveGrpMessage", msg.SenderId, msg);
                    }                    
                }
                Console.WriteLine("Hey");
                if (DelGroupMessages.TryGetValue(groupName, out var messagess))
                {
                    Console.WriteLine("Hi");
                    foreach (var msg in messagess)
                    {                        
                        await Clients.Caller.SendAsync("GrpMessageRemoved", msg.Id, msg.Timestamp);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error joining group '{groupName}': {ex.Message}");
                throw;
            }
        }

        public async Task SendToGroup(string senderId, string groupName, Grpmsg groupmsg)
        {
            if (GroupConnections.TryGetValue(groupName, out var connectionIds))
            {
                foreach (var connectionId in connectionIds)
                {
                    await Clients.Client(connectionId).SendAsync("ReceiveGrpMessage", senderId, groupmsg);
                }
            }

            lock (GroupMessages)
            {
                if (!GroupMessages.ContainsKey(groupName))
                {
                    GroupMessages[groupName] = new List<Grpmsg>();
                }
                GroupMessages[groupName].Add(groupmsg);
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

        public async Task RemoveGrpMessage(string groupName, string messageId, string chatDate)
        {
            // Parse the chatDate string to DateTime
            if (!DateTime.TryParse(chatDate, out var parsedDate))
            { return; }

            Console.WriteLine($"RemoveGrpMessage called with groupName: {groupName}, messageId: {messageId}, chatDate: {chatDate},{parsedDate},{parsedDate.Date}");
            lock (GroupMessages)
            {
                if (GroupMessages.TryGetValue(groupName, out var messages))
                {
                    
                    var messageToRemove = messages.FirstOrDefault(m => m.Id == messageId && m.Timestamp == parsedDate.Date);

                    Console.WriteLine("Got", messageToRemove);
                    if (messageToRemove != null)
                    {
                        messages.Remove(messageToRemove);
                       /* lock (DelGroupMessages)
                        {
                            if (!DelGroupMessages.ContainsKey(groupName))
                            {
                                DelGroupMessages[groupName] = new List<Grpmsg>();
                            }
                            DelGroupMessages[groupName].Add(messageToRemove);
                        }*/
                    }
                }
                lock (DelGroupMessages)
                {
                    if (!DelGroupMessages.ContainsKey(groupName))
                    {
                        DelGroupMessages[groupName] = new List<Abc>();
                    }
                    var deletedMessage = new Abc
                    {
                        Id = messageId,
                        Timestamp = chatDate,
                    };
                    Console.WriteLine(deletedMessage.Id,deletedMessage.Timestamp);
                    DelGroupMessages[groupName].Add(deletedMessage);                   
                }
            }
            await Clients.Group(groupName).SendAsync("GrpMessageRemoved", messageId, chatDate);
        }

        /*private bool IsMessageDeleted(string groupName, string messageId, DateTime timestamp)
        {
            lock (DelGroupMessages)
            {
                if (DelGroupMessages.TryGetValue(groupName, out var deletedMessages))
                {
                    return deletedMessages.Any(m => m.Id == messageId && m.Timestamp == timestamp);
                }
            }
            return false;
        }*/

    }
}
