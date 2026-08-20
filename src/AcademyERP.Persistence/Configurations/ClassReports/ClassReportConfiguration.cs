using AcademyERP.Domain.Entities.ClassReports;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyERP.Persistence.Configurations.ClassReports;

public class ClassReportConfiguration 
    : IEntityTypeConfiguration<ClassReport>
{
    public void Configure(EntityTypeBuilder<ClassReport> builder)
    {
        builder.ToTable("ClassReports");

        builder.HasKey(x => x.Id);


        builder.Property(x => x.LessonTaken)
            .HasMaxLength(500);


        builder.Property(x => x.NextHomework)
            .HasMaxLength(500);


        builder.Property(x => x.TeacherRemarks)
            .HasMaxLength(2000);


        builder.HasOne(x => x.ScheduledClass)
            .WithMany()
            .HasForeignKey(x => x.ScheduledClassId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}