using AcademyERP.Domain.Entities.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyERP.Persistence.Configurations;

public class ClassDurationConfiguration : BaseEntityConfiguration<ClassDuration>
{
    public override void Configure(EntityTypeBuilder<ClassDuration> builder)
    {
        base.Configure(builder);
        builder.ToTable("ClassDurations");

        builder.Property(c => c.Name)
               .HasMaxLength(50)
               .IsRequired();

        builder.HasIndex(c => c.Name)
               .IsUnique();

        builder.Property(c => c.Minutes)
               .IsRequired();

        builder.Property(c => c.IsActive)
               .IsRequired();
    }
}