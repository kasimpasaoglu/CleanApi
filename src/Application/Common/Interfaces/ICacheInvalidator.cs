namespace Application.Common.Interfaces;

public interface ICacheInvalidator
{
    Task EvictTagsAsync(CancellationToken cancellationToken, params string[] tags);
}
