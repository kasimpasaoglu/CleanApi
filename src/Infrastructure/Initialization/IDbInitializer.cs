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
    /// GeoData, Lead Data gibi acilis seed verilerini ekler.
    /// </summary>
    /// <param name="geoDataJsonPath">geo datasinin oldugu dizin</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task TrySeedAsync(string geoDataJsonPath, CancellationToken cancellationToken = default);
}