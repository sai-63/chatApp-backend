using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
//using Common.Models;
using login.Common.Models;
using MongoDB.Bson;
using MongoDB.Driver;
//using Group = login.Common.Models.Group;
namespace Repository
{
    public class GroupRepository : IGroupRepository
    {
        private readonly IMongoCollection<Grp> _groo;

        public GroupRepository(IMongoClient client, string databaseName, string collectionName)
        {
            var database = client.GetDatabase(databaseName);
            _groo = database.GetCollection<Grp>(collectionName);
        }

        public async Task<Grp> GetGroupByNameAsync(string groupName)
        {
            var filter = Builders<Grp>.Filter.Eq(g => g.Name, groupName);
            return await _groo.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateGroupAsync(Grp group)
        {
            await _groo.InsertOneAsync(group);
        }

        public async Task<bool> AddUsersToGroupAsync(Joingrp j)
        {
            var filter = Builders<Grp>.Filter.Eq("name", j.groupname);
            var group = await _groo.Find(filter).FirstOrDefaultAsync();

            if (group != null)
            {
                if (group.Users.Contains(j.username))
                {
                    return false;
                }
                else
                {
                    group.Users.Add(j.username);
                    var update = Builders<Grp>.Update.Set("users", group.Users);
                    await _groo.UpdateOneAsync(filter, update);
                    return true;
                }
            }
            else
            {
                throw new Exception("Group not found.");
            }
        }

        //Get all groups
        public async Task<IEnumerable<Grp>> GetAllGroupsAsync()
        {
            return await _groo.Find(_ => true).ToListAsync();
        }

        public async Task<IEnumerable<Grp>> GetUserGroupsAsync(string userId)
        {
            var filter = Builders<Grp>.Filter.AnyEq(g => g.Users, userId);
            return await _groo.Find(filter).ToListAsync();
        }
        public async Task<IEnumerable<Grpmsg>> GetGroupMessagesAsync(string groupname)
        {
            var filter = Builders<Grp>.Filter.Eq(g => g.Name, groupname);
            var group = await _groo.Find(filter).FirstOrDefaultAsync();
            if (group != null)
            {
                var messages = group.Messages.Select(msg => new Grpmsg
                {
                    SenderId = msg.SenderId,
                    Message = msg.Message
                }).ToList();

                return group.Messages;
            }
            else
            {
                return new List<Grpmsg>();
            }
        }

        public async Task<IEnumerable<List<string>>> GetUsersOfGroupAsync(string groupname)
        {
            var filter = Builders<Grp>.Filter.Eq(g => g.Name, groupname);
            var group = await _groo.Find(filter).FirstOrDefaultAsync();
            if (group != null)
            {
                return group.Users.Select(user => new List<string> { user }).ToList();
            }
            else
            {
                return new List<List<string>>();
            }
        }

        public async Task<bool> RemoveFromGroupAsync(Joingrp j)
        {
            var filter = Builders<Grp>.Filter.Eq("name", j.groupname);
            var group = await _groo.Find(filter).FirstOrDefaultAsync();
            if (group != null)
            {
                if (group.Users.Contains(j.username))
                {
                    group.Users.Remove(j.username);
                    var update = Builders<Grp>.Update.Set("users", group.Users);
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
        public async Task<IEnumerable<string>> GetgroupidAsync(string gname)
        {
            var group = await _groo.Find(group => group.Name == gname).FirstOrDefaultAsync();
            return group == null ? null : new List<string> { group.Id.ToString() };
            //return group.Id.ToString("N");
        }

        public async Task AddGrpChatAsync(string groupname, Grpmsg gm)
        {
            var newMessage = new Grpmsg
            {
                Id = ObjectId.GenerateNewId().ToString(),
                SenderId = gm.SenderId,
                Message = gm.Message,
                Timestamp = DateTime.UtcNow
            };
            var filter = Builders<Grp>.Filter.Eq(g => g.Name, groupname);
            var update = Builders<Grp>.Update.Push(g => g.Messages, newMessage);
            await _groo.UpdateOneAsync(filter, update);
        }

        public async Task<bool> EditGChatAsync(string groupname, string messageId, string newMessage)
        {
            try
            {
                var groupFilter = Builders<Grp>.Filter.Eq(g => g.Name, groupname);
                var messageFilter = Builders<Grp>.Filter.ElemMatch(g => g.Messages, m => m.Id == messageId);
                var combinedFilter = Builders<Grp>.Filter.And(groupFilter, messageFilter);
                var update = Builders<Grp>.Update.Set("Messages.$.Message", newMessage);
                var result = await _groo.UpdateOneAsync(combinedFilter, update);
                return result.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating chat message: " + ex.Message);
                return false;
            }
        }

    }
}