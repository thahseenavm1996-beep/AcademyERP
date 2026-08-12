using AcademyERP.Domain.Entities.Finance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyERP.Persistence.Configurations;

public class PaymentMethodConfiguration
    : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(
        EntityTypeBuilder<PaymentMethod> builder)
    {
        builder.ToTable("PaymentMethods");

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.HasData(
            new PaymentMethod
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Cash",
                IsActive = true,
                IsDeleted = false,
                CreatedAt = new DateTime(2025, 1, 1)
            },
            new PaymentMethod
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Bank Transfer",
                IsActive = true,
                IsDeleted = false,
                CreatedAt = new DateTime(2025, 1, 1)
            },
            new PaymentMethod
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Stripe",
                IsActive = true,
                IsDeleted = false,
                CreatedAt = new DateTime(2025, 1, 1)
            },
            new PaymentMethod
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Name = "PayPal",
                IsActive = true,
                IsDeleted = false,
                CreatedAt = new DateTime(2025, 1, 1)
            },
            new PaymentMethod
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Name = "Card",
                IsActive = true,
                IsDeleted = false,
                CreatedAt = new DateTime(2025, 1, 1)
            });
    }
}