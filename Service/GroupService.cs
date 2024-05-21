using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Common.Models;
using login.Common.Models;
using MongoDB.Bson;
using Repository;
using Group = Common.Models.Group;

namespace Service
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;

        public GroupService(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }
        public async Task<Common.Models.Group> GetGroupByNameAsync(string groupName)
        {
            return await _groupRepository.GetGroupByNameAsync(groupName);
        }
        public async Task<IEnumerable<string>> GetgroupidAsync(string gname)
        {
            return await _groupRepository.GetgroupidAsync(gname);
        }
        public async Task CreateGroupAsync(Group group)
        {
            await _groupRepository.CreateGroupAsync(group);
        }

        public async Task<bool> AddUsersToGroupAsync(Joingrp j)
        {
            return await _groupRepository.AddUsersToGroupAsync(j);
        }

        //Get all groups
        public async Task<IEnumerable<Group>> GetAllGroups()
        {
            return await _groupRepository.GetAllGroupsAsync();
        }

        //public async Task<IEnumerable<Group>> GetUserGroupsAsync(string userId)
        //{
        //  return await _groupRepository.GetUserGroupsAsync(userId);
        //}
        public async Task<IEnumerable<Grpmsg>> GetGroupMessagesAsync(string groupname)
        {
            return await _groupRepository.GetGroupMessagesAsync(groupname);
        }

        public async Task<IEnumerable<List<string>>> GetUsersOfGroupAsync(string groupname)
        {
            return await _groupRepository.GetUsersOfGroupAsync(groupname);
        }
        public async Task<bool> RemoveFromGroupAsync(Joingrp j)
        {
            return await _groupRepository.RemoveFromGroupAsync(j);

            //await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has left the group {groupName}.");
        }


        public async Task SendGrpMessageAsync(string groupname,Grpmsg messages)
        {
            await _groupRepository.AddGrpChatAsync(groupname,messages);
        }
    }
}
