using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Domain.Identity;

namespace System.Infrastructure.Features.Identity;

internal sealed class RolePermissionConfig : IEntityTypeConfiguration<RolePermissionM>
{
    public void Configure(EntityTypeBuilder<RolePermissionM> builder)
    {
        builder
            .HasKey(_ => new { _.RoleId, _.PermissionId });

        builder
            .HasOne<PermissionM>()
            .WithMany()
            .HasForeignKey(_ => _.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne<RoleM>()
            .WithMany()
            .HasForeignKey(_ => _.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

    }

}

