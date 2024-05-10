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

        public async Task<bool> SignupAsync(string username, string email, string password)
        {
            // Check if username or email already exists
            var existingUser = await _userRepository.GetUserByUsernameAsync(username);
            if (existingUser != null)
            {
                return false; // Username already exists
            }

            existingUser = await _userRepository.GetUserByEmailAsync(email);
            if (existingUser != null)
            {
                return false; // Email already exists
            }

            // Create new user
            var newUser = new User
            {
                Username = username,
                Email = email,
                Password = password // You should hash the password before storing it
            };

            await _userRepository.AddUserAsync(newUser);
            return true;
        }

        public async Task<List<User>> getAllUsersAsync(string id)
        {
            return await _userRepository.GetAllUsersAsync(id);
        }

        public async Task<string> getUserIdAsync(string username)
        {
            User user = await _userRepository.GetUserByUsernameAsync(username);
            return user.Id;
        }
    }
}
