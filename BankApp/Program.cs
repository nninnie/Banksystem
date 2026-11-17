using BankApp.DataAccessLayer.Data;
using BankApp.DataAccessLayer.Models;
using BankApp.Services.Services;
using BankApp.Mapping;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BankApp.Services.Interfaces;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace BankApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddRazorPages();

            // Register AutoMapper (loads MappingProfile automatically)
            builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

            // Add database context, Dependency Injection, and configure the connection string
            // Long timeout - the suspicious search reads over a million transactions
            builder.Services.AddDbContext<BankAppContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    sql => sql.CommandTimeout(120)));

            // Add Identity services and configure options
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));

            // Configures ASP.NET Core Identity with roles, without the built-in UI pages (no public registration)
            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;

                // Same key length as my existing migration (AddDefaultIdentity used 128)
                options.Stores.MaxLengthForKeys = 128;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Sends users who are not logged in to my own login page
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Login";
                options.AccessDeniedPath = "/Index";
            });

            // Registers Services so it can be injected into PageModels
            builder.Services.AddScoped<IDashboardService, DashboardService>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<ITransactionService, TransactionService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ISuspiciousTransactionService, SuspiciousTransactionService>();

            // Registers the Response Caching services in the DI container
            builder.Services.AddResponseCaching();

            builder.Services.AddMemoryCache();

            var app = builder.Build();

            // Applies Identity migrations and seeds roles and default users at application startup
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                // Creates the Identity tables if they are missing (does nothing if they already exist)
                var identityDb = services.GetRequiredService<ApplicationDbContext>();
                await identityDb.Database.MigrateAsync();

                await SeedData.InitializeAsync(services);
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Shows numbers, dates and currency in Swedish format (5 369 and 41 833 191,90) also in Azure
            var swedish = new CultureInfo("sv-SE");
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(swedish),
                SupportedCultures = new[] { swedish },
                SupportedUICultures = new[] { swedish }
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            // Enables the Response Caching middleware in the request pipeline
            app.UseResponseCaching();

            app.UseAuthentication();

            // Applies authorization rules for protected pages and roles
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            app.Run();
        }
    }
}
