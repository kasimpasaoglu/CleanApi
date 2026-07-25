namespace Application.Common.Interfaces;

public interface IFileStorageService
{
    /// <summary>
    /// Dosya yükleme işlemi yapar.
    /// </summary>
    /// <param name="fileStream"> Yüklenecek dosyanın akışı.</param>
    /// <param name="relativePath"> Yüklenecek dosyanın yolu.</param>
    /// <param name="contentType"> Yüklenecek dosyanın içeriği tipi.</param>
    /// <param name="cancellationToken"> İptal jetonu.</param>
    /// <returns> Yüklenen dosyanın yolu.</returns>
    Task<string> UploadAsync(Stream fileStream, string relativePath, string contentType, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Dosya indirme işlemi yapar.
    /// </summary>
    /// <param name="storedPath">Indirilecek dosyanın yolu.</param>
    /// <param name="cancellationToken"> İptal jetonu.</param>
    /// <returns> Yüklenen dosyanın akışı.</returns>
    Task<Stream> DownloadAsync(string storedPath, CancellationToken cancellationToken = default);
    
    /// <summary>
    ///  Dosya silme işlemi yapar.
    /// </summary>
    /// <param name="storedPath"> Silinecek dosyanın yolu.</param>
    /// <param name="cancellationToken"> İptal jetonu.</param>
    /// <returns> Görev tamamlandığında dönen bir görev.</returns>
    Task DeleteAsync(string storedPath, CancellationToken cancellationToken = default);
}