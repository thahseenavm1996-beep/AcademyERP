using AcademyERP.Domain.Entities.TeacherCourses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyERP.Persistence.Configurations;

public class TeacherCourseConfiguration : BaseEntityConfiguration<TeacherCourse>
{
    public override void Configure(EntityTypeBuilder<TeacherCourse> builder)
    {
        base.Configure(builder);
        builder.ToTable("TeacherCourses");

        builder.HasOne(tc => tc.Teacher)
               .WithMany(t => t.TeacherCourses)
               .HasForeignKey(tc => tc.TeacherId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(tc => tc.Course)
               .WithMany(c => c.TeacherCourses)
               .HasForeignKey(tc => tc.CourseId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(tc => new { tc.TeacherId, tc.CourseId })
               .IsUnique();
    }
}