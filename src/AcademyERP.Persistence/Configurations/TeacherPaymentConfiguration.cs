using AcademyERP.Domain.Entities.TeacherPayments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyERP.Persistence.Configurations;

public class TeacherPaymentConfiguration : IEntityTypeConfiguration<TeacherPayment>
{
    public void Configure(EntityTypeBuilder<TeacherPayment> builder)
    {
        builder.ToTable("TeacherPayments");

        builder.Property(x => x.Amount)
            .HasPrecision(18, 2);

        builder.Property(x => x.Status)
            .HasConversion<string>();

        builder.Property(x => x.TransactionReference)
            .HasMaxLength(100);

        builder.Property(x => x.Remarks)
            .HasMaxLength(500);

        builder.HasOne(x => x.Teacher)
            .WithMany(x => x.TeacherPayments)
            .HasForeignKey(x => x.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Enrollment)
            .WithMany(x => x.TeacherPayments)
            .HasForeignKey(x => x.EnrollmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.TeacherId,
            x.EnrollmentId,
            x.CycleStartDate,
            x.CycleEndDate
        })
        .IsUnique();
    }
}
