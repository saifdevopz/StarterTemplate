namespace Parent.API.DTOs.HabitTags;

internal sealed record UpsertHabitTagsDto
{
    public required List<string> TagIds { get; init; }
}
