using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Domain.Features.Tenant;

namespace System.Infrastructure.Features.Multitenancy;

internal sealed class TenantConfig : IEntityTypeConfiguration<TenantM>
{
    public void Configure(EntityTypeBuilder<TenantM> builder)
    {
        builder.HasKey(_ => _.TenantId);

        builder.Property(_ => _.TenantName)
               .HasMaxLength(15)
               .IsRequired();

        builder.HasIndex(_ => _.DatabaseName)
               .IsUnique();

        builder.Property(_ => _.DatabaseName)
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(_ => _.ConnectionString)
               .HasMaxLength(300)
               .IsRequired();
    }
}