using AcademyERP.Domain.Entities.TeacherAvailabilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyERP.Persistence.Configurations;

public class TeacherAvailabilityConfiguration : BaseEntityConfiguration<TeacherAvailability>
{
    public override void Configure(EntityTypeBuilder<TeacherAvailability> builder)
    {
        base.Configure(builder);
        builder.ToTable("TeacherAvailabilities");


        builder.Property(ta => ta.DayOfWeek)
               .HasConversion<string>();

        builder.Property(ta => ta.IsAvailable)
               .IsRequired();

        builder.HasOne(ta => ta.Teacher)
               .WithMany(t => t.TeacherAvailabilities)
               .HasForeignKey(ta => ta.TeacherId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ta => ta.TimeSlot)
               .WithMany(ts => ts.TeacherAvailabilities)
               .HasForeignKey(ta => ta.TimeSlotId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(ta => new
        {
            ta.TeacherId,
            ta.DayOfWeek,
            ta.TimeSlotId
        }).IsUnique();
    }
}