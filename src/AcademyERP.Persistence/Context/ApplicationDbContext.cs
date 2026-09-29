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
using AcademyERP.Domain.Entities.Admissions;
using AcademyERP.Domain.Entities.Finance;
using AttendanceEntity = AcademyERP.Domain.Entities.Attendance.Attendance;
using AcademyERP.Domain.Entities.EnrollmentSchedules;
using AcademyERP.Domain.Entities.AcademicProgress;
using AcademyERP.Domain.Entities.Communication;
using AcademyERP.Domain.Entities.TeacherPayments;
using AcademyERP.Domain.Entities.ReportUnlockRequests;
using AcademyERP.Domain.Entities.TeacherRegistrations;
using AcademyERP.Domain.Entities.TeacherStudentRequests;



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
    public DbSet<ReportUnlockRequest> ReportUnlockRequests
    => Set<ReportUnlockRequest>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<ClassDuration> ClassDurations => Set<ClassDuration>();
    public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();
    public DbSet<TeacherCourse> TeacherCourses => Set<TeacherCourse>();
    public DbSet<FeeInvoice> FeeInvoices => Set<FeeInvoice>();
    public DbSet<FeePaymentSubmission> FeePaymentSubmissions { get; set; }
    public DbSet<EnrollmentFeePause> EnrollmentFeePauses => Set<EnrollmentFeePause>();
    public DbSet<TeacherPayment> TeacherPayments => Set<TeacherPayment>();

public DbSet<TeacherPaymentConfiguration> TeacherPaymentConfigurations
    => Set<TeacherPaymentConfiguration>();
    public DbSet<TeacherPaymentPause> TeacherPaymentPauses
    => Set<TeacherPaymentPause>();
    public DbSet<ClassProgress> ClassProgresses => 
    Set<ClassProgress>();
    public DbSet<Conversation> Conversations => Set<Conversation>();

public DbSet<Message> Messages => Set<Message>();
    public DbSet<TeacherAvailability> TeacherAvailabilities => Set<TeacherAvailability>();
    public DbSet<TeacherRegistrationRequest>
    TeacherRegistrationRequests =>
        Set<TeacherRegistrationRequest>();

public DbSet<TeacherRegistrationAvailability>
    TeacherRegistrationAvailabilities =>
        Set<TeacherRegistrationAvailability>();

public DbSet<TeacherRegistrationClass>
    TeacherRegistrationClasses =>
        Set<TeacherRegistrationClass>();

public DbSet<TeacherStudentRequest> TeacherStudentRequests
    => Set<TeacherStudentRequest>();

public DbSet<TeacherStudentRequestSchedule> TeacherStudentRequestSchedules
    => Set<TeacherStudentRequestSchedule>();
    public DbSet<RegistrationRequest>
    RegistrationRequests => Set<RegistrationRequest>();
   public DbSet<AttendanceEntity> Attendances => Set<AttendanceEntity>();
public DbSet<EnrollmentSchedule> EnrollmentSchedules => 
    Set<EnrollmentSchedule>();
public DbSet<RegistrationRequestStudent>
    RegistrationRequestStudents =>
        Set<RegistrationRequestStudent>();
    public DbSet<AdmissionApplication> AdmissionApplications =>
    Set<AdmissionApplication>();
    public DbSet<FeePayment> FeePayments { get; set; }

public DbSet<PaymentMethod> PaymentMethods { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        modelBuilder.Entity<Conversation>()
        .HasMany(x => x.Messages)
        .WithOne(x => x.Conversation)
        .HasForeignKey(x => x.ConversationId)
        .OnDelete(DeleteBehavior.Cascade);


    modelBuilder.Entity<Message>()
        .HasOne(x => x.Sender)
        .WithMany()
        .HasForeignKey(x => x.SenderUserId)
        .OnDelete(DeleteBehavior.Restrict);


    modelBuilder.Entity<Message>()
        .HasOne(x => x.Receiver)
        .WithMany()
        .HasForeignKey(x => x.ReceiverUserId)
        .OnDelete(DeleteBehavior.Restrict);


modelBuilder.Entity<AttendanceEntity>()
    .HasOne(x => x.ScheduledClass)
    .WithMany(x => x.Attendances)
    .HasForeignKey(x => x.ScheduledClassId)
    .OnDelete(DeleteBehavior.Cascade);
        
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
