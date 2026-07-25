namespace Infrastructure.Services.Models;

public class FileStorageOptions
{
    public string WindowsRootPath { get; set; } = string.Empty;
    public string MacRootPath { get; set; } = string.Empty;

    public string GetActiveRootPath()
    {
        if (OperatingSystem.IsWindows()) return WindowsRootPath;
        if (OperatingSystem.IsMacOS()) return MacRootPath;

        throw new PlatformNotSupportedException("Unsupported.FileStorage");
    }
}