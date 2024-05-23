using System.Collections.Generic;
using System.Threading.Tasks;
using login.Common.Models;
using MongoDB.Bson;
using MongoDB.Driver.Core.Servers;

namespace Repository
{
    public interface IChatRepository
    {
        Task<IEnumerable<Chat>> getAllChatsAsync();
        Task AddChatAsync(Chat chat);
        Task<bool> DeleteChatAsync(string messageId);
        Task<bool> DeleteChatForMeAsync(string messageId);
        Task<bool> EditChatAsync(string messageId,string newMessage);
        // Other method signatures as needed
        Task<IEnumerable<Chat>> getIndividualChats(String senderId, String receiverId);
        Task MarkAsReadAsync(List<string> messageIds);
    }
}
