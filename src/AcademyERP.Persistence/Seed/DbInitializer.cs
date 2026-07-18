using AcademyERP.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using AcademyERP.Application.Constants;

namespace AcademyERP.Persistence.Seed;

public static class DbInitializer
{
    public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        var roles = new[]
{
    Roles.SuperAdmin,
    Roles.Administrator,
    Roles.Teacher,
    Roles.Parent,
    Roles.Student,
    Roles.Accountant,
    Roles.HR
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

            await userManager.AddToRoleAsync(admin, Roles.SuperAdmin);
        }
    }
}