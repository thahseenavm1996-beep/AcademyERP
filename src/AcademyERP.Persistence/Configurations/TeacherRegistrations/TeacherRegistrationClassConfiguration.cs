using AcademyERP.Domain.Entities.TeacherRegistrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyERP.Persistence.Configurations.TeacherRegistrations;

public class TeacherRegistrationClassConfiguration
    : IEntityTypeConfiguration<TeacherRegistrationClass>
{
    public void Configure(EntityTypeBuilder<TeacherRegistrationClass> builder)
    {
        builder.ToTable("TeacherRegistrationClasses");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DayOfWeek)
            .HasConversion<int>();

        builder.Property(x => x.ClassType)
            .HasMaxLength(100);

        builder.Property(x => x.StudentName)
    .HasMaxLength(200);

builder.HasOne(x => x.Student)
    .WithMany()
    .HasForeignKey(x => x.StudentId)
    .IsRequired(false)
    .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.Course)
            .WithMany()
            .HasForeignKey(x => x.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.TimeSlot)
            .WithMany()
            .HasForeignKey(x => x.TimeSlotId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ClassDuration)
            .WithMany()
            .HasForeignKey(x => x.ClassDurationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
