using Microsoft.AspNetCore.Identity;

namespace BankApp.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // Defines the roles that must be created during seeding.
            string[] roles = { "Admin", "Cashier" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Admin
            string adminEmail = "robert.johansson@admin.se";
            string adminPassword = "Abc123#";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(adminUser, adminPassword);
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Cashier
            string cashierEmail = "robert.johansson@cashier.se";
            string cashierPassword = "Abc123#";

            if (await userManager.FindByEmailAsync(cashierEmail) == null)
            {
                var cashierUser = new IdentityUser
                {
                    UserName = cashierEmail,
                    Email = cashierEmail,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(cashierUser, cashierPassword);
                await userManager.AddToRoleAsync(cashierUser, "Cashier");
            }
        }
    }
}