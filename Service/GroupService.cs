using System.Collections.Generic;
using System.Threading.Tasks;
using login.Common.Models;
using Repository;

namespace Service
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;

        public GroupService(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }
        public async Task<Group> GetGroupByNameAsync(string groupName)
        {
            return await _groupRepository.GetGroupByNameAsync(groupName);
        }

        public async Task CreateGroupAsync(Group group)
        {
            await _groupRepository.CreateGroupAsync(group);
        }

        public async Task AddUsersToGroupAsync(string groupId, List<string> userIds)
        {
            await _groupRepository.AddUsersToGroupAsync(groupId, userIds);
        }

        //Get all groups
        public async Task<IEnumerable<Group>> GetAllGroups()
        {
            return await _groupRepository.GetAllGroupsAsync();
        }
    }
}
