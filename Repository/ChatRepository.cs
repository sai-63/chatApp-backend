using System.Collections.Generic;
using System.Threading.Tasks;
using login.Common.Models;
using MongoDB.Driver;

namespace Repository
{
    public class ChatRepository : IChatRepository
    {
        private readonly IMongoCollection<Chat> _collection;

        public ChatRepository(IMongoClient client, string databaseName, string collectionName)
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
            chat.Timestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            await _collection.InsertOneAsync(chat);
        }

        public async Task<IEnumerable<Chat>> getIndividualChats(string senderId, string receiverId)
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


        // Other methods for getting chats by sender/receiver id can be implemented similarly
    }
}
