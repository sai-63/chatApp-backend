// Services/IUserService.cs

using System.Threading.Tasks;
using login.Common.Models;

namespace Service
{
    public interface IUserService
    {
        Task<User> ValidateUserAsync(string username, string password);
        Task<bool> SignupAsync(User user);
        Task<bool> IsFriend(string userId,string friendId);
        Task<List<User>> getAllUsersAsync(string username);
        Task<string> getUserIdAsync(string username);
        Task<bool> addFriend(string userId,string friendId);
        Task<List<User>> getAllFriendsAsync(string userId);
        Task setUserOnline(string userName);
        Task setUserOffline(string userName,DateTime time);
    }
}