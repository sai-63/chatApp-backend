using System.Collections.Generic;
using System.Threading.Tasks;
using login.Common.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Repository
{
    public class GroupRepository : IGroupRepository
    {

        private readonly IMongoCollection<Group> _groo;

        public GroupRepository(IMongoClient client, string databaseName, string collectionName)
        {
            var database = client.GetDatabase(databaseName);
            _groo = database.GetCollection<Group>(collectionName);
        }

        public async Task<Group> GetGroupByNameAsync(string groupName)
        {
            var filter = Builders<Group>.Filter.Eq(g => g.Name, groupName);
            return await _groo.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateGroupAsync(Group group)
        {
            await _groo.InsertOneAsync(group);
        }

        public async Task AddUsersToGroupAsync(string groupId, List<string> userIds)
        {
            var filter = Builders<Group>.Filter.Eq(g => g.Id, ObjectId.Parse(groupId));
            var update = Builders<Group>.Update.AddToSetEach(g => g.Users, userIds);
            await _groo.UpdateOneAsync(filter, update);
        }

        //Get all groups
        public async Task<IEnumerable<Group>> GetAllGroupsAsync()
        {
            return await _groo.Find(_ => true).ToListAsync();
        }
    }
}
