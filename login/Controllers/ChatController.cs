using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using login.Common.Models;
using Service;

namespace login.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IUserService _userService;

        public ChatController(IChatService chatService, IUserService userService)
        {
            _chatService = chatService;
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
    }
}
