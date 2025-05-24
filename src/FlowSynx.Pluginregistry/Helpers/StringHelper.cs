namespace FlowSynx.Pluginregistry.Helpers;

public static class StringHelper
{
    public static List<string> GetTagsList(string? tags)
    {
        return tags?.Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim().ToLowerInvariant())
            .Distinct()
            .ToList() ?? new List<string>();
    }
}