using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models;
using login.Common.Models;
using MongoDB.Bson;
using MongoDB.Driver.Core.Servers;

namespace Repository
{
    public interface IGroupRepository
    {
        Task<Group> GetGroupByNameAsync(string groupName);
        Task CreateGroupAsync(Group group);
        Task<bool>AddUsersToGroupAsync(Joingrp j);
        Task<IEnumerable<Group>> GetAllGroupsAsync();
        Task<IEnumerable<New>> GetGroupMessagesAsync(string groupname);
        Task<IEnumerable<List<String>>> GetUsersOfGroupAsync(string groupname);

    }
}
