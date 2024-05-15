using Common.Models;
using login.Common.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Group = login.Common.Models.Group;
namespace Service
{
    public interface IGroupService
    {
        Task<Group> GetGroupByNameAsync(string groupName);
        Task CreateGroupAsync(Group group);
        Task<bool> AddUsersToGroupAsync(Joingrp j);
        Task<IEnumerable<Group>> GetAllGroups();
        //Task<IEnumerable<Group>> GetUserGroupsAsync(string userId);

        Task<IEnumerable<New>> GetGroupMessagesAsync(string groupname);
        Task<IEnumerable<List<String>>> GetUsersOfGroupAsync(string groupname);
        Task<bool> RemoveFromGroupAsync(Joingrp j);


    }
}
