using AcademyERP.Domain.Entities.TeachingSchedules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyERP.Persistence.Configurations.TeachingSchedules;

public class TeachingScheduleConfiguration : IEntityTypeConfiguration<TeachingSchedule>
{
    public void Configure(EntityTypeBuilder<TeachingSchedule> builder)
    {
        builder.ToTable("TeachingSchedules");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DayOfWeek)
               .IsRequired();

        builder.Property(x => x.StartTime)
               .IsRequired();

        builder.Property(x => x.IsActive)
               .HasDefaultValue(true);

        builder.Property(x => x.MaximumStudents)
               .HasDefaultValue(1);

        builder.Property(x => x.Remarks)
               .HasMaxLength(1000);

        builder.HasOne(x => x.Enrollment)
               .WithMany()
               .HasForeignKey(x => x.EnrollmentId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Teacher)
               .WithMany()
               .HasForeignKey(x => x.TeacherId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ClassDuration)
               .WithMany()
               .HasForeignKey(x => x.ClassDurationId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}