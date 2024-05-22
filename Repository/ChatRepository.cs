using System.Collections.Generic;
using System.Threading.Tasks;
using login.Common.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Repository
{
    public class ChatRepository : IChatRepository
    {
        private readonly IMongoCollection<Chat> _collection;

        public ChatRepository(IMongoClient client, String databaseName, String collectionName)
        {
            var database = client.GetDatabase(databaseName);
            _collection = database.GetCollection<Chat>(collectionName);
        }

        public async Task<IEnumerable<Chat>> getAllChatsAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task AddChatAsync(Chat chat)
        {
            await _collection.InsertOneAsync(chat);
        }

        public async Task<IEnumerable<Chat>> getIndividualChats(String senderId, String receiverId)
        {
            var filter = Builders<Chat>.Filter.Or(
                Builders<Chat>.Filter.And(
                    Builders<Chat>.Filter.Eq(chat => chat.SenderId, senderId),
                    Builders<Chat>.Filter.Eq(chat => chat.ReceiverId, receiverId)
                ),
                Builders<Chat>.Filter.And(
                    Builders<Chat>.Filter.Eq(chat => chat.SenderId, receiverId),
                    Builders<Chat>.Filter.Eq(chat => chat.ReceiverId, senderId)
                )
            );

            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<bool> DeleteChatAsync(string messageId)
        {
            try
            {
                var filter = Builders<Chat>.Filter.Eq("MessageId", messageId);
                var result = await _collection.DeleteOneAsync(filter);
                return result.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log error)
                Console.WriteLine("Error deleting chat: " + ex.Message);
                return false;
            }
        }

        public async Task<bool> EditChatAsync(string messageId,string newMessage)
        {
            try
            {
                var filter = Builders<Chat>.Filter.Eq("MessageId", messageId);
                var update = Builders<Chat>.Update.Set("Message", newMessage);
                var result = await _collection.UpdateOneAsync(filter, update);
                return result.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log error)
                Console.WriteLine("Error updating chat message: " + ex.Message);
                return false;
            }
        }

        public async Task MarkAsReadAsync(List<String> messageIds)
        {
            var filter = Builders<Chat>.Filter.In(chat => chat.MessageId, messageIds);
            var update = Builders<Chat>.Update.Set(chat => chat.IsRead, true);

            await _collection.UpdateManyAsync(filter, update);
        }


        // Other methods for getting chats by sender/receiver id can be implemented similarly
    }
}
