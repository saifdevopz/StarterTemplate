namespace Parent.API.Services.Sorting;

internal sealed class SortMappingProvider(IEnumerable<ISortMappingDefinition> sortMappingDefinitions)
{
    public List<SortMapping> GetMappings<TSource, TDestination>()
    {
        SortMappingDefinition<TSource, TDestination>? sortMappingDefinition = sortMappingDefinitions
            .OfType<SortMappingDefinition<TSource, TDestination>>()
            .FirstOrDefault();

        if (sortMappingDefinition is null)
        {
            throw new InvalidOperationException(
                $"The mapping from '{typeof(TSource).Name}' into'{typeof(TDestination).Name} isn't defined");
        }

        return sortMappingDefinition.Mappings.ToList();
    }

    public bool ValidateMappings<TSource, TDestination>(string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return true;
        }

        var sortFields = sort
            .Split(',')
            .Select(f => f.Trim().Split(' ')[0])
            .Where(f => !string.IsNullOrWhiteSpace(f))
            .ToList();

        List<SortMapping> mapping = GetMappings<TSource, TDestination>();

        return sortFields.TrueForAll(f => mapping.Exists(m => m.SortField.Equals(f, StringComparison.OrdinalIgnoreCase)));
    }
}
