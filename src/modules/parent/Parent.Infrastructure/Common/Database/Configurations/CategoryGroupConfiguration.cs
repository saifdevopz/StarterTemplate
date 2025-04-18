using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Parent.Domain.Inventory.CategoryGroup;

namespace Parent.Infrastructure.Common.Database.Configurations;

internal sealed class CategoryGroupConfiguration : IEntityTypeConfiguration<CategoryGroupM>
{
    public void Configure(EntityTypeBuilder<CategoryGroupM> builder)
    {
        builder.HasKey(e => e.CategoryGroupId);

        builder.Property(e => e.CategoryGroupCode)
            .HasMaxLength(12)
            .IsRequired();

        builder.HasIndex(e => e.CategoryGroupCode)
            .IsUnique();

        builder.Property(e => e.CategoryGroupDesc)
            .HasMaxLength(50);

        builder.Property(e => e.IsActive)
            .IsRequired();
    }

}
