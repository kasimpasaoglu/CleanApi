namespace Application.Common.Interfaces;

public interface ICsvFileBuilderService
{
    /// <summary>
    /// Verilen veri koleksiyonundan CSV dosya içeriği üretir.
    /// </summary>
    /// <typeparam name="T">Her satırı temsil eden model tipi</typeparam>
    /// <param name="records">Yazılacak kayıtlar</param>
    /// <returns>Byte[] olarak CSV dosyası içeriği</returns>
    byte[] BuildCsvFile<T>(IEnumerable<T> records);
}