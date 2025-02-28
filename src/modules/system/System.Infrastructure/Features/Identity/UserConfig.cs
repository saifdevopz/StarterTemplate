using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Domain.Identity;

namespace System.Infrastructure.Features.Identity;

internal sealed class UserConfig : IEntityTypeConfiguration<UserM>
{
    public void Configure(EntityTypeBuilder<UserM> builder)
    {
        builder.HasKey(_ => _.UserId);

        builder.HasIndex(_ => _.Email)
            .IsUnique();

        builder.Property(_ => _.Email)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(_ => _.PasswordHash)
            .IsRequired();

        builder.Property(_ => _.PasswordSalt)
            .IsRequired();
    }
}