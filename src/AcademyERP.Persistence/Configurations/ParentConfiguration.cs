using AcademyERP.Domain.Entities.Parents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyERP.Persistence.Configurations;

public class ParentConfiguration : BaseEntityConfiguration<Parent>
{
    public override void Configure(EntityTypeBuilder<Parent> builder)
    {
        builder.ToTable("Parents");
        base.Configure(builder);

        builder.Property(p => p.FullName)
               .HasMaxLength(150)
               .IsRequired();

        builder.Property(p => p.PhoneNumber)
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(p => p.Email)
               .HasMaxLength(150)
               .IsRequired();

        builder.Property(p => p.Remarks)
               .HasMaxLength(500);

        builder.Property(p => p.Status)
               .HasConversion<string>();

        builder.HasMany(p => p.StudentParents)
               .WithOne(sp => sp.Parent)
               .HasForeignKey(sp => sp.ParentId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}