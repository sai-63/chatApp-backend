using login.Common.Models;
using Microsoft.AspNetCore.Http;
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
        Task SendMessageWithFileAsync(Chat message, IFormFile file);
        Task<bool> DeleteMessageAsync(string messageId);
        Task<bool> DeleteMessageForMeAsync(string messageId);
        Task<bool> EditMessageAsync(string messageId,string newMessage);
        // Other method signatures as needed
        Task<IDictionary<string, List<Chat>>> getIndividualMessages(String senderId, String receiverId);
        Task MarkAsRead(List<string> messageIds);
    }
}
