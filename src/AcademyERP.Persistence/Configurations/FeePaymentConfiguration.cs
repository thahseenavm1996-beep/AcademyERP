using AcademyERP.Domain.Entities.Finance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyERP.Persistence.Configurations;

public class FeePaymentConfiguration
    : IEntityTypeConfiguration<FeePayment>
{
    public void Configure(
        EntityTypeBuilder<FeePayment> builder)
    {
        builder.ToTable("FeePayments");

        builder.Property(x => x.Amount)
            .HasPrecision(18, 2);

        builder.Property(x => x.TransactionReference)
            .HasMaxLength(100);

        builder.HasOne(x => x.FeeInvoice)
            .WithMany(x => x.Payments)
            .HasForeignKey(x => x.FeeInvoiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.PaymentMethod)
            .WithMany(x => x.Payments)
            .HasForeignKey(x => x.PaymentMethodId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}