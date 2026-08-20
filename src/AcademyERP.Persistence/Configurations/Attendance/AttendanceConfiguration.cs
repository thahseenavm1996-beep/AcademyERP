using AttendanceEntity = AcademyERP.Domain.Entities.Attendance.Attendance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyERP.Persistence.Configurations.Attendance;

public class AttendanceConfiguration : IEntityTypeConfiguration<AttendanceEntity>
{
    public void Configure(EntityTypeBuilder<AttendanceEntity> builder)
    {
        builder.ToTable("Attendances");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Remarks)
            .HasMaxLength(1000);


        builder.HasOne(x => x.ScheduledClass)
            .WithMany()
            .HasForeignKey(x => x.ScheduledClassId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}