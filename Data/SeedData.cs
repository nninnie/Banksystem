using Microsoft.AspNetCore.Identity;

namespace BankApp.Data
{
    // SeedData class automatically creates required roles and system users (Admin and Cashier) at startup if they do not already exist.
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string[] roles = { "Admin", "Cashier" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminUser = new IdentityUser
            {
                UserName = "robert.johansson@admin.se",
                Email = "robert.johansson@admin.se",
                EmailConfirmed = true
            };

            if (await userManager.FindByEmailAsync(adminUser.Email) == null)
            {
                await userManager.CreateAsync(adminUser, "Admin123#");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            var cashierUser = new IdentityUser
            {
                UserName = "robert.johansson@cashier.se",
                Email = "robert.johansson@cashier.se",
                EmailConfirmed = true
            };

            if (await userManager.FindByEmailAsync(cashierUser.Email) == null)
            {
                await userManager.CreateAsync(cashierUser, "Abc123#");
                await userManager.AddToRoleAsync(cashierUser, "Cashier");
            }
        }
    }
}

