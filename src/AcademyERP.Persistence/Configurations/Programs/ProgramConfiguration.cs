using AcademyERP.Domain.Entities.Programs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyERP.Persistence.Configurations.Programs;

public class ProgramConfiguration : IEntityTypeConfiguration<Program>
{
    public void Configure(EntityTypeBuilder<Program> builder)
    {
        builder.ToTable("Programs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProgramCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(x => x.ProgramCode)
            .IsUnique();

        builder.Property(x => x.ProgramName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.DefaultDurationMinutes)
            .IsRequired();

        builder.Property(x => x.DisplayOrder)
            .HasDefaultValue(0);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);
    }
}