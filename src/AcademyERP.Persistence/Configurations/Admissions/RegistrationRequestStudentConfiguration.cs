using AcademyERP.Domain.Entities.Admissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyERP.Persistence.Configurations.Admissions;

public class RegistrationRequestStudentConfiguration
    : IEntityTypeConfiguration<RegistrationRequestStudent>
{
    public void Configure(
        EntityTypeBuilder<RegistrationRequestStudent> builder)
    {
        builder.ToTable(
            "RegistrationRequestStudents");

        builder.Property(x => x.StudentName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.PreferredTime)
            .HasMaxLength(100);

        builder.Property(x => x.Remarks)
            .HasMaxLength(500);
            builder.HasOne(x => x.Program)
    .WithMany()
    .HasForeignKey(x => x.ProgramId)
    .OnDelete(DeleteBehavior.Restrict);
    }
}