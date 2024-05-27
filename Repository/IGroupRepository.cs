using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using login.Common.Models;
using MongoDB.Bson;
using MongoDB.Driver.Core.Servers;
namespace Repository
{
    public interface IGroupRepository
    {
        Task<string> GetUnameAsync(string userId);
        Task<Grp> GetGroupByNameAsync(string groupName);
        Task CreateGroupAsync(Grp group);
        Task<bool> AddUsersToGroupAsync(Joingrp j);
        //Task<IEnumerable<Grp>> GetAllGroupsAsync();
        Task<IEnumerable<Grp>> GetUserGroupMessagesAsync(string username);
        Task<IEnumerable<Grpmsg>> GetGroupMessagesAsync(string groupname);
        Task<IEnumerable<List<string>>> GetUsersOfGroupAsync(string groupname);
        Task<bool> RemoveFromGroupAsync(Joingrp j);
        Task<IEnumerable<string>> GetgroupidAsync(string gname);

        Task AddGrpChatAsync(string groupname, Grpmsg gm);
        Task<bool> EditGChatAsync(string groupname, string messageId, string newMessage);
    }
}