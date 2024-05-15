// Services/IUserService.cs

using System.Threading.Tasks;
using login.Common.Models;

namespace Service
{
    public interface IUserService
    {
        Task<User> ValidateUserAsync(string username, string password);
        Task<bool> SignupAsync(string username, string email, string password, string nickname, List<string> friends);
        Task<bool> IsFriend(string userId,string friendId);
        Task<List<User>> getAllUsersAsync(string id);
        Task<string> getUserIdAsync(string username);
        Task<bool> addFriend(string userId,string friendId);
        Task<List<User>> getAllFriendsAsync(string id);
    }
}