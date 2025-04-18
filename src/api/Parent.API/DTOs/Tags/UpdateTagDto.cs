namespace Parent.API.DTOs.Tags;

internal sealed record UpdateTagDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}
