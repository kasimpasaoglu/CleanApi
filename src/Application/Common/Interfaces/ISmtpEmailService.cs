namespace Application.Common.Interfaces;

public interface ISmtpEmailService
{
    /// <summary>
    ///  Email gönderme işlemini asenkron olarak gerçekleştirir.
    /// </summary>
    /// <param name="to"> Email gönderilecek alıcının adresi.</param>
    /// <param name="subject"> Email'in konusu.</param>
    /// <param name="body"> Email'in içeriği.</param>
    /// <param name="isHtml"> Email içeriğinin HTML formatında olup olmadığını belirtir. Varsayılan değer true'dur.</param>
    /// <returns> Task</returns>
    Task SendAsync(string to, string subject, string body, bool isHtml = true);
    
    
    /// <summary>
    /// Toplu mail gonderme islemini asenkron olarak gerceklestiri
    /// </summary>
    /// <param name="to">Alicilarin adreslerini iceren liste</param>
    /// <param name="subject">konu</param>
    /// <param name="body">mesaj icerigi</param>
    /// <param name="isHtml">mesaj iceriginin html olup olmadigini belirler, varsayilan true</param>
    /// <returns></returns>
    Task SendBulkBccAsync(IReadOnlyCollection<string> to,string subject,string body,bool isHtml = true);

    /// <summary>
    /// Belirtilen birincil alıcıya (TO) mail gönderir, BCC listesindeki adresler gizli olarak kopyalanır.
    /// </summary>
    /// <param name="to">Birincil alıcı (To alanında görünür)</param>
    /// <param name="bcc">Gizli kopya alıcıları</param>
    /// <param name="subject">Konu</param>
    /// <param name="body">Mesaj içeriği</param>
    /// <param name="isHtml">Mesaj içeriğinin HTML olup olmadığını belirler, varsayılan true</param>
    /// <returns></returns>
    Task SendWithBccAsync(string to, IReadOnlyCollection<string> bcc, string subject, string body, bool isHtml = true);

}