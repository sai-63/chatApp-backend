﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using login.Common.Models;
using Service;
using MongoDB.Bson;

namespace login.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IUserService _userService;
        private readonly IGroupService _groupService;

        public ChatController(IChatService chatService, IUserService userService,IGroupService groupService)
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
        public async Task<IEnumerable<Chat>> GetIndividualMessages(String senderId,String receiverId)
        {
            return await _chatService.getIndividualMessages(senderId, receiverId);
        }

        // Other methods for getting chats by sender/receiver id can be implemented similarly
        

        //Profile Update
        [HttpPost]
        [Route("ProfileUpdate")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] Userupdateprofile model)
        {
            // Update user profile using UserService
            var result = await _userService.UpdateUserProfileAsync(model.UserId, model.Email, model.Username);

            if (result)
            {
                return Ok(new { success = true, message = "Profile updated successfully" });
            }

            return NotFound("User not found");
        }


        //Group CHAT API"S

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
        public async Task<IActionResult> AddUsersToGroup(string groupId, List<string> userIds)
        {
            // Validate group and user IDs
            if (string.IsNullOrWhiteSpace(groupId) || userIds == null || userIds.Count == 0)
            {
                return BadRequest("Invalid group or user IDs");
            }

            // Add users to the group
            await _groupService.AddUsersToGroupAsync(groupId, userIds);

            return Ok("Users added to the group successfully.");
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





    }
}
