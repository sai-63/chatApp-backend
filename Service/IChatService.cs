using login.Common.Models;
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
        // Other method signatures as needed
        Task<IEnumerable<Chat>> getIndividualMessages(String senderId, String receiverId);
    }
}
