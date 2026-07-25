namespace Web.Extensions;

public static class OutputCacheStoreExtensions
{
    public static async Task EvictTagsAsync(
        this IOutputCacheStore store,
        CancellationToken cancellationToken,
        params string[] tags)
    {
        foreach (var tag in tags)
            await store.EvictByTagAsync(tag, cancellationToken);
    }
}
