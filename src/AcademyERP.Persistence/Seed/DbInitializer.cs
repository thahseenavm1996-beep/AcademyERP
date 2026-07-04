using AcademyERP.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace AcademyERP.Persistence.Seed;

public static class DbInitializer
{
    public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        string[] roles =
        {
            "Admin",
            "Teacher",
            "Student",
            "Parent"
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new ApplicationRole
                {
                    Name = role
                });
            }
        }

        var adminEmail = "admin@academyerp.com";

        var admin = await userManager.FindByEmailAsync(adminEmail);

        if (admin == null)
        {
            admin = new ApplicationUser
            {
                FullName = "System Administrator",
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                IsActive = true
            };

            await userManager.CreateAsync(admin, "Admin@123");

            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}