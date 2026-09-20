using AcademyERP.Domain.Entities.ReportUnlockRequests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyERP.Persistence.Configurations.ReportUnlockRequests;

public class ReportUnlockRequestConfiguration
    : IEntityTypeConfiguration<ReportUnlockRequest>
{
    public void Configure(
        EntityTypeBuilder<ReportUnlockRequest> builder)
    {
        builder.ToTable("ReportUnlockRequests");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Reason)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.AdminRemarks)
            .HasMaxLength(1000);

        builder.HasOne(x => x.ScheduledClass)
            .WithMany()
            .HasForeignKey(x => x.ScheduledClassId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Teacher)
            .WithMany()
            .HasForeignKey(x => x.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.ScheduledClassId,
            x.TeacherId
        });

        builder.HasIndex(x => x.Status);
    }
}