using System.Collections.Generic;
using System.Threading.Tasks;
using login.Common.Models;
using MongoDB.Bson;
using Repository;

namespace Service
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;

        public ChatService(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public async Task<bool> DeleteMessageAsync(string id)
        {
            return await _chatRepository.DeleteChatAsync(id);
        }

        public async Task<IEnumerable<Chat>> GetAllChatsAsync()
        {
            return await _chatRepository.getAllChatsAsync();
        }

        public async Task<IDictionary<string, List<Chat>>> getIndividualMessages(string senderId, string receiverId)
        {
            var chats = await _chatRepository.getIndividualChats(senderId, receiverId);

            var groupedChats = chats
                .GroupBy(chat => chat.Timestamp.Date) // Group by date
                .ToDictionary(
                    group => group.Key.ToString("yyyy-MM-dd"), // Use the date as a key in the desired format
                    group => group.ToList() // Convert the group to a list of chats
                );

            return groupedChats;
        }


        public async Task SendMessageAsync(Chat message)
        {
            await _chatRepository.AddChatAsync(message);
        }

        // Other methods for getting chats by sender/receiver id can be implemented similarly
    }
}
