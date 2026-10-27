using BankApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankApp.Pages
{
    // LoginModel for responsible for handling user loggin
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;

        public LoginModel(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [BindProperty]
        public LoginViewModel Input { get; set; }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            // If fields are empty or invalid, stop here and return the page with error messages
            if (!ModelState.IsValid)
            {
                
                return Page();
            }

            TempData["LastLoginTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

            var result = await _signInManager.PasswordSignInAsync(
                Input.Username,
                Input.Password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToPage("/Index");
            }

            ErrorMessage = "Fel användarnamn eller lösenord.";
            return Page();
        }
    }
}
