using BankApp.Helpers;
using BankApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace BankApp.Pages
{
    // LoginModel for responsible for handling user login
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public LoginModel(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [BindProperty]
        public LoginViewModel Input { get; set; }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
  
            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.FindByNameAsync(Input.Username);

            if (user != null)
            {
                var check = await _signInManager.CheckPasswordSignInAsync(user, Input.Password, lockoutOnFailure: false);

                if (check.Succeeded)
                {
                    var loginTime = SwedishTime.Now;

                    var claims = new[] { new Claim("LoginTime", loginTime.ToString("yyyy-MM-dd HH:mm")) };

                    await _signInManager.SignInWithClaimsAsync(user, isPersistent: false, claims);

                    TempData["Welcome"] = true;

                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                        return RedirectToPage("/Admin/Dashboard");

                    return RedirectToPage("/Admin/Customers");
                }
            }

            ErrorMessage = "Invalid username or password.";
            return Page();
        }
    }
}
