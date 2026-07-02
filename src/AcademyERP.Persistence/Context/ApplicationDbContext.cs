using AcademyERP.Domain.Entities.Enrollments;
using AcademyERP.Domain.Entities.Lookups;
using AcademyERP.Domain.Entities.Parents;
using AcademyERP.Domain.Entities.Courses;
using AcademyERP.Domain.Entities.StudentParents;
using AcademyERP.Domain.Entities.Students;
using AcademyERP.Domain.Entities.Teachers;
using Microsoft.EntityFrameworkCore;
using AcademyERP.Domain.Entities.TeacherCourses;
using AcademyERP.Domain.Entities.TeacherAvailabilities;

namespace AcademyERP.Persistence.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Student> Students => Set<Student>();

    public DbSet<Teacher> Teachers => Set<Teacher>();

    public DbSet<Parent> Parents => Set<Parent>();

    public DbSet<StudentParent> StudentParents => Set<StudentParent>();


    public DbSet<Course> Courses => Set<Course>();

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
}