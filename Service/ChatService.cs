using System.Collections.Generic;
using System.Threading.Tasks;
using login.Common.Models;
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

        public async Task<IEnumerable<Chat>> GetAllChatsAsync()
        {
            return await _chatRepository.getAllChatsAsync();
        }

        public async Task<IEnumerable<Chat>> getIndividualMessages(string senderId, string receiverId)
        {
            return await _chatRepository.getIndividualChats(senderId, receiverId);
        }

        public async Task SendMessageAsync(Chat message)
        {
            await _chatRepository.AddChatAsync(message);
        }

        // Other methods for getting chats by sender/receiver id can be implemented similarly
    }
}
