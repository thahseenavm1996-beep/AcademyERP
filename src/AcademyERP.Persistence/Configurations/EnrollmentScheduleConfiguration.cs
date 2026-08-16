using AcademyERP.Domain.Entities.EnrollmentSchedules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyERP.Persistence.Configurations;

public class EnrollmentScheduleConfiguration 
    : BaseEntityConfiguration<EnrollmentSchedule>
{
    public override void Configure(
        EntityTypeBuilder<EnrollmentSchedule> builder)
    {
        base.Configure(builder);

        builder.ToTable("EnrollmentSchedules");


        builder.HasOne(x => x.Enrollment)
            .WithMany(x => x.EnrollmentSchedules)
            .HasForeignKey(x => x.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.HasOne(x => x.TimeSlot)
            .WithMany()
            .HasForeignKey(x => x.TimeSlotId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.HasOne(x => x.ClassDuration)
            .WithMany()
            .HasForeignKey(x => x.ClassDurationId)
            .OnDelete(DeleteBehavior.Restrict);
            builder.HasIndex(x => new
{
    x.EnrollmentId,
    x.DayOfWeek,
    x.TimeSlotId
})
.IsUnique();
    }
}