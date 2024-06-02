using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using login.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.FileIO;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
namespace Repository
{
    public class GroupRepository : IGroupRepository
    {
        private readonly IMongoCollection<Grp> _groo;
        private readonly IMongoCollection<User> _user;

        public GroupRepository(IMongoClient client, string databaseName, string collectionName, string userCollection)
        {
            var database = client.GetDatabase(databaseName);
            _groo = database.GetCollection<Grp>(collectionName);
            _user = database.GetCollection<User>(userCollection);
        }

        public async Task<string> GetUnameAsync(string userId)
        {
            var user = await _user.Find(u => u.Id == userId).FirstOrDefaultAsync();
            return user?.Username;
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
        public async Task<Grp> GetUserGroupMessagesAsync(string groupname)
        {
            var filter = Builders<Grp>.Filter.Eq(g => g.Name, groupname);
            return await _groo.Find(filter).FirstOrDefaultAsync();
            //return await _groo.Find(_ => true).ToListAsync();
            //_logger.LogInformation($"Fetching group messages for user: {userId}");
            //var filter = Builders<Grp>.Filter.AnyEq("users", username);
            //var groups = await _groo.Find(filter).ToListAsync();

            //foreach (var group in groups)
            //{
            //  var messagesFilter = Builders<Grp>.Filter.Eq("_id", group.Id);
            //var projection = Builders<Grp>.Projection.Include("messages");
            //var groupWithMessages = await _groo.Find(messagesFilter).Project<Grp>(projection).FirstOrDefaultAsync();

            //if (groupWithMessages != null)
            //{
            //group.Messages = groupWithMessages.Messages;
            //}
            //}

            //return groups;
        }
        public async Task<Grp> FullDetOfGroupAsync(string groupname)
        {
            var filter = Builders<Grp>.Filter.Eq(g => g.Name, groupname);
            return await _groo.Find(filter).FirstOrDefaultAsync();
        }
        public async Task<List<string>> GetallgrpsAsync(string username)
        {
            var filter = Builders<Grp>.Filter.AnyEq(g => g.Users, username);
            var projection = Builders<Grp>.Projection.Include(g => g.Name).Exclude("_id");
            var groups = await _groo.Find(filter).Project<Grp>(projection).ToListAsync();
            return groups.Select(g => g.Name).ToList();
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
        public async Task<string> GetgroupidAsync(string gname)
        {
            var group = await _groo.Find(group => group.Name == gname).FirstOrDefaultAsync();
            return group == null ? null : group.Id.ToString();
            //return group.Id.ToString("N");
        }
        public async Task<string> GetgroupnameAsync(ObjectId groupid)
        {
            var group = await _groo.Find(group => group.Id == groupid).FirstOrDefaultAsync();
            return group == null ? null : group.Name;
            //return group.Id.ToString("N");
        }

        public async Task AddGrpChatAsync(string groupname, Grpmsg gm)
        {
            var newMessage = new Grpmsg
            {
                //Id = ObjectId.GenerateNewId().ToString(),
                Id=gm.Id,
                SenderId = gm.SenderId,
                Message = gm.Message,
                FileName= gm.FileName,
                FileType=gm.FileType,
                FileContent=gm.FileContent,
                FileSize=gm.FileSize,
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
        public async Task<Dictionary<string, string>> Getnameid()
        {
            var abc = await _user.Find(_ => true).ToListAsync();
            return abc.ToDictionary(u => u.Id, u => u.Nickname);
        }

        public async Task<bool> DeleteGrpMessageAsync(string groupname, string messageId)
        {
            try
            {
                var groupFilter = Builders<Grp>.Filter.Eq(g => g.Name, groupname);
                var messageFilter = Builders<Grp>.Filter.ElemMatch(g => g.Messages, m => m.Id == messageId);

                var combinedFilter = Builders<Grp>.Filter.And(groupFilter, messageFilter);

                var update = Builders<Grp>.Update.PullFilter(g => g.Messages, m => m.Id == messageId);

                var result = await _groo.UpdateOneAsync(combinedFilter, update);

                return result.ModifiedCount > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error deleting chat message: " + e.Message);
                return false;
            }
        }
        public async Task<bool> DeleteGrpForMeAsync(string groupname, string messageId)
        {
            try
            {
                var groupFilter = Builders<Grp>.Filter.Eq(g => g.Name, groupname);
                var messageFilter = Builders<Grp>.Filter.ElemMatch(g => g.Messages, m => m.Id == messageId && m.DeletedBy==false);
                var combinedFilter = Builders<Grp>.Filter.And(groupFilter, messageFilter);

                var update = Builders<Grp>.Update.Set("messages.$.DeletedBy", true);

                var result = await _groo.UpdateOneAsync(combinedFilter, update);

                return result.ModifiedCount > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error deleting chat message for user: " + e.Message);
                return false;
            }
        }


    }
}