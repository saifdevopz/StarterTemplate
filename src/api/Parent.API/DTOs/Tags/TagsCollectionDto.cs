namespace Parent.API.DTOs.Tags;

internal sealed record TagsCollectionDto : ICollectionResponse<TagDto>
{
    public List<TagDto> Items { get; init; } = [];
}
