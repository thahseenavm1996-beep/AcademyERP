using AcademyERP.Domain.Entities.Enrollments;
using AcademyERP.Domain.Entities.Lookups;
using AcademyERP.Domain.Entities.Parents;
using AcademyERP.Domain.Entities.Courses;
using AcademyERP.Domain.Entities.StudentParents;
using AcademyERP.Domain.Entities.Students;
using AcademyERP.Domain.Entities.Teachers;
using AcademyERP.Domain.Entities.TeacherCourses;
using AcademyERP.Domain.Entities.TeacherAvailabilities;
using AcademyERP.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AcademyERP.Domain.Entities.Common;
using AcademyERP.Domain.Entities;
using AcademyERP.Domain.Entities.Programs;
using AcademyERP.Domain.Entities.ClassReports;
using AcademyERP.Domain.Entities.TeachingSchedules;
using AcademyERP.Domain.Entities.ScheduledClasses;

namespace AcademyERP.Persistence.Context;

public class ApplicationDbContext
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<DocumentSequence> DocumentSequences => Set<DocumentSequence>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<Parent> Parents => Set<Parent>();
    public DbSet<StudentParent> StudentParents => Set<StudentParent>();
    public DbSet<Program> Programs => Set<Program>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<ClassReport> ClassReports => Set<ClassReport>();
    public DbSet<TeachingSchedule> TeachingSchedules => Set<TeachingSchedule>();
    public DbSet<ScheduledClass> ScheduledClasses => Set<ScheduledClass>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<ClassDuration> ClassDurations => Set<ClassDuration>();
    public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();
    public DbSet<TeacherCourse> TeacherCourses => Set<TeacherCourse>();
    public DbSet<TeacherAvailability> TeacherAvailabilities => Set<TeacherAvailability>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
    public override async Task<int> SaveChangesAsync(
    CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}