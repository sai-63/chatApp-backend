using login.Common.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface IGroupService
    {
        Task<Group> GetGroupByNameAsync(string groupName);
        Task CreateGroupAsync(Group group);
        Task AddUsersToGroupAsync(string groupId, List<string> userIds);
        Task<IEnumerable<Group>> GetAllGroups();
        //Task<IEnumerable<Group>> GetUserGroupsAsync(string userId);

        Task<IEnumerable<New>> GetGroupMessagesAsync(string groupname);
        
    }
}
