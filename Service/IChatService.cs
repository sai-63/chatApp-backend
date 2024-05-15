using login.Common.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface IChatService
    {
        Task<IEnumerable<Chat>> GetAllChatsAsync();
        Task SendMessageAsync(Chat message);
        Task<bool> DeleteMessageAsync(string id);
        // Other method signatures as needed
        Task<IDictionary<string, List<Chat>>> getIndividualMessages(String senderId, String receiverId);
    }
}
