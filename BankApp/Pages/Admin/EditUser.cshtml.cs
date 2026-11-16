using AutoMapper;
using BankApp.Services.Interfaces;
using BankApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankApp.Pages.Admin
{
    // PageModel – edit a system user (Admin only)
    [Authorize(Roles = "Admin")]
    public class EditUserModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public EditUserModel(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        // Filled with the user's current values on GET, typed in the form on POST
        [BindProperty]
        public EditUserViewModel Input { get; set; } = new();

        // GET – page opens: load the user and fill the form
        public async Task<IActionResult> OnGetAsync(string id)
        {
            var user = await _userService.GetUserAsync(id);

            if (user == null)
                return NotFound();

            // The seeded accounts can not be changed
            if (user.IsProtected)
                return RedirectToPage("/Admin/UserAdmin");

            Input = _mapper.Map<EditUserViewModel>(user);

            return Page();
        }

        // POST – form sent: validate, update, then go back to the list
        public async Task<IActionResult> OnPostAsync(string id)
        {
            var user = await _userService.GetUserAsync(id);

            if (user == null)
                return NotFound();

            // The id in the address is the truth, not a field in the form
            Input.Id = id;

            // An admin can not take the admin role from himself, he would lock himself out
            if (user.Email == User.Identity?.Name && Input.Role != user.Role)
                ModelState.AddModelError("Input.Role", "You can not change your own role.");

            // Server-side validation
            if (!ModelState.IsValid)
                return Page();

            var errors = await _userService.UpdateUserAsync(id, Input.Email, Input.Role, Input.NewPassword);

            if (errors.Count == 0)
                return RedirectToPage("/Admin/UserAdmin");

            // Errors from Identity (weak password, email already used) are shown above the form
            foreach (var error in errors)
                ModelState.AddModelError(string.Empty, error);

            return Page();
        }
    }
}