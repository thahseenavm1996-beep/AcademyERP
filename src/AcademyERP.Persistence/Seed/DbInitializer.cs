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

        // -------------------------
// Ensure Time Slots
// -------------------------

var requiredTimeSlots = Enumerable.Range(5, 19)
    .Select(hour => new
    {
        StartTime = new TimeOnly(hour % 24, 0),
        EndTime = new TimeOnly((hour + 1) % 24, 0)
    })
    .ToList();

foreach (var slot in requiredTimeSlots)
{
    var exists = await context.TimeSlots.AnyAsync(x =>
        x.StartTime == slot.StartTime &&
        x.EndTime == slot.EndTime);

    if (!exists)
    {
        context.TimeSlots.Add(
            new TimeSlot
            {
                Name = $"{slot.StartTime:hh\\:mm tt} - {slot.EndTime:hh\\:mm tt}",
                StartTime = slot.StartTime,
                EndTime = slot.EndTime,
                IsActive = true
            });
    }
}

        await context.SaveChangesAsync();
    }
}