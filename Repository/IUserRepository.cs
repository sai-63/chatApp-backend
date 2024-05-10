// Repositories/IUserRepository.cs

using login.Common.Models;
using System.Threading.Tasks;

namespace Repository
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByUsernameAsync(string username);
        Task AddUserAsync(User user);
        Task<List<User>> GetAllUsersAsync(string id);
    }
}