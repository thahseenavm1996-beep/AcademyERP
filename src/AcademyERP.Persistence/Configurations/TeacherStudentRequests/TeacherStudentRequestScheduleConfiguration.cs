using AcademyERP.Domain.Entities.TeacherStudentRequests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyERP.Persistence.Configurations.TeacherStudentRequests;

public class TeacherStudentRequestScheduleConfiguration
    : BaseEntityConfiguration<TeacherStudentRequestSchedule>
{
    public override void Configure(
        EntityTypeBuilder<TeacherStudentRequestSchedule> builder)
    {
        base.Configure(builder);

        builder.ToTable("TeacherStudentRequestSchedules");

        builder.HasKey(x => x.Id);

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
            x.TeacherStudentRequestId,
            x.DayOfWeek
        });
    }
}
