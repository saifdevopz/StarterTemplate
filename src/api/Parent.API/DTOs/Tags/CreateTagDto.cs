namespace Parent.API.DTOs.Tags;

internal sealed record CreateTagDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}
