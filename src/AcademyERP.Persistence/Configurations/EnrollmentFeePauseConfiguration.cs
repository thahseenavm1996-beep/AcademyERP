using AcademyERP.Domain.Entities.Enrollments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyERP.Persistence.Configurations;

public class EnrollmentFeePauseConfiguration
    : IEntityTypeConfiguration<EnrollmentFeePause>
{
    public void Configure(EntityTypeBuilder<EnrollmentFeePause> builder)
    {
        builder.ToTable("EnrollmentFeePauses");

        builder.Property(x => x.Reason)
            .HasMaxLength(500);

        builder.HasOne(x => x.Enrollment)
            .WithMany(x => x.FeePauses)
            .HasForeignKey(x => x.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.EnrollmentId, x.PauseStartDate });
    }
}
