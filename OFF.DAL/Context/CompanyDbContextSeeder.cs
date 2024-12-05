using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using OFF.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFF.DAL.Context
{
    public class CompanyDbContextSeeder
    {
        private readonly CompanyDbContext _context;

        public CompanyDbContextSeeder(CompanyDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (!_context.Users.Any())
            {
                var adminUser = new AppUser { UserName = "Nehal", Email = "offside.team2024@gmail.com" };

                var passwordHasher = new PasswordHasher<AppUser>();
                var hashedPassword = passwordHasher.HashPassword(adminUser, "Offside@24"); // Use a strong password
                adminUser.PasswordHash = hashedPassword;

                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                }

                await userManager.CreateAsync(adminUser);
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}
