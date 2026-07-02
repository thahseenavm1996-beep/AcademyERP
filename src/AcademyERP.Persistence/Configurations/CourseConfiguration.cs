using AcademyERP.Domain.Entities.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyERP.Persistence.Configurations;

public class CourseConfiguration : BaseEntityConfiguration<Course>
{
    public override void Configure(EntityTypeBuilder<Course> builder)
    {
        base.Configure(builder);

        builder.ToTable("Courses");

        builder.Property(c => c.CourseCode)
               .HasMaxLength(20)
               .IsRequired();

        builder.HasIndex(c => c.CourseCode)
               .IsUnique();

        builder.Property(c => c.CourseName)
               .HasMaxLength(150)
               .IsRequired();

        builder.Property(c => c.Description)
               .HasMaxLength(500);

        builder.Property(c => c.StandardMonthlyFee)
               .HasPrecision(18, 2);
    }
}