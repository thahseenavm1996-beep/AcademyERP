using AcademyERP.Domain.Entities.TeacherPayments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyERP.Persistence.Configurations;

public class TeacherPaymentConfigurationEntityTypeConfiguration
    : IEntityTypeConfiguration<AcademyERP.Domain.Entities.TeacherPayments.TeacherPaymentConfiguration>
{
    public void Configure(
        EntityTypeBuilder<AcademyERP.Domain.Entities.TeacherPayments.TeacherPaymentConfiguration> builder)
    {
        builder.ToTable("TeacherPaymentConfigurations");

        builder.Property(x => x.PaymentAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.Remarks)
            .HasMaxLength(500);

        builder.HasOne(x => x.Teacher)
            .WithMany(x => x.TeacherPaymentConfigurations)
            .HasForeignKey(x => x.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Enrollment)
            .WithMany(x => x.TeacherPaymentConfigurations)
            .HasForeignKey(x => x.EnrollmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.TeacherId,
            x.EnrollmentId,
            x.EffectiveFrom
        });
    }
}
