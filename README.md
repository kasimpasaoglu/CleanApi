# CleanApi

> Clean Architecture solution template for .NET 10 Web API projects.

---

## Türkçe

### Genel Bakış

CleanApi, dört projeden oluşan, çalışmaya hazır bir Clean Architecture çözümüdür:

| Proje | Sorumluluk |
|---|---|
| `src/Domain` | Entity'ler, temel sınıflar (auditable / soft-delete), enum'lar, sabitler, domain event'ler, repository arayüzleri |
| `src/Application` | CQRS use case'leri (MediatR), pipeline behaviour'ları, validasyon, mapping, servis arayüzleri |
| `src/Infrastructure` | EF Core + SQL Server, Identity, SaveChanges interceptor'ları, repository'ler, dış servisler (SMTP, dosya depolama, CSV) |
| `src/Web` | API controller'ları, middleware, Swagger, konfigürasyon — composition root |

### Kutudan çıkanlar

- **MediatR ile CQRS** — sıralı 5 pipeline behaviour: UnhandledException → Authorization → Validation (FluentValidation) → Performance → Audit
- **EF Core 10 + SQL Server** — generic repository + unit of work, 4 interceptor: audit alanları, audit log, soft delete, domain event dispatch
- **ASP.NET Core Identity ile cookie authentication** — `CleanApi_session` cookie'si (HttpOnly, Secure, 3 gün kayan süre)
- **Serilog** — async console + günlük dönen dosya sink'i (`Logs/log-<tarih>.txt`)
- **Swagger + API versiyonlama** — URL segment versiyonlama (`/api/v1/...`)
- **Output caching** — isimli policy'ler (24s / 2s / 1s / 15dk / 5dk) ve tag tabanlı invalidation servisi
- **Health check** — DbContext kontrolü dahil `GET /health`
- **Request audit middleware** — istek kayıtlarını veritabanına yazar
- **Result pattern** — `Result` / `Error` tipleri, global exception handler üzerinden RFC 7807 ProblemDetails yanıtlarına dönüştürülür
- **Örnek feature** — uçtan uca bağlanmış `SampleEntity` (entity → EF konfigürasyonu → repository → command/query → controller) çalışan bir referanstır

### Gereksinimler

- .NET SDK **10.0.101** veya üzeri (`global.json` ile sabitlenmiştir, `rollForward: latestFeature`)
- SQL Server (lokal veya uzak)
- EF Core CLI aracı: `dotnet tool install --global dotnet-ef`

### Template'i yükleme

Template kök dizininden (`CleanApi.sln` dosyasının bulunduğu klasör) çalıştırın:

```bash
dotnet new install ./
dotnet new list clean-api   # kaydolduğunu doğrulayın
```

Template'i değiştirdikten sonra güncellemek için: `dotnet new install ./ --force`
Kaldırmak için: `dotnet new uninstall <template-klasör-yolu>`

### Yeni proje oluşturma

```bash
dotnet new clean-api -n ProjeAdi -o ProjeAdi
```

Tüm `CleanApi` geçişleri — solution dosyası, session cookie adı, Swagger başlığı, CORS policy adı, DataProtection uygulama adı — proje adınızla değiştirilir. Namespace'ler bilinçli olarak öneksizdir (`Domain`, `Application`, `Infrastructure`, `Web`) ve olduğu gibi kalır.

### İlk çalıştırma — adım adım

Template bilinçli olarak **EF migration içermez** (örnek entity'leri zaten değiştireceksiniz); bu yüzden ilk çalıştırmadan önce ilk migration'ı oluşturun:

1. **Connection string** — `src/Web/appsettings.json` içinde `ConnectionStrings:DefaultConnection` değerini kendi SQL Server bağlantı cümlenizle değiştirin.
2. **İlk migration**:

   ```bash
   dotnet ef migrations add InitialCreate --project src/Infrastructure --startup-project src/Web
   ```

3. **Çalıştırın**:

   ```bash
   dotnet run --project src/Web
   ```

   Açılışta bekleyen migration'lar otomatik uygulanır ve — Development'ta `SeedInitialData` `true` olduğu için — `Administrator` rolü, varsayılan bir departman ve varsayılan admin kullanıcısı seed edilir.

4. **Keşfedin** — Swagger UI (yalnızca Development): `https://localhost:7249/swagger` (portlar `src/Web/Properties/launchSettings.json` içinde tanımlıdır). Health endpoint'i: `/health`.
5. **Giriş** — `POST /api/v1/Auth/login`:

   ```json
   { "email": "admin@yourdomain.com", "password": "Admin123*" }
   ```

   Yanıt `CleanApi_session` cookie'sini set eder; sonraki istekler bu cookie ile doğrulanır.

> ⚠️ **Varsayılan admin bilgilerini hemen değiştirin.** `src/Infrastructure/Initialization/Admin/AdminSeeder.cs` içinde tanımlıdır.

### Konfigürasyon başvurusu (`src/Web/appsettings.json`)

| Bölüm | Amaç |
|---|---|
| `ConnectionStrings` + `Db:ActiveConnection` | `Db:ActiveConnection`, **hangi** isimli connection string'in kullanılacağını seçer. Birden fazla isimli bağlantı ekleyip tek anahtarla geçiş yapabilirsiniz. Referans verilen string yoksa uygulama açılışta hata verir. |
| `SeedInitialData` | `true` → açılışta veritabanı seed'i çalışır (Development varsayılanı). Production'da `false` olmalıdır. |
| `SmtpSettings` | SMTP e-posta servisinin kullandığı posta ayarları (host, port, kimlik bilgileri). |
| `FileStorage` | Lokal dosya depolama kök yolları — çalışan işletim sistemine göre `WindowsRootPath` / `MacRootPath` seçilir. |
| `Licensing` | AutoMapper ve MediatR lisans anahtarları (ticari sürümler). Anahtarınız yoksa placeholder'ları olduğu gibi bırakın. |
| `Serilog` | Sink'ler, minimum seviyeler ve enricher'lar. |
| `DataProtection` | **Yalnızca Production** — `KeysPath` zorunludur (yoksa açılışta hata fırlatılır); `ApplicationName` key ring'i izole eder. `appsettings.Production.json` içinde tanımlıdır. |

**Ortam katmanlaması:** `appsettings.json` tam iskelettir (development varsayılanları). `appsettings.Production.json` yalnızca Production'da **farklı olan değerleri** içerir ve `ASPNETCORE_ENVIRONMENT` değişkenine göre **çalışma zamanında** üzerine bindirilir (değişken set edilmemişse ASP.NET Core varsayılanı `Production`'dır). Override dosyasını minimal tutun.

### CORS

Yalnızca Development'ta aktif olan `CleanApi FrontEnd` policy'si, credentials destekli olarak `http://localhost:3000` origin'ine izin verir. Origin'leri `src/Web/DependencyInjection.cs` içinden düzenleyin.

### İlk feature'ınızı ekleme

1. **Domain** — entity `src/Domain/Entities/<Schema>/` altına, repository arayüzü `src/Domain/Interfaces/Repositories/` altına.
2. **Infrastructure** — EF konfigürasyonu `src/Infrastructure/Persistence/Configurations/` altına, repository implementasyonu ve `src/Infrastructure/DependencyInjection.cs` içine DI kaydı.
3. **Application** — handler, validator ve mapping profile ile birlikte `src/Application/Features/<FeatureAdi>/Commands|Queries/`.
4. **Web** — controller `src/Web/Controllers/V1/` altına.
5. **Migration** — `dotnet ef migrations add <Ad> --project src/Infrastructure --startup-project src/Web`.

Çalışan referans olarak `SampleEntity` dilimini kullanın; gerçek feature'lara başladığınızda tüm `Sample*` dosyalarını (ve placeholder `SchemaName` şemasını/klasörlerini) silin.

### Çözüm yerleşimi

```
CleanApi.sln
global.json
src/
├── Domain/            # Entity'ler, temel sınıflar, enum'lar, sabitler, domain event'ler, repository arayüzleri
├── Application/       # Use case'ler (MediatR), behaviour'lar, validator'lar, mapping, servis arayüzleri
├── Infrastructure/    # EF Core, Identity, interceptor'lar, repository'ler, dış servisler, seed
└── Web/               # Controller'lar, middleware, Swagger, konfigürasyon, Program.cs
```
