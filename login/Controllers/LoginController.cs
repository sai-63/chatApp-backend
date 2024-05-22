using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Service;
using login.Common.Models;
using MongoDB.Bson;

namespace login.Controllers
{
    [ApiController]
    public class SignupController : ControllerBase
    {
        private readonly IUserService _userService;

        public SignupController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<User>> Login(User user)
        {
            var user1 = await _userService.ValidateUserAsync(user.Username, user.Password);

            // Authentication successful
            // You can generate and return a JWT token here

            return Ok(user1);
        }

        [HttpPost]
        [Route("SignUp")]
        public async Task<ActionResult> Signup([FromBody] User user)
        {
            var result = await _userService.SignupAsync(user.Username, user.Email, user.Password,user.Nickname,user.Friends);
            if (result)
            {
                return Ok("User signed up successfully.");
            }
            return BadRequest("Failed to sign up user.");
        }

        [HttpGet]
        [Route("GetObjectIdOfUser")]
        public async Task<ActionResult<String>> GetUserId(String username)
        {
            return await _userService.getUserIdAsync(username);
        }


        [HttpGet]
        [Route("GetOtherUsers")]
        public async Task<ActionResult<List<User>>> GetUsers(string username)
        {
            return await _userService.getAllUsersAsync(username);
        }

        [HttpPost]
        [Route("AddFriend")]
        public async Task<ActionResult> AddFriend(Friend friend)
        {
            var check = await _userService.IsFriend(friend.userId, friend.friendId);
            if(check)
            {
                return Ok("Already a friend");
            }

            var result = await _userService.addFriend(friend.userId,friend.friendId);
            if (result)
            {
                return Ok("Friend added successfully.");
            }
            return BadRequest("Failed to add as a friend.");
        }

        [HttpGet]
        [Route("GetFriends")]
        public async Task<ActionResult<List<User>>> GetFriends(string userId)
        {
            return await _userService.getAllFriendsAsync(userId);
        }

    }
}
