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
        public async Task<IEnumerable<Chat>> GetIndividualMessages(String senderId,String receiverId)
        {
            return await _chatService.getIndividualMessages(senderId, receiverId);
        }

        // Other methods for getting chats by sender/receiver id can be implemented similarly
    }
}
