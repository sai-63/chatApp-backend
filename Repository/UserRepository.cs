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

        public async Task<List<User>> GetAllUsersAsync(string id)
        {
            return await _collection.Find(e => e.Id != id).ToListAsync();
        }

    }
}
