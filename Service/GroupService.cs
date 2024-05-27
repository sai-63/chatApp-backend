using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using login.Common.Models;
using MongoDB.Bson;
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

        public async Task<string> GetUNameAsync(string userId)
        {
            return await _groupRepository.GetUnameAsync(userId);
        }
        public async Task<Grp> GetGroupByNameAsync(string groupName)
        {
            return await _groupRepository.GetGroupByNameAsync(groupName);
        }
        public async Task<IEnumerable<string>> GetgroupidAsync(string gname)
        {
            return await _groupRepository.GetgroupidAsync(gname);
        }
        public async Task CreateGroupAsync(Grp group)
        {
            await _groupRepository.CreateGroupAsync(group);
        }


        public async Task<bool> AddUsersToGroupAsync(Joingrp j)
        {
            return await _groupRepository.AddUsersToGroupAsync(j);
        }

        //Get all groups
        public async Task<IEnumerable<Grp>> GetUserGroupMessagesAsync(string username)
        {
            return await _groupRepository.GetUserGroupMessagesAsync(username);
        }


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


        public async Task SendGrpMessageAsync(string groupname, Grpmsg messages)
        {
            await _groupRepository.AddGrpChatAsync(groupname, messages);
        }

        public async Task<bool> EditGMessageAsync(string groupname, string messageId, string newMessage)
        {
            return await _groupRepository.EditGChatAsync(groupname, messageId, newMessage);
        }
    }
}