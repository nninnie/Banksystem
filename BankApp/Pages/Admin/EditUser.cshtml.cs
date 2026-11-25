using AutoMapper;
using BankApp.Services.Interfaces;
using BankApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankApp.Pages.Admin
{
    // PageModel – edit a system user
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

        [BindProperty]
        public EditUserViewModel Input { get; set; } = new();

        // GET – page opens: load the user and fill the form
        public async Task<IActionResult> OnGetAsync(string id)
        {
            var user = await _userService.GetUserAsync(id);

            if (user == null)
                return NotFound();

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

            Input.Id = id;

            if (user.Email == User.Identity?.Name && Input.Role != user.Role)
                ModelState.AddModelError("Input.Role", "You can not change your own role.");

            if (!ModelState.IsValid)
                return Page();

            var errors = await _userService.UpdateUserAsync(id, Input.Email, Input.Role, Input.NewPassword);

            if (errors.Count == 0)
                return RedirectToPage("/Admin/UserAdmin");

            foreach (var error in errors)
                ModelState.AddModelError(string.Empty, error);

            return Page();
        }
    }
}