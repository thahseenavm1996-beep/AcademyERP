using AcademyERP.Domain.Entities.Enrollments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyERP.Persistence.Configurations;

public class EnrollmentConfiguration : BaseEntityConfiguration<Enrollment>
{
    public override void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        base.Configure(builder);
        builder.ToTable("Enrollments");

        builder.Property(e => e.MonthlyFee)
               .HasPrecision(18, 2);

        builder.Property(e => e.ScholarshipAmount)
               .HasPrecision(18, 2);

        builder.Property(e => e.DiscountAmount)
               .HasPrecision(18, 2);

        builder.Property(e => e.FinalMonthlyFee)
               .HasPrecision(18, 2);

        builder.Property(e => e.Remarks)
               .HasMaxLength(500);

        builder.Property(e => e.ScholarshipReason)
               .HasMaxLength(250);

        builder.Property(e => e.DiscountReason)
               .HasMaxLength(250);
        builder.Property(e => e.Status)
.HasConversion<string>();

        // Student -> Enrollments
        builder.HasOne(e => e.Student)
               .WithMany(s => s.Enrollments)
               .HasForeignKey(e => e.StudentId)
               .OnDelete(DeleteBehavior.Restrict);

        // Course -> Enrollments
        builder.HasOne(e => e.Course)
               .WithMany(c => c.Enrollments)
               .HasForeignKey(e => e.CourseId)
               .OnDelete(DeleteBehavior.Restrict);

        // Teacher -> Enrollments
        builder.HasOne(e => e.Teacher)
               .WithMany(t => t.Enrollments)
               .HasForeignKey(e => e.TeacherId)
               .OnDelete(DeleteBehavior.SetNull);

        // TimeSlot -> Enrollments
        builder.HasOne(e => e.TimeSlot)
               .WithMany()
               .HasForeignKey(e => e.TimeSlotId)
               .OnDelete(DeleteBehavior.Restrict);

        // ClassDuration -> Enrollments
        builder.HasOne(e => e.ClassDuration)
               .WithMany()
               .HasForeignKey(e => e.ClassDurationId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}