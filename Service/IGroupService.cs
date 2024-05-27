using login.Common.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace Service
{
    public interface IGroupService
    {
        Task<string> GetUNameAsync(string userId);
        Task<Grp> GetGroupByNameAsync(string groupName);
        Task CreateGroupAsync(Grp group);
        Task<bool> AddUsersToGroupAsync(Joingrp j);
        Task<IEnumerable<Grp>> GetUserGroupMessagesAsync(string username);

        Task<IEnumerable<Grpmsg>> GetGroupMessagesAsync(string groupname);
        Task<IEnumerable<List<string>>> GetUsersOfGroupAsync(string groupname);
        Task<bool> RemoveFromGroupAsync(Joingrp j);
        Task<IEnumerable<string>> GetgroupidAsync(string gname);
        Task SendGrpMessageAsync(string groupname, Grpmsg messages);
        Task<bool> EditGMessageAsync(string groupname, string messageId, string newMessage);
    }
}