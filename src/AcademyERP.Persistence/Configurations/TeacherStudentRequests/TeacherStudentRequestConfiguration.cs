using AcademyERP.Domain.Entities.TeacherStudentRequests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyERP.Persistence.Configurations.TeacherStudentRequests;

public class TeacherStudentRequestConfiguration
    : BaseEntityConfiguration<TeacherStudentRequest>
{
    public override void Configure(
        EntityTypeBuilder<TeacherStudentRequest> builder)
    {
        base.Configure(builder);

        builder.ToTable("TeacherStudentRequests");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status)
            .HasConversion<int>();

        builder.Property(x => x.StudentName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.StudentPhoneNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.StudentFee)
            .HasPrecision(18, 2);

        builder.Property(x => x.TeacherSalary)
            .HasPrecision(18, 2);

        builder.Property(x => x.TeacherRemarks)
            .HasMaxLength(1000);

        builder.Property(x => x.AdminRemarks)
            .HasMaxLength(1000);

        builder.HasOne(x => x.Teacher)
            .WithMany()
            .HasForeignKey(x => x.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Student)
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasOne(x => x.Course)
            .WithMany()
            .HasForeignKey(x => x.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Schedules)
            .WithOne(x => x.TeacherStudentRequest)
            .HasForeignKey(x => x.TeacherStudentRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x =>
            new
            {
                x.TeacherId,
                x.StudentId,
                x.CourseId,
                x.Status
            });
    }
}
