using BankApp.DataAccessLayer.DTOs;

namespace BankApp.Services.Interfaces
{
    // Contract for administration of system users (Admin and Cashier)
    public interface IUserService
    {
        Task<List<UserDto>> GetUsersAsync();

        Task<List<string>> CreateUserAsync(string email, string password, string role);

        Task<UserDto?> GetUserAsync(string id);

        Task<List<string>> UpdateUserAsync(string id, string email, string role, string? newPassword);

        Task<List<string>> DeleteUserAsync(string id, string? currentUserName);
    }
}