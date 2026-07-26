using AcademyERP.Domain.Entities.Identity;
using AcademyERP.Domain.Entities.Lookups;
using AcademyERP.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AcademyERP.Application.Constants;

namespace AcademyERP.Persistence.Seed;

public static class DbInitializer
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        // -------------------------
        // Seed Roles
        // -------------------------

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

        // -------------------------
        // Seed Super Admin
        // -------------------------

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

            var result = await userManager.CreateAsync(
                admin,
                "Admin@123");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(
                    admin,
                    Roles.SuperAdmin);
            }
        }

        // -------------------------
        // Seed Class Durations
        // -------------------------

        if (!await context.ClassDurations.AnyAsync())
        {
            context.ClassDurations.AddRange(
                new ClassDuration
                {
                    Name = "30 Minutes",
                    Minutes = 30,
                    IsActive = true
                },
                new ClassDuration
                {
                    Name = "45 Minutes",
                    Minutes = 45,
                    IsActive = true
                },
                new ClassDuration
                {
                    Name = "1 Hour",
                    Minutes = 60,
                    IsActive = true
                },
                new ClassDuration
                {
                    Name = "2 Hours",
                    Minutes = 120,
                    IsActive = true
                },
                new ClassDuration
                {
                    Name = "3 Hours",
                    Minutes = 180,
                    IsActive = true
                }
            );
        }

        // -------------------------
        // Seed Initial Time Slots
        // -------------------------

        if (!await context.TimeSlots.AnyAsync())
        {
            context.TimeSlots.AddRange(
                new TimeSlot
                {
                    Name = "06:00 AM - 07:00 AM",
                    StartTime = new TimeOnly(6, 0),
                    EndTime = new TimeOnly(7, 0),
                    IsActive = true
                },
                new TimeSlot
                {
                    Name = "07:00 AM - 08:00 AM",
                    StartTime = new TimeOnly(7, 0),
                    EndTime = new TimeOnly(8, 0),
                    IsActive = true
                },
                new TimeSlot
                {
                    Name = "08:00 AM - 09:00 AM",
                    StartTime = new TimeOnly(8, 0),
                    EndTime = new TimeOnly(9, 0),
                    IsActive = true
                },
                new TimeSlot
                {
                    Name = "04:00 PM - 05:00 PM",
                    StartTime = new TimeOnly(16, 0),
                    EndTime = new TimeOnly(17, 0),
                    IsActive = true
                },
                new TimeSlot
                {
                    Name = "05:00 PM - 06:00 PM",
                    StartTime = new TimeOnly(17, 0),
                    EndTime = new TimeOnly(18, 0),
                    IsActive = true
                },
                new TimeSlot
                {
                    Name = "06:00 PM - 07:00 PM",
                    StartTime = new TimeOnly(18, 0),
                    EndTime = new TimeOnly(19, 0),
                    IsActive = true
                },
                new TimeSlot
                {
                    Name = "07:00 PM - 08:00 PM",
                    StartTime = new TimeOnly(19, 0),
                    EndTime = new TimeOnly(20, 0),
                    IsActive = true
                },
                new TimeSlot
                {
                    Name = "08:00 PM - 09:00 PM",
                    StartTime = new TimeOnly(20, 0),
                    EndTime = new TimeOnly(21, 0),
                    IsActive = true
                }
            );
        }

        await context.SaveChangesAsync();
    }
}