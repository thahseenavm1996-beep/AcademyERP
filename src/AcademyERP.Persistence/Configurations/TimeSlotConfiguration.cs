using AcademyERP.Domain.Entities.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyERP.Persistence.Configurations;

public class TimeSlotConfiguration : BaseEntityConfiguration<TimeSlot>
{
    public override void Configure(EntityTypeBuilder<TimeSlot> builder)
    {
        base.Configure(builder);
        builder.ToTable("TimeSlots");

        builder.Property(t => t.Name)
               .HasMaxLength(50)
               .IsRequired();

        builder.HasIndex(t => t.Name)
               .IsUnique();

        builder.Property(t => t.StartTime)
               .IsRequired();

        builder.Property(t => t.EndTime)
               .IsRequired();

        builder.Property(t => t.IsActive)
               .IsRequired();
    }
}