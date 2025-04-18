namespace Parent.API.Services.Sorting;

#pragma warning disable S2326 // Unused type parameters should be removed
internal sealed class SortMappingDefinition<TSource, TDestination> : ISortMappingDefinition
#pragma warning restore S2326 // Unused type parameters should be removed
{
    public required SortMapping[] Mappings { get; init; }
}
