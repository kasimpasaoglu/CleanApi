namespace Web.Services;

public sealed class OutputCacheInvalidator(IOutputCacheStore store) : ICacheInvalidator
{
    public Task EvictTagsAsync(CancellationToken cancellationToken, params string[] tags)
        => store.EvictTagsAsync(cancellationToken, tags);
}