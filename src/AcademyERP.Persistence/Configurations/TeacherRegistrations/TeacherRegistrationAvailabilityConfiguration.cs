using AcademyERP.Domain.Entities.TeacherRegistrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyERP.Persistence.Configurations.TeacherRegistrations;

public class TeacherRegistrationAvailabilityConfiguration
    : IEntityTypeConfiguration<TeacherRegistrationAvailability>
{
    public void Configure(EntityTypeBuilder<TeacherRegistrationAvailability> builder)
    {
        builder.ToTable("TeacherRegistrationAvailabilities");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DayOfWeek)
            .HasConversion<int>();

        builder.HasOne(x => x.TimeSlot)
            .WithMany()
            .HasForeignKey(x => x.TimeSlotId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.TeacherRegistrationRequestId,
            x.DayOfWeek,
            x.TimeSlotId
        })
        .IsUnique();
    }
}
