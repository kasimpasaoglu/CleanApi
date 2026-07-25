namespace Infrastructure.Initialization;

public interface IDbInitializer
{
    /// <summary>
    /// Veritabanını migrate eder.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task InitializeAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Acilis seed verilerini ekler.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task TrySeedAsync(CancellationToken cancellationToken = default);
}