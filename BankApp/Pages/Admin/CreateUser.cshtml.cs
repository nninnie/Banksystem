using BankApp.Services.Interfaces;
using BankApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankApp.Pages.Admin
{
    // PageModel – create a new system user 
    [Authorize(Roles = "Admin")]
    public class CreateUserModel : PageModel
    {
        private readonly IUserService _userService;

        public CreateUserModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public CreateUserViewModel Input { get; set; } = new();

        public void OnGet()
        {
        }

        // POST – form sent: validate, create the user, then go back to the list
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var errors = await _userService.CreateUserAsync(Input.Email, Input.Password, Input.Role);

            if (errors.Count == 0)
                return RedirectToPage("/Admin/UserAdmin");

            foreach (var error in errors)
                ModelState.AddModelError(string.Empty, error);

            return Page();
        }
    }
}