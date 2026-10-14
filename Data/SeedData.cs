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
            var config = serviceProvider.GetRequiredService<IConfiguration>();

            // Hämta lösenord och email från appsettings.json
            string adminEmail = config["SeedUsers:AdminEmail"];
            string adminPassword = config["SeedUsers:AdminPassword"];
            string cashierEmail = config["SeedUsers:CashierEmail"];
            string cashierPassword = config["SeedUsers:CashierPassword"];

            // Roller som ska seedas
            string[] roles = { "Admin", "Cashier" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Admin
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

