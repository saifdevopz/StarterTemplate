using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Domain;

namespace Store.Infrastructure.Common.Database.Configurations;

internal sealed class StoreMConfig : IEntityTypeConfiguration<StoreM>
{
    public void Configure(EntityTypeBuilder<StoreM> builder)
    {
        builder.HasKey(e => e.StoreId);

        builder.Property(e => e.StoreName)
            .HasMaxLength(12)
            .IsRequired();
    }

}