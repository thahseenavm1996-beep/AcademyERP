using AcademyERP.Domain.Entities.TeacherPayments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyERP.Persistence.Configurations;

public class TeacherPaymentPauseConfiguration
    : IEntityTypeConfiguration<TeacherPaymentPause>
{
    public void Configure(
        EntityTypeBuilder<TeacherPaymentPause> builder)
    {
        builder.ToTable("TeacherPaymentPauses");

        builder.Property(x => x.PauseStartDate)
            .IsRequired();

        builder.Property(x => x.ResumeDate)
            .IsRequired(false);

        builder.Property(x => x.Reason)
            .HasMaxLength(500);

        builder.HasOne(x => x.TeacherPaymentConfiguration)
            .WithMany(x => x.Pauses)
            .HasForeignKey(x => x.TeacherPaymentConfigurationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x =>
            new
            {
                x.TeacherPaymentConfigurationId,
                x.PauseStartDate
            });
    }
}