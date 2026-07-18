using AcademyERP.Domain.Entities.ScheduledClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyERP.Persistence.Configurations.ScheduledClasses;

public class ScheduledClassConfiguration : IEntityTypeConfiguration<ScheduledClass>
{
       public void Configure(EntityTypeBuilder<ScheduledClass> builder)
       {
              builder.ToTable("ScheduledClasses");

              builder.HasKey(x => x.Id);

              builder.Property(x => x.ClassDate)
                     .IsRequired();

              builder.Property(x => x.StartTime)
                     .IsRequired();

              builder.Property(x => x.EndTime)
                     .IsRequired();

              builder.Property(x => x.Status)
                     .IsRequired();

              builder.Property(x => x.Remarks)
                     .HasMaxLength(1000);

              builder.Property(x => x.CancellationReason)
                     .HasMaxLength(500);

              builder.HasOne(x => x.TeachingSchedule)
              .WithMany(x => x.ScheduledClasses)
              .HasForeignKey(x => x.TeachingScheduleId)
              .OnDelete(DeleteBehavior.Restrict);
       }
}