namespace Parent.API.Services.Sorting;

internal sealed record SortMapping(string SortField, string PropertyName, bool Reverse = false);
