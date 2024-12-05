using Microsoft.AspNetCore.Identity;
using OFF.DAL.Model;

namespace OFF.DAL.Context
{
    public static class IdentityContextSeedcs
    {

        public static async Task SeedUserAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var adminUserName = "NehalAbdelrahman"; 
            var adminEmail = "offside.team@gmail.com"; 
            var adminPassword = "OFFside@24"; // Use a stron

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            var adminUser = new AppUser
            {
                FName="Nehal",
                LName="Abdelrahman",
                UserName = adminUserName,
                Email = adminEmail,
                Gender="Female",
                Nationality="Egyptian",
                BirthDate = new DateTime(1990, 1, 1),

            };

            var createAdminUserResult = await userManager.CreateAsync(adminUser, adminPassword);

            if (createAdminUserResult.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
            else
            {
            }
        }

    }

}

