using System.Threading.Tasks;
using login.Common.Models;
using Repository;

namespace Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> ValidateUserAsync(string username, string password)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user == null)
            {
                return new User();
            }
            return user;
        }

        public async Task<bool> SignupAsync(User user)
        {
            // Check if username or email already exists
            var existingUser = await _userRepository.GetUserByUsernameAsync(user.Username);
            if (existingUser != null)
            {
                return false; // Username already exists
            }

            existingUser = await _userRepository.GetUserByEmailAsync(user.Email);
            if (existingUser != null)
            {
                return false; // Email already exists
            }

            await _userRepository.AddUserAsync(user);
            return true;
        }

        public async Task<List<User>> getAllUsersAsync(string username)
        {
            return await _userRepository.GetAllUsersAsync(username);
        }

        public async Task<string> getUserIdAsync(string username)
        {
            User user = await _userRepository.GetUserByUsernameAsync(username);
            return user.Id;
        }

        public async Task<bool> addFriend(string userId, string friendId)
        {
            var result = await _userRepository.addFriendAsync(userId,friendId);
            if (result)
            {
                return true;
            }
            return false;
        }

        public async Task<List<User>> getAllFriendsAsync(string userId)
        {
            return await _userRepository.GetAllFriendsAsync(userId);
        }

        public async Task<bool> IsFriend(string userId, string friendId)
        {
            var check= await _userRepository.isFriendAsync(userId, friendId);
            if (check)
            {
                return true;
            }
            return false;
        }

        public async Task setUserOnline(string userName)
        {
            await _userRepository.setUserOnlineAsync(userName);
        }
        public async Task setUserOffline(string userName,DateTime time)
        {
            await _userRepository.setUserOfflineAsync(userName,time);
        }
    }
}
