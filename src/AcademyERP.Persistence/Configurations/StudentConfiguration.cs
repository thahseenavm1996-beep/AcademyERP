using AcademyERP.Domain.Entities.Students;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyERP.Persistence.Configurations;

public class StudentConfiguration : BaseEntityConfiguration<Student>
{
    public override void Configure(EntityTypeBuilder<Student> builder)
    {
        base.Configure(builder);
        builder.ToTable("Students");



        builder.Property(x => x.AdmissionNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.FullName)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(x => x.AdmissionNumber)
            .IsUnique();
        builder.Property(s => s.Gender)
   .HasConversion<string>();

        builder.Property(s => s.Status)
               .HasConversion<string>();
    }
}