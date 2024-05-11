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

        public async Task<IEnumerable<Group>> GetUserGroupsAsync(string userId)
        {
            var filter = Builders<Group>.Filter.AnyEq(g => g.Users, userId);
            return await _groo.Find(filter).ToListAsync();
        }

        //public async Task<IEnumerable<Group>> GetGroupMessagesAsync(string groupname)
        //{
        //  var filter = Builders<Group>.Filter.Eq(c => c.Name, groupname);
        //return await _groo.Find(filter).ToListAsync();
        //}
        public async Task<IEnumerable<New>> GetGroupMessagesAsync(string groupname)
        {
            var filter = Builders<Group>.Filter.Eq(g => g.Name, groupname);
            var group = await _groo.Find(filter).FirstOrDefaultAsync();

            // If group is found, return its messages
            if (group != null)
            {
                // Transform Group messages into a list of Message objects
                var messages = group.Messages.Select(msg => new New
                {
                    SenderId = msg.SenderId,
                    Message = msg.Message
                }).ToList();

                return messages;
            }
            else
            {
                // Group not found, return an empty list
                return new List<New>();
            }
        }

    }
}
