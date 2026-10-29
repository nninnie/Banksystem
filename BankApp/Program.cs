using BankApp.DataAccessLayer.Data;
using BankApp.DataAccessLayer.Models;
using BankApp.Services.Services;
using BankApp.Mapping;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BankApp.Services.Interfaces;

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
            builder.Services.AddDbContext<BankAppContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add Identity services and configure options
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));

            // Configures ASP.NET Core Identity with roles and EF Core as the storage provider
            builder.Services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

            // Registers Services so it can be injected into PageModels
            builder.Services.AddScoped<IDashboardService, DashboardService>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<ITransactionService, TransactionService>();
            builder.Services.AddScoped<IAccountService, AccountService>();

            // Registers the Response Caching services in the DI container
            builder.Services.AddResponseCaching();

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
