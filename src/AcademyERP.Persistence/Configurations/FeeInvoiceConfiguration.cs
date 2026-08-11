using AcademyERP.Domain.Entities.Finance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyERP.Persistence.Configurations;

public class FeeInvoiceConfiguration : IEntityTypeConfiguration<FeeInvoice>
{
    public void Configure(EntityTypeBuilder<FeeInvoice> builder)
    {
        builder.ToTable("FeeInvoices");

        builder.Property(x => x.InvoiceNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Amount)
            .HasPrecision(18, 2);

        builder.Property(x => x.DiscountAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.PaidAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.BalanceAmount)
            .HasPrecision(18, 2);

        builder.HasOne(x => x.Enrollment)
    .WithMany(x => x.FeeInvoices)
    .HasForeignKey(x => x.EnrollmentId)
    .OnDelete(DeleteBehavior.Restrict);
    }
}