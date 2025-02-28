using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Domain.Features.Tenant;

namespace System.Infrastructure.Features.Multitenancy;

internal sealed class TenantTypeConfig : IEntityTypeConfiguration<TenantTypeM>
{
    public void Configure(EntityTypeBuilder<TenantTypeM> builder)
    {
        builder.HasKey(_ => _.TenantTypeId);

        builder.Property(_ => _.TenantTypeCode)
               .HasMaxLength(10)
               .IsRequired();

        builder.HasIndex(_ => _.TenantTypeCode)
               .IsUnique();

        builder.Property(_ => _.TenantTypeDesc)
               .IsRequired();
    }
}
