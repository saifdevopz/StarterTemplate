using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Domain.Identity;

namespace System.Infrastructure.Features.Identity;

internal sealed class UserRoleConfig : IEntityTypeConfiguration<UserRoleM>
{
    public void Configure(EntityTypeBuilder<UserRoleM> builder)
    {
        builder.HasKey(_ => new { _.UserId, _.RoleId });

        builder.HasOne<UserM>()
               .WithMany()
               .HasForeignKey(_ => _.UserId);

        builder.HasOne<RoleM>()
               .WithMany()
               .HasForeignKey(_ => _.RoleId);
    }
}
