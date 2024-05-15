using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Models;
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

        public async Task<bool>AddUsersToGroupAsync(Joingrp j)
        {
            var filter = Builders<Group>.Filter.Eq("name",j.groupname);
            var group = await _groo.Find(filter).FirstOrDefaultAsync();

            if (group != null)
            {
                if (group.Users.Contains(j.username))
                {
                    // User is already in the group, print a message or handle it as needed
                    return false;
                }
                // Check if the user is already in the group
                else
                {
                    // Add the user to the group
                    group.Users.Add(j.username);

                    // Update the group document in the database
                    var update = Builders<Group>.Update.Set("users", group.Users);
                    await _groo.UpdateOneAsync(filter, update);
                    return true;
                }
            }
            else
            {
                // Handle case where group is not found
                throw new Exception("Group not found.");
            }
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

        public async Task<IEnumerable<List<String>>> GetUsersOfGroupAsync(string groupname)
        {
            var filter = Builders<Group>.Filter.Eq(g => g.Name, groupname);
            var group = await _groo.Find(filter).FirstOrDefaultAsync();
            if (group != null)
            {
                return group.Users.Select(user => new List<string> { user }).ToList();
            }
            else
            {
                return new List<List<String>>();
            }
        }

        public async Task<bool> RemoveFromGroupAsync(Joingrp j)
        {
            var filter = Builders<Group>.Filter.Eq("name", j.groupname);
            var group = await _groo.Find(filter).FirstOrDefaultAsync();
            if (group != null)
            {
                if (group.Users.Contains(j.username))
                {
                    group.Users.Remove(j.username);
                    var update = Builders<Group>.Update.Set("users", group.Users);
                    await _groo.UpdateOneAsync(filter, update);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                throw new Exception("Group not found.");
            }

            //await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has left the group {groupName}.");
        }

    }
}
