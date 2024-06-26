﻿// Repositories/IUserRepository.cs

using login.Common.Models;
using System.Threading.Tasks;

namespace Repository
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByUsernameAsync(string username);
        Task AddUserAsync(User user);
        Task<List<User>> GetAllUsersAsync(string username);
        Task<bool> addFriendAsync(string userId,string friendId);
        Task<bool> isFriendAsync(string userId, string friendId);
        Task<List<User>> GetAllFriendsAsync(string userId);
        Task setUserOnlineAsync(string userName);
        Task setUserOfflineAsync(string userName,DateTime time);
    }
}