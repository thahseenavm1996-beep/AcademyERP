using AcademyERP.Domain.Entities.Admissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyERP.Persistence.Configurations.Admissions;

public class AdmissionApplicationConfiguration
    : IEntityTypeConfiguration<AdmissionApplication>
{
    public void Configure(
        EntityTypeBuilder<AdmissionApplication> builder)
    {
        builder.ToTable("AdmissionApplications");

        builder.HasKey(x => x.Id);


        // Student Information

        builder.Property(x => x.StudentName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Age)
            .IsRequired();

        builder.Property(x => x.Gender)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.Country)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.City)
            .HasMaxLength(100);


        // Contact Information

        builder.Property(x => x.WhatsAppNumber)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.Email)
            .HasMaxLength(250);


        // Learning Preferences

        builder.Property(x => x.PreferredTiming)
            .HasMaxLength(200);

        builder.Property(x => x.Message)
            .HasMaxLength(2000);


        // Admission Status

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);


        // Program Relationship
        // Every application must have a Program.

        builder.HasOne(x => x.Program)
            .WithMany()
            .HasForeignKey(x => x.ProgramId)
            .OnDelete(DeleteBehavior.Restrict);


        // Course Relationship
        // Course is optional and may be assigned later.

        builder.HasOne(x => x.Course)
            .WithMany()
            .HasForeignKey(x => x.CourseId)
            .OnDelete(DeleteBehavior.Restrict);


        // Useful indexes for the Admin admission list

        builder.HasIndex(x => x.ProgramId);

        builder.HasIndex(x => x.CourseId);

        builder.HasIndex(x => x.Status);
        builder.Property(x => x.IsConverted)
            .HasDefaultValue(false);

        builder.Property(x => x.StudentId);

    }
}