using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

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

        public async Task SendToUser(string senderId, string receiverId, string message)
        {
            string groupName = GetGroupName(receiverId);
            await Clients.Group(groupName).SendAsync("ReceiveMessage", senderId, message);
        }

        public async Task SendToOwnGroup(string senderId,string message)
        {
            string groupName = GetGroupName(senderId);
            await Clients.Group(groupName).SendAsync("ReceiveMessage", senderId, message);
        }

        private string GetGroupName(string userId)
        {
            return $"User_{userId}";
        }
    }
}
