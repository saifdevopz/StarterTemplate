﻿namespace Parent.Domain.TestEntities;

public sealed class HabitTag
{
    public required string HabitId { get; set; }
    public required string TagId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
