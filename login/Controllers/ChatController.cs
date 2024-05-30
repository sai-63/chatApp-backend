using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using login.Common.Models;
using Service;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;

namespace login.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IGroupService _groupService;
        private readonly IUserService _userService;

        public ChatController(IChatService chatService, IGroupService groupService, IUserService userService)
        {
            _chatService = chatService;
            _groupService = groupService;
            _userService = userService;
        }

        [HttpGet]
        [Route("Get All Messages")]
        public async Task<IEnumerable<Chat>> GetAllChats()
        {
            return await _chatService.GetAllChatsAsync();
        }

        [HttpPost]
        [Route("Send Message")]
        public async Task<IActionResult> SendMessage([FromForm] Chatform fd)
        {
            Chat message = new Chat();
            //message.Id = fd.Id;
            message.SenderId = fd.SenderId;
            message.ReceiverId = fd.ReceiverId;
            message.Message = fd.Message;
            message.MessageId = fd.MessageId;
            message.Timestamp = fd.Timestamp;


            if (fd.File != null)
            {
                await _chatService.SendMessageWithFileAsync(message, fd.File);
            }
            else
            {
                await _chatService.SendMessageAsync(message);
            }


            return Ok("Message sent successfully.");
        }

        [HttpPost]
        [Route("DownloadFile")]
        public async Task<IActionResult> DownloadFile([FromBody] Chat chatMessage)
        {
            try
            {
                if (chatMessage == null)
                {
                    return BadRequest("Chat message data is missing.");
                }

                // Check if the message has file content
                if (chatMessage.FileContent == null)
                {
                    return BadRequest("Message does not contain file content.");
                }

                // Determine the content type based on the file type


                // Return the file content with the appropriate content type
                return File(chatMessage.FileContent, chatMessage.FileType, chatMessage.FileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing the request.");
            }
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
        public async Task<IActionResult> DeleteMessage(String messageId)
        {
            var result = await _chatService.DeleteMessageAsync(messageId);
            if (result)
            {
                return Ok("Message deleted successfully.");
            }
            return BadRequest("Couldn't delete");
        }

        [HttpPost]
        [Route("DeleteMessageForMe")]
        public async Task<IActionResult> DeleteMessageForMe(String messageId)
        {
            var result = await _chatService.DeleteMessageForMeAsync(messageId);
            if (result)
            {
                return Ok("Message deleted successfully.");
            }
            return BadRequest("Couldn't delete");
        }

        [HttpPost]
        [Route("DeleteGrpMessage")]
        public async Task<IActionResult> DeleteGrpMessage(string groupname, string messageId)
        {
            // Create a filter to find the group by its name
            var res=await _groupService.DeleteGrpMessageAsync(groupname,messageId);
            if (res)
            {
                return Ok("Deleted bro");
            }
            return BadRequest("i cant");
        }

        [HttpPost]
        [Route("DeleteGrpForMe")]
        public async Task<IActionResult> DeleteGrpForMe(string groupname, string messageId)
        {
            var res = await _groupService.DeleteGrpForMeAsync(groupname, messageId);
            if (res)
            {
                return Ok("Message deleted for you.");
            }
            return BadRequest("Unable to delete message.");
        }


        [HttpPost]
        [Route("EditMessage")]
        public async Task<IActionResult> EditMessage(String messageId, String newMessage)
        {
            var result = await _chatService.EditMessageAsync(messageId, newMessage);
            if (result)
            {
                return Ok("Message edited successfully.");
            }
            return BadRequest("Couldn't edit");
        }

        [HttpPost("markasread")]
        public async Task<IActionResult> MarkAsRead(List<String> messageIds)
        {
            await _chatService.MarkAsRead(messageIds);
            return Ok();
        }


        // Other methods for getting chats by sender/receiver id can be implemented similarly
        [HttpGet]
        [Route("Getnamebyid")]
        public async Task<IActionResult> Getnamebyid(string userId)
        {
            var nname = await _groupService.GetUNameAsync(userId);
            return Ok(nname);
        }

        [HttpGet]
        [Route("Getallgrps")]
        public async Task<List<string>> Getallgrps(string username)
        {
            return await _groupService.GetallgrpsAsync(username);
        }
        [HttpGet]
        [Route("GetUserGroupMessages")]
        public async Task<IActionResult> GetUserGroupMessages(string groupname)
        {
            try
            {
                var groupMessages = await _groupService.GetUserGroupMessagesAsync(groupname);
                return Ok(groupMessages);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
        [HttpGet]
        [Route("Getgroupid")]
        public async Task<IEnumerable<string>> Getgroupid(string gname)
        {
            return await _groupService.GetgroupidAsync(gname);
        }
        [HttpGet]
        [Route("FullDetOfGroup")]
        public async Task<IActionResult> FullDetOfGroup(string groupname)
        {
            try
            {
                var group = await _groupService.FullDetOfGroupAsync(groupname);
                return Ok(group);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpPost]
        [Route("EditGroupMessage")]
        public async Task<IActionResult> EditGroupMessage(string groupname, string messageId, string newMessage)
        {
            var res = await _groupService.EditGMessageAsync(groupname, messageId, newMessage);
            if (res) { return Ok("Edited bro"); }
            else { return Ok("Not edited dude"); }


        }

        [HttpPost]
        [Route("Create Group")]
        public async Task CreateGroup(Grp group)
        {
            await _groupService.CreateGroupAsync(group);
            //return Ok("done");
        }

        [HttpPost]
        [Route("Join Group")]
        public async Task<IActionResult> AddUsersToGroup(Joingrp grp)
        {
            var res=await _groupService.AddUsersToGroupAsync(grp);
            if (res) { return Ok("User added"); } else { return Ok("User not added"); }
        }

        [HttpGet]
        [Route("Getnameforid")]
        public async Task<IActionResult> Getnameforid()
        {
            var res=await _groupService.GetnameforidAsync();
            return Ok(res);
        }

        [HttpPost]
        [Route("SendGrpMessage")]
        public async Task<IActionResult> SendGrpMessage(string groupname,[FromForm] Groupform gf)
        {
            if (string.IsNullOrWhiteSpace(gf.SenderId) || string.IsNullOrWhiteSpace(gf.Message))
            {
                return BadRequest("SenderId and Message are required.");
            }

            var gmessage = new Grpmsg
            {
                SenderId = gf.SenderId,
                Message = gf.Message,
                Timestamp = gf.Timestamp
            };

            if (gf.File != null)
            {
                await _groupService.SendGrpMessageWithFileAsync(groupname,gmessage, gf.File);
            }
            else
            {
                await _groupService.SendGrpMessageAsync(groupname,gmessage);
            }


            return Ok("Message sent successfully.");
        }

        

    }
}
