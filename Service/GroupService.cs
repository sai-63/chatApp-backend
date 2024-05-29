using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using login.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using MongoDB.Bson;
using Repository;

namespace Service
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IUserService _userService;

        public GroupService(IGroupRepository groupRepository,IUserService userService)
        {
            _groupRepository = groupRepository;
            _userService = userService;
        }

        public async Task<string> GetUNameAsync(string userId)
        {
            return await _groupRepository.GetUnameAsync(userId);
        }
        public async Task<List<string>> GetallgrpsAsync(string username)
        {
            return await _groupRepository.GetallgrpsAsync(username);
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
        public async Task<IDictionary<string, List<Grpmsg>>>GetUserGroupMessagesAsync(string groupname)
        {
            //return await _groupRepository.GetUserGroupMessagesAsync(groupname);
            var group = await _groupRepository.GetUserGroupMessagesAsync(groupname);            
            var groupedMessages = group.Messages
                .GroupBy(msg => msg.Timestamp.Date.ToString("yyyy-MM-dd")) // Group by date
                .ToDictionary(
                    group => group.Key,
                    group => group.ToList() // Sort messages by time and convert to list
                );           
            return groupedMessages;
        }
        public async Task<Grp> FullDetOfGroupAsync(string groupname)
        {
            return await _groupRepository.FullDetOfGroupAsync(groupname);
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

        public async Task SendGMessageWithFileAsync(string groupname,Grpmsg gmessage, IFormFile file)
        {
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                gmessage.FileContent = ms.ToArray();
                gmessage.FileName = (string)(file.FileName);
                gmessage.FileType = file.ContentType;
                gmessage.FileSize = file.Length;
            }


            // Save the chat message (with file) to the database
            await _groupRepository.AddGrpChatAsync(groupname,gmessage);
        }

        public async Task<bool> EditGMessageAsync(string groupname, string messageId, string newMessage)
        {
            return await _groupRepository.EditGChatAsync(groupname, messageId, newMessage);
        }
        public async Task<Dictionary<string, string>> GetnameforidAsync()
        {
            return await _groupRepository.Getnameid();
        }

        public async Task SendGrpMessageWithFileAsync(string groupname,Grpmsg gmessage, IFormFile file)
        {
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                gmessage.FileContent = ms.ToArray();
                gmessage.FileName = (string)(file.FileName);
                gmessage.FileType = file.ContentType;
                gmessage.FileSize = file.Length;
            }


            // Save the chat message (with file) to the database
            await _groupRepository.AddGrpChatAsync(groupname,gmessage);
        }
    }
}