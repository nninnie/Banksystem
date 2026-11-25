using System.Globalization;
using System.Security.Claims;

namespace BankApp.Helpers
{
    // Helper – one place that turns the logged-in user into a display name and role
    public static class UserDisplay
    {
        public static string Name(ClaimsPrincipal user)
        {
            if (user.Identity?.Name == null)
                return "Guest";

            var raw = user.Identity.Name.Split('@')[0].Replace(".", " ");
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(raw);
        }

        public static string Role(ClaimsPrincipal user) =>
            user.IsInRole("Admin") ? "Admin"
            : user.IsInRole("Cashier") ? "Cashier"
            : "Visitor";
    }
}