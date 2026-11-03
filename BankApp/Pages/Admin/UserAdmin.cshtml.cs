using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankApp.Pages.Admin
{
    // PageModel – user administration (Admin only).
    [Authorize(Roles = "Admin")]
    public class UserAdminModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}