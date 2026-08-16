using AcademyERP.Domain.Entities.Admissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyERP.Persistence.Configurations.Admissions;

public class RegistrationRequestConfiguration
    : IEntityTypeConfiguration<RegistrationRequest>
{
    public void Configure(
        EntityTypeBuilder<RegistrationRequest> builder)
    {
        builder.ToTable("RegistrationRequests");

        builder.Property(x => x.ParentName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Country)
            .HasMaxLength(100);

        builder.Property(x => x.Address)
            .HasMaxLength(500);

        builder.Property(x => x.ReviewRemarks)
            .HasMaxLength(1000);

        builder.HasMany(x => x.Students)
            .WithOne(x => x.RegistrationRequest)
            .HasForeignKey(x => x.RegistrationRequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}