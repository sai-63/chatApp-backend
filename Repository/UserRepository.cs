using login.Common.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _collection;

        public UserRepository(IMongoClient client, string databaseName, string collectionName)
        {
            var database = client.GetDatabase(databaseName);
            _collection = database.GetCollection<User>(collectionName);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _collection.Find(u => u.Username == username).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _collection.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task AddUserAsync(User user)
        {
            await _collection.InsertOneAsync(user);
        }

        public async Task<List<User>> GetAllUsersAsync(string username)
        {
            return await _collection.Find(e => e.Username != username).ToListAsync();
        }

        public async Task<bool> addFriendAsync(string userId, string friendId)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);

            // Update definition to add the userId to the Friends array
            var update = Builders<User>.Update.Push(u => u.Friends, friendId);

            // Update the user document in the collection
            var result = await _collection.UpdateOneAsync(filter, update);

            // Check if the update was successful
            if(result.ModifiedCount > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<List<User>> GetAllFriendsAsync(string userId)
        {
            // Assuming _collection is your MongoDB collection
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var user = await _collection.Find(filter).FirstOrDefaultAsync();

            if (user == null)
            {
                // User not found, return an empty list or handle it accordingly
                return new List<User>();
            }

            var friendFilter = Builders<User>.Filter.In(u => u.Id, user.Friends);
            var friends = await _collection.Find(friendFilter).ToListAsync();

            return friends;
        }

        public async Task<bool> isFriendAsync(string userId, string friendId)
        {
            var filter = Builders<User>.Filter.And(
        Builders<User>.Filter.Eq(u => u.Id, userId),
        Builders<User>.Filter.AnyEq(u => u.Friends, friendId)
    );

            // Check if there's any user matching the filter
            var user = await _collection.Find(filter).FirstOrDefaultAsync();

            // If user is not null, friendId is in the Friends list
            return user != null;
        }

        public async Task setUserOnlineAsync(string userName)
        {
            var filter = Builders<User>.Filter.Eq("Username", userName);
            var update = Builders<User>.Update.Set("IsOnline", true);
            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task setUserOfflineAsync(string userName)
        {
            var filter = Builders<User>.Filter.Eq("Username", userName);
            var update = Builders<User>.Update.Set("IsOnline", false);
            await _collection.UpdateOneAsync(filter, update);
        }
    }
}
