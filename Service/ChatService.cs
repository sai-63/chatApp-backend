using System.Collections.Generic;
using System.Threading.Tasks;
using login.Common.Models;
using Microsoft.AspNetCore.Http;
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

        public async Task<bool> DeleteMessageAsync(string messageId)
        {
            return await _chatRepository.DeleteChatAsync(messageId);
        }

        public async Task<bool> DeleteMessageForMeAsync(string messageId)
        {
            return await _chatRepository.DeleteChatForMeAsync(messageId);
        }

        public async Task<bool> EditMessageAsync(string messageId, string newMessage)
        {
            return await _chatRepository.EditChatAsync(messageId, newMessage);
        }

        public async Task<IEnumerable<Chat>> GetAllChatsAsync()
        {
            return await _chatRepository.getAllChatsAsync();
        }

        public async Task<IDictionary<string, List<Chat>>> getIndividualMessages(string senderId, string receiverId)
        {
            var chats = await _chatRepository.getIndividualChats(senderId, receiverId);

            var filteredChats = chats
                .Where(chat => !(chat.SenderId == senderId && chat.SenderRemoved) || chat.SenderId == receiverId)
                .ToList();

            var groupedChats = filteredChats
                .GroupBy(chat => chat.Timestamp.Date) // Group by date
                .ToDictionary(
                    group => group.Key.ToString("yyyy-MM-dd"), // Use the date as a key in the desired format
                    group => group.ToList() // Convert the group to a list of chats
                );

            return groupedChats;
        }


        public async Task MarkAsRead(List<String> messageIds)
        {
            await _chatRepository.MarkAsReadAsync(messageIds);
        }

        public async Task SendMessageAsync(Chat message)
        {
            await _chatRepository.AddChatAsync(message);
        }

        public async Task SendMessageWithFileAsync(Chat message, IFormFile file)
        {
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                message.FileContent = ms.ToArray();
                message.FileName = (string)(file.FileName);
                message.FileType = file.ContentType;
                message.FileSize = file.Length;
            }


            // Save the chat message (with file) to the database
            await _chatRepository.AddChatAsync(message);
        }

        // Other methods for getting chats by sender/receiver id can be implemented similarly
    }
}
