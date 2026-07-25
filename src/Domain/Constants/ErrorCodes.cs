namespace Domain.Constants;

public abstract class ErrorCodes
{
    // Konvansiyon: sabitler kullanıcıya görünen Türkçe başlıklardır (ProblemDetails.Title).
    // Kullanım: Error.Conflict(ErrorCodes.AlreadyExists.SampleEntity, "ayrıntılı mesaj")
    // Yeni feature'da sabiti buraya ekle; handler'a string gömme.

    public abstract class NotFound
    {
        public const string User = "Kullanıcı Bulunamadı.";
        public const string FindMe = "Kullanıcı doğrulanamadı.";
    }

    public abstract class AlreadyExists
    {
        public const string SampleEntity = "Örnek Kayıt Zaten Mevcut.";
    }

    //Authentication Errors
    public const string Unauthorized = "Oturum Bulunamadı. Tekrar Giriş Yapınız.";
    public const string Forbidden = "Yetkisiz Erişim.";
    public const string InvalidCredentials = "E-posta veya şifre hatalı.";
    public const string LockedOut = "Hesap geçici olarak kilitlendi.";
    public const string Problem = "Başarısız işlem! Beklenmedik bir hata oluştu. Lütfen tekrar deneyiniz.";
}
