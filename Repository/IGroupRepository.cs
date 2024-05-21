using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common.Models;
using login.Common.Models;
using MongoDB.Bson;
using MongoDB.Driver.Core.Servers;
using Group = Common.Models.Group;
namespace Repository
{
    public interface IGroupRepository
    {
        Task<Group> GetGroupByNameAsync(string groupName);
        Task CreateGroupAsync(Group group);
        Task<bool> AddUsersToGroupAsync(Joingrp j);
        Task<IEnumerable<Group>> GetAllGroupsAsync();
        Task<IEnumerable<Grpmsg>> GetGroupMessagesAsync(string groupname);
        Task<IEnumerable<List<string>>> GetUsersOfGroupAsync(string groupname);
        Task<bool> RemoveFromGroupAsync(Joingrp j);
        Task<IEnumerable<string>> GetgroupidAsync(string gname);

        Task AddGrpChatAsync(string groupname, Grpmsg gm);
    }
}
