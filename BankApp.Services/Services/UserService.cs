using BankApp.DataAccessLayer.DTOs;
using BankApp.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Services.Services
{
    // Service class – handles all system user logic (uses Identity's UserManager)
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;

        // The seeded accounts are protected, so nobody can lock themselves (or the teacher) out
        private static readonly string[] ProtectedEmails =
        {
            "robert.johansson@admin.se",
            "robert.johansson@cashier.se"
        };

        public UserService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        private static bool IsProtected(string? email) =>
            ProtectedEmails.Contains(email ?? "", StringComparer.OrdinalIgnoreCase);

        // Returns all system users with their role (every user has one role)
        public async Task<List<UserDto>> GetUsersAsync()
        {
            var users = await _userManager.Users.OrderBy(u => u.Email).ToListAsync();

            var result = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                result.Add(new UserDto
                {
                    Id = user.Id,
                    Email = user.Email ?? "",
                    Role = roles.FirstOrDefault() ?? "No role",
                    IsProtected = IsProtected(user.Email)
                });
            }

            return result;
        }

        // Fetches one user with role, or null if it does not exist
        public async Task<UserDto?> GetUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email ?? "",
                Role = roles.FirstOrDefault() ?? "No role",
                IsProtected = IsProtected(user.Email)
            };
        }

        // Creates a user and gives it a role, returns error messages (empty list = success)
        public async Task<List<string>> CreateUserAsync(string email, string password, string role)
        {
            var user = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
                return result.Errors.Select(e => e.Description).ToList();

            var roleResult = await _userManager.AddToRoleAsync(user, role);

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return roleResult.Errors.Select(e => e.Description).ToList();
            }

            return new List<string>();
        }

        // Updates email, role and (if given) a new password, returns error messages (empty list = success)
        public async Task<List<string>> UpdateUserAsync(string id, string email, string role, string? newPassword)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return new List<string> { "User not found." };

            if (IsProtected(user.Email))
                return new List<string> { "The seeded accounts can not be changed." };

            user.Email = email;
            user.UserName = email;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return result.Errors.Select(e => e.Description).ToList();

            var currentRoles = await _userManager.GetRolesAsync(user);

            if (!currentRoles.Contains(role))
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, role);
            }

            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, newPassword);

                if (!passwordResult.Succeeded)
                    return passwordResult.Errors.Select(e => e.Description).ToList();
            }

            return new List<string>();
        }


        // Deletes a user, returns error messages 
        public async Task<List<string>> DeleteUserAsync(string id, string? currentUserName)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return new List<string> { "User not found." };

            if (IsProtected(user.Email))
                return new List<string> { "The seeded accounts can not be deleted." };

            if (user.UserName == currentUserName)
                return new List<string> { "You can not delete your own account." };

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
                return result.Errors.Select(e => e.Description).ToList();

            return new List<string>();
        }
    }
}