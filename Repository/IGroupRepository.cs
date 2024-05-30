using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using login.Common.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver.Core.Servers;
namespace Repository
{
    public interface IGroupRepository
    {
        Task<string> GetUnameAsync(string userId);
        Task<Grp> GetGroupByNameAsync(string groupName);
        Task CreateGroupAsync(Grp group);
        Task<bool> AddUsersToGroupAsync(Joingrp j);

        Task<List<string>> GetallgrpsAsync(string username);
        Task<Grp> GetUserGroupMessagesAsync(string groupname);
        Task<Grp> FullDetOfGroupAsync(string groupname);
        Task<IEnumerable<Grpmsg>> GetGroupMessagesAsync(string groupname);
        Task<IEnumerable<List<string>>> GetUsersOfGroupAsync(string groupname);
        Task<bool> RemoveFromGroupAsync(Joingrp j);
        Task<IEnumerable<string>> GetgroupidAsync(string gname);

        Task AddGrpChatAsync(string groupname, Grpmsg gm);
        Task<bool> EditGChatAsync(string groupname, string messageId, string newMessage);
        Task<Dictionary<string, string>> Getnameid();
        Task<bool> DeleteGrpMessageAsync(string groupname, string messageId);
        Task<bool> DeleteGrpForMeAsync(string groupname, string messageId);
    }
}