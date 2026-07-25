using Infrastructure.Services.Models;

namespace Infrastructure.Services;

public class LocalFileStorageService(IOptions<FileStorageOptions> options) : IFileStorageService
{
    private readonly string _root = options.Value.GetActiveRootPath();

    public async Task<string> UploadAsync(Stream stream, string relativePath, string contentType, CancellationToken ct = default)
    {
        var safe = relativePath.Replace('\\', '/').TrimStart('/');
        var fullPath = ToFullPath(safe);

        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        if (stream.CanSeek) stream.Position = 0;
        await using var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.Read);
        await stream.CopyToAsync(fs, ct);
        
        return safe;
    }

    public Task<Stream> DownloadAsync(string storedPath, CancellationToken ct = default)
    {
        var full = ToFullPath(storedPath);

        return !File.Exists(full) 
            ? throw new FileNotFoundException("Dosya bulunamadı", full) 
            : Task.FromResult<Stream>(new FileStream(full, FileMode.Open, FileAccess.Read, FileShare.Read));
    }

    public Task DeleteAsync(string storedPath, CancellationToken ct = default)
    {
        var full = ToFullPath(storedPath);

        if (File.Exists(full)) 
            File.Delete(full);
        return Task.CompletedTask;
    }
    
    private string ToFullPath(string storedPath)
    {
        var safeOs = storedPath.Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar);
        var full = Path.GetFullPath(Path.Combine(_root, safeOs));
        var rootFull = Path.GetFullPath(_root) + Path.DirectorySeparatorChar;

        if (!full.StartsWith(rootFull, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Path traversal");

        return full;
    }
}