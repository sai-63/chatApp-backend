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

        public async Task<bool> DeleteChatAsync(string id)
        {
            try
            {
                var filter = Builders<Chat>.Filter.Eq("MessageId", id);
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


        // Other methods for getting chats by sender/receiver id can be implemented similarly
    }
}
