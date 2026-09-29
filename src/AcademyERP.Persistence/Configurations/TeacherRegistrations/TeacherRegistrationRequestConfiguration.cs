using AcademyERP.Domain.Entities.TeacherRegistrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyERP.Persistence.Configurations.TeacherRegistrations;

public class TeacherRegistrationRequestConfiguration
    : IEntityTypeConfiguration<TeacherRegistrationRequest>
{
    public void Configure(EntityTypeBuilder<TeacherRegistrationRequest> builder)
    {
        builder.ToTable("TeacherRegistrationRequests");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FullName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.MobileNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Country)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Address)
            .HasMaxLength(500);

        builder.Property(x => x.Qualification)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Department)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.TeachingExperience)
            .HasMaxLength(200);

        builder.Property(x => x.Remarks)
            .HasMaxLength(1000);

        builder.Property(x => x.ReviewRemarks)
            .HasMaxLength(1000);

        builder.Property(x => x.Gender)
            .HasConversion<int>();

        builder.Property(x => x.Status)
            .HasConversion<int>();

        builder.HasMany(x => x.Availability)
            .WithOne(x => x.TeacherRegistrationRequest)
            .HasForeignKey(x => x.TeacherRegistrationRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.CurrentClasses)
            .WithOne(x => x.TeacherRegistrationRequest)
            .HasForeignKey(x => x.TeacherRegistrationRequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
