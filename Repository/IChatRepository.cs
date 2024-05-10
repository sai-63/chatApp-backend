using System.Collections.Generic;
using System.Threading.Tasks;
using login.Common.Models;
using MongoDB.Driver.Core.Servers;

namespace Repository
{
    public interface IChatRepository
    {
        Task<IEnumerable<Chat>> getAllChatsAsync();
        Task AddChatAsync(Chat chat);
        // Other method signatures as needed
        Task<IEnumerable<Chat>> getIndividualChats(String senderId, String receiverId);
    }
}
