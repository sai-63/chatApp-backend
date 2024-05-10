using System.Collections.Generic;
using System.Threading.Tasks;
using login.Common.Models;
using MongoDB.Driver.Core.Servers;

namespace Repository
{
    public interface IGroupRepository
    {
        Task<Group> GetGroupByNameAsync(string groupName);
        Task CreateGroupAsync(Group group);
        Task AddUsersToGroupAsync(string groupId, List<string> userIds);
        Task<IEnumerable<Group>> GetAllGroupsAsync();
    }
}
