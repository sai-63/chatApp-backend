using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using login.Common.Models;
using Service;
using MongoDB.Bson;
using Common.Models;
using System.Text.RegularExpressions;
using Group = login.Common.Models.Group;
namespace login.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IUserService _userService;
        private readonly IGroupService _groupService;

        public ChatController(IChatService chatService, IUserService userService, IGroupService groupService)
        {
            _chatService = chatService;
            _userService = userService;
            _groupService = groupService;
        }

        [HttpGet]
        [Route("Get All Messages")]
        public async Task<IEnumerable<Chat>> GetAllChats()
        {
            return await _chatService.GetAllChatsAsync();
        }

        [HttpPost]
        [Route("Send Message")]
        public async Task<IActionResult> SendMessage(Chat message)
        {
            await _chatService.SendMessageAsync(message);
            return Ok("Message sent successfully.");
        }

        [HttpGet]
        [Route("GetMessagesSenderIdUserId")]
        public async Task<IActionResult> GetIndividualMessages(string senderId, string receiverId)
        {
            var result = await _chatService.getIndividualMessages(senderId, receiverId);
            return Ok(result); // Returns the result as a JSON response
        }


        [HttpPost]
        [Route("DeleteMessage")]
        public async Task<IActionResult> DeleteMessage(String id)
        {
            var result = await _chatService.DeleteMessageAsync(id);
            if (result)
            {
                return Ok("Message deleted successfully.");
            }
            return BadRequest("Couldn't delete");
        }


        // Other methods for getting chats by sender/receiver id can be implemented similarly


        //Group Chat API'S

        [HttpPost]
        [Route("CreateGroup")]
        public async Task<IActionResult> CreateGroup(Group group)
        {
            // Validate group data
            //if (group == null || string.IsNullOrWhiteSpace(group.Name) || group.Users == null || group.Users.Count == 0)
            //{return BadRequest("Invalid group data");}

            // Check if the group already exists
            //var existingGroup = await _chatService.GetGroupByNameAsync(group.Name);
            //if (existingGroup != null) {return Conflict("Group with the same name already exists");}

            // Create the group
            await _groupService.CreateGroupAsync(group);

            return Ok("Group created successfully.");
        }

        [HttpPost]
        [Route("AddUsersToGroup")]
        public async Task<IActionResult> AddUsersToGroup(Joingrp j)
        {

            // Add users to the group
            var adduser = await _groupService.AddUsersToGroupAsync(j);
            if (adduser)
            {
                return Ok("Users added to the group successfully.");
            }
            else
            {
                //return Ok($"{username} already in Group {groupname}");
                return Ok("Already");
            }
        }

        //Get all groups
        [HttpGet]
        [Route("GetAllGroups")]
        public async Task<IEnumerable<Group>> GetAllGroups()
        {
            return await _groupService.GetAllGroups();
        }

        [HttpGet]
        [Route("GetUserGroups")]
        public async Task<ActionResult<IEnumerable<Group>>> GetUserGroups(string username)
        {
            var userGroups = await _groupService.GetAllGroups(); // Fetch all groups
            var userGroupsWithMessages = userGroups.Where(g => g.Users.Contains(username)); // Filter groups with messages from the current user
            return Ok(userGroupsWithMessages);
        }

        [HttpGet]
        [Route("GetGroupMessages")]
        public async Task<ActionResult<IEnumerable<New>>> GetGroupMessages(string groupname)
        {
            //if (string.IsNullOrWhiteSpace(groupid))
            //{
            //  return BadRequest("Group ID cannot be null or empty");
            //}

            var groupMessages = await _groupService.GetGroupMessagesAsync(groupname); // Fetch messages for the group

            return Ok(groupMessages);
        }

        [HttpGet]
        [Route("GetUserOfGroup")]
        public async Task<ActionResult<IEnumerable<List<String>>>> GetUsersOfGroup(string groupname)
        {
            var grusers = await _groupService.GetUsersOfGroupAsync(groupname);
            return Ok(grusers);
        }

        [HttpPost]
        [Route("RemoveUserFromGroup")]
        public async Task<IActionResult> RemoveUserFromGroup(Joingrp j)
        {

            // Remove user from the group
            var removeuser = await _groupService.RemoveFromGroupAsync(j);
            if (removeuser)
            {
                return Ok("User removed from the group successfully.");
            }
            else
            {
                //No such user in grp
                return Ok("No such user in the group");
            }
        }
    }
}
