using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Domain.Identity;

namespace System.Infrastructure.Features.Identity;

internal sealed class RoleConfig : IEntityTypeConfiguration<RoleM>
{
    public void Configure(EntityTypeBuilder<RoleM> builder)
    {
        builder.HasKey(_ => _.RoleId);

        builder.Property(_ => _.RoleName)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(_ => _.RoleName)
            .IsUnique();

        builder.Property(_ => _.NormalizedRoleName)
            .HasMaxLength(30)
            .IsRequired();
    }
}