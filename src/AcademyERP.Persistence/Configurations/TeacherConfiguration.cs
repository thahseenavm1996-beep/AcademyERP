using AcademyERP.Domain.Entities.Teachers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyERP.Persistence.Configurations;

public class TeacherConfiguration : BaseEntityConfiguration<Teacher>
{
    public override void Configure(EntityTypeBuilder<Teacher> builder)
    {
        base.Configure(builder);
        builder.ToTable("Teachers");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.EmployeeCode)
               .HasMaxLength(20)
               .IsRequired();

        builder.HasIndex(t => t.EmployeeCode)
               .IsUnique();

        builder.Property(t => t.FullName)
               .HasMaxLength(150)
               .IsRequired();

        builder.Property(t => t.Country)
               .HasMaxLength(100);

        builder.Property(t => t.TimeZone)
               .HasMaxLength(100);

        builder.Property(t => t.Remarks)
               .HasMaxLength(500);

        builder.Property(t => t.Gender)
               .HasConversion<string>();

        builder.Property(t => t.Status)
               .HasConversion<string>();

        builder.HasMany(t => t.Enrollments)
               .WithOne(e => e.Teacher)
               .HasForeignKey(e => e.TeacherId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}