using AcademyERP.Domain.Entities.StudentParents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyERP.Persistence.Configurations;

public class StudentParentConfiguration : BaseEntityConfiguration<StudentParent>
{
    public override void Configure(EntityTypeBuilder<StudentParent> builder)
    {
        base.Configure(builder);
        builder.ToTable("StudentParents");


        builder.Property(sp => sp.Relationship)
       .HasConversion<string>()
       .IsRequired();
        builder.HasOne(sp => sp.Student)
               .WithMany(s => s.StudentParents)
               .HasForeignKey(sp => sp.StudentId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sp => sp.Parent)
               .WithMany(p => p.StudentParents)
               .HasForeignKey(sp => sp.ParentId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}