using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using login.Common.Models;
using Service;
using MongoDB.Bson;
using Common.Models;

namespace login.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
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
    }
}
