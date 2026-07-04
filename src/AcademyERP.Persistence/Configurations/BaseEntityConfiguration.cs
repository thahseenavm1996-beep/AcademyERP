using AcademyERP.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcademyERP.Persistence.Configurations;

public abstract class BaseEntityConfiguration<TEntity>
    : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(x => x.CreatedAt)
       .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(e => e.CreatedBy)
               .HasMaxLength(100);

        builder.Property(e => e.UpdatedBy)
               .HasMaxLength(100);

        builder.Property(e => e.IsDeleted)
               .HasDefaultValue(false);
    }
}