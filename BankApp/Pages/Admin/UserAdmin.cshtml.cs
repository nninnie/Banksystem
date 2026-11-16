using AutoMapper;
using BankApp.Services.Interfaces;
using BankApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankApp.Pages.Admin
{
    // PageModel – user administration (Admin only), shows all system users
    [Authorize(Roles = "Admin")]
    public class UserAdminModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserAdminModel(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        // Shown in the table on the page
        public List<UserViewModel> Users { get; set; } = new();

        // Shown above the table if a delete fails
        public string? ErrorMessage { get; set; }

        // GET – page opens: load all system users
        public async Task OnGetAsync()
        {
            await LoadUsersAsync();
        }

        // POST – the Delete button: delete the user, then show the list again
        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            var errors = await _userService.DeleteUserAsync(id, User.Identity?.Name);

            if (errors.Count == 0)
                return RedirectToPage();

            ErrorMessage = string.Join(" ", errors);

            await LoadUsersAsync();
            return Page();
        }

        private async Task LoadUsersAsync()
        {
            var users = await _userService.GetUsersAsync();
            Users = _mapper.Map<List<UserViewModel>>(users);
        }
    }
}