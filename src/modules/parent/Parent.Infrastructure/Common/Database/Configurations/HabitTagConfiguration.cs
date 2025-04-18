﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Parent.Domain.TestEntities;

namespace Parent.Infrastructure.Common.Database.Configurations;

internal sealed class HabitTagConfiguration : IEntityTypeConfiguration<HabitTag>
{
    public void Configure(EntityTypeBuilder<HabitTag> builder)
    {
        builder.HasKey(ht => new { ht.HabitId, ht.TagId });

        // Already applied by the FK definition (Habit, Tag)
        builder.Property(h => h.HabitId).HasMaxLength(500);
        builder.Property(h => h.TagId).HasMaxLength(500);

        builder.HasOne<Tag>()
            .WithMany()
            .HasForeignKey(ht => ht.TagId);

        builder.HasOne<Habit>()
            .WithMany(h => h.HabitTags)
            .HasForeignKey(ht => ht.HabitId);
    }
}
