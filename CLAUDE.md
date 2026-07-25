# CleanApi — Agent Mimari Rehberi

Bu doküman, bu repoda çalışan AI agent'ları için temel mimari haritadır. Kod yazmadan önce ilgili bölümü oku; buradaki konvansiyonların dışına çıkma.

## Proje Kimliği

- **.NET 10** (`net10.0`), ASP.NET Core Web API, Clean Architecture (4 katman).
- **Single-tenant**: `TenantId` veya multi-tenant altyapısı yoktur; varsayma.
- **Auth**: ASP.NET Identity + **cookie** (`CleanApi_session`). JWT yoktur; REST endpoint'leri yalnızca cookie kabul eder.
- **Test projesi yok**: Doğrulama `dotnet build` + manuel test iledir. Warning'leri görmezden gelme.
- **Varsayılan kültür**: `tr-TR` (Program.cs). Kullanıcıya görünen mesajlar Türkçedir (Türkçe karakterler eksiksiz: ç, ğ, ı, ö, ş, ü).
- **Örnek dilim**: `SampleEntity` uçtan uca referanstır; gerçek feature'lar başlayınca silinir (bkz. son bölüm).

## Katmanlar ve Bağımlılık Yönü

`Domain ← Application ← Infrastructure` — `Web` composition root olarak hepsini referans alar. Bağımlılık oku her zaman içe (Domain'e) doğrudur; tersine referans ekleme.

| Katman | İçerik | Referans verdiği |
|---|---|---|
| `src/Domain` | Entity, enum, sabitler, base sınıflar, domain event tabanı, repository **arayüzleri** | — (paket bağımlılığı da yok) |
| `src/Application` | Command/Query/Handler/Validator, pipeline behaviour'ları, servis **arayüzleri**, DTO/Result modelleri, mapping profilleri, domain event handler'ları | Domain |
| `src/Infrastructure` | AppDbContext, interceptor'lar, repository **implementasyonları**, Identity, dış servisler (SMTP, dosya, CSV), seed | Application, Domain |
| `src/Web` | Controller'lar, middleware, Swagger, exception handler, DI bootstrap, appsettings | Hepsi |

### Neyi nereye koyarım?

| Eklenecek şey | Konum |
|---|---|
| Entity | `src/Domain/Entities/<Şema>/` (klasör adı = DB şeması) |
| Enum | `src/Domain/Enums/` |
| Hata mesajı / policy / rol / validasyon sabiti | `src/Domain/Constants/` (`ErrorCodes`, `Policies`, `Roles`, `ValidationRules`) |
| Repository arayüzü | `src/Domain/Interfaces/Repositories/` (`IRepository<T, TId>` tabanından türet) |
| Domain event | Entity içinde `BaseEvent` türevi; handler `src/Application/Events/<EventAdı>/` |
| Command / Query | `src/Application/Features/<Feature>/Commands\|Queries/<UseCase>/` (Command + Handler + Validator + Response aynı klasörde) |
| AutoMapper profili | `src/Application/Features/<Feature>/Profiles/MappingProfiles.cs` |
| Servis arayüzü | `src/Application/Common/Interfaces/` → impl `src/Infrastructure/Services/` |
| EF configuration | `src/Infrastructure/Persistence/Configurations/<Şema>/` (`IEntityTypeConfiguration<T>`, assembly taramasıyla otomatik bulunur) |
| Repository impl | `src/Infrastructure/Persistence/Repositories/` (`EfRepository<T, TId>` tabanından türet) + DI kaydı `src/Infrastructure/DependencyInjection.cs` |
| Controller | `src/Web/Controllers/V1/` (`BaseApiController`'dan türet) |
| Middleware | `src/Web/Middleware/` |

Her projede `GlobalUsings.cs` vardır; yeni ortak using'ler oraya eklenir, dosya başlarına değil.

## İstek Yaşam Döngüsü

```
Controller → Mediator.Send → [Behaviour pipeline] → Handler → Result<T> → result.Match(Results.Ok, CustomResults.Problem)
```

Behaviour sırası (kayıt sırası = çalışma sırası, `src/Application/DependencyInjection.cs`):

| # | Behaviour | Ne yapar |
|---|---|---|
| 1 | `UnhandledExceptionBehaviour` | Exception'ı loglar + `AuditLog`'a güvenli (redacted/truncated) payload yazar + yeniden fırlatır. İstek iptalini (`IRequestCancellationClassifier`) ayrıştırıp sessizce `OperationCanceledException`'a çevirir. |
| 2 | `AuthorizationBehaviour` | Request sınıfındaki `[Authorize]` attribute'una bakar (aşağıda). Girişsizse `Unauthorized`, rol/policy tutmazsa `Forbidden` içerikli `BusinessException` fırlatır. |
| 3 | `ValidationBehaviour` | Tüm `IValidator<TRequest>`'leri çalıştırır; hata varsa `ValidationException` → 400 `ValidationProblemDetails`. |
| 4 | `PerformanceBehaviour` | 1000 ms'yi aşan istekleri warning olarak loglar. |
| 5 | `AuditBehavior` | Başarılı her request için `dbo.AuditLog`'a kayıt atar; `Result.IsFailure` ise yalnızca warning loglar. |

Controller kuralları: iş mantığı yazılmaz, try/catch yazılmaz; yalnızca `Mediator.Send` + `Match`. Endpoint imzası `IResult` döner. Route: `api/v{version:apiVersion}/[controller]`, versiyonlama `Asp.Versioning` iledir.

## Hata Yönetimi (kanonik desen)

- Hata modeli: `Error(Code, Description, ErrorType)` — factory'ler: `Error.NotFound(...)`, `Error.Conflict(...)`, `Error.Validation(...)`, `Error.Failure(...)`, `Error.Unauthorized(...)`, `Error.Forbidden(...)`. Mesaj sabitleri `Domain/Constants/ErrorCodes.cs`'ten gelir; string'i handler'a gömme.
- **Yol 1 (tercih)**: Handler iş kuralı ihlalinde `return Result.Failure<T>(Error.X(ErrorCodes.Y, "..."))`.
- **Yol 2**: Derin çağrı zincirinden kısa devre için `throw new BusinessException(Error.X(...))`. Handler içinde yakalarsan `ex.ToFailureResult<T>()` ile Result'a çevir; yakalamazsan `CustomExceptionHandler` zaten ProblemDetails'e çevirir.
- `ErrorType` → HTTP status eşlemesi `src/Web/Infrastructure/ProblemDetailsFactory.cs`:

| ErrorType | Status |
|---|---|
| Validation, Problem | 400 |
| Unauthorized | 401 |
| Forbidden | 403 |
| NotFound | 404 |
| Conflict | 409 |
| Failure (ve diğerleri) | 500 |

- `CustomExceptionHandler` (`src/Web/Infrastructure/`): `BusinessException`, `ValidationException`, `UnauthorizedAccessException`, `OperationCanceledException` tiplerini ProblemDetails'e çevirir. Development'ta bilinmeyen exception handle edilmez (stack trace görünür); Production'da fallback ProblemDetails döner.

## Authorization — iki seviye

1. **Controller seviyesi**: ASP.NET Core `[Authorize]` (cookie şeması) — kimlik doğrulama. Anonim endpoint'e `[AllowAnonymous]`.
2. **Request seviyesi**: `Application.Common.Security.AuthorizeAttribute` (ASP.NET'inki DEĞİL — namespace'e dikkat) request **record'unun üzerine** konur; `AuthorizationBehaviour` işler:
   - `[Authorize(Roles = Roles.Administrator)]` — virgülle çoklu rol, biri yetse geçer.
   - `[Authorize(Policy = Policies.CanCreateSampleEntity)]` — policy'ler `src/Infrastructure/DependencyInjection.cs` içinde `AddAuthorization(options => options.AddPolicy(...))` ile rol eşlemesine bağlanır; sabit `Domain/Constants/Policies.cs`'e eklenir. Çalışan örnek: `CreateSampleEntityCommand`.
- Cookie: `CleanApi_session` — HttpOnly, Secure, SameSite=Lax, 3 gün kayan süre. Login/logout akışı `src/Web/Controllers/V1/AuthController.cs` (`POST /api/v1/Auth/login` → cookie set edilir).
- Aktif kullanıcı bilgisi handler'larda `ICurrentUserService` ile alınır (UserId, FullName, IpAddress).

## Veri Erişimi

- `AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>` (`src/Infrastructure/Persistence/`). Yeni entity: DbSet ekle + configuration sınıfı yaz — `ApplyConfigurationsFromAssembly` otomatik bulur; şema `ToTable("Ad", "Şema")` ile verilir.
- **Repository deseni**: Arayüz Domain'de (`IRepository<T, TId>` tabanı: GetById/GetAll/Find/Any/Count/Add/Update/Remove + serbest LINQ için `Query(predicate, asNoTracking)`), implementasyon Infrastructure'da (`EfRepository<T, TId>` tabanı). Yeni repository DI'a `src/Infrastructure/DependencyInjection.cs` içinde kaydedilir.
- **Repository'ler SaveChanges ÇAĞIRMAZ.** Yazma akışında handler `IUnitOfWork.SaveChangesAsync(ct)` çağırır; çoklu adımda `BeginTransactionAsync` / `CommitTransactionAsync` / `RollbackTransactionAsync` kullanılır (impl: `EfUnitOfWork`).
- **Base entity seçimi** (`src/Domain/Common/`):

| Taban | Ne zaman |
|---|---|
| `BaseEntity<TId>` | Salt kimlikli basit tablo |
| `BaseAuditableEntity` | Created/Modified alanları izlensin |
| `BaseSoftDeletableEntity` | Silme yumuşak olsun (IsDeleted) |

- **4 interceptor otomatik çalışır — bu işleri elle YAPMA:**

| Interceptor | Otomatik iş |
|---|---|
| `AuditableEntitySaveChangesInterceptor` | CreatedBy/CreatedDate/ModifiedBy/ModifiedDate alanlarını doldurur |
| `AuditLogInterceptor` | Değişen entity'lerin eski/yeni değerlerini `dbo.AuditLog`'a yazar |
| `SoftDeleteInterceptor` | `BaseSoftDeletableEntity` türevlerinde `Remove` → `IsDeleted=true` güncellemesine çevirir |
| `DispatchDomainEventsInterceptor` | Entity'lerde biriken domain event'leri SaveChanges akışında publish eder |

- ⚠️ **Global soft-delete query filter YOKTUR.** Soft-deletable entity sorgularında `IsDeleted` filtresini sorguya kendin ekle (örn. `repo.Query(x => !x.IsDeleted)`).
- Migration: `dotnet ef migrations add <Ad> --project src/Infrastructure --startup-project src/Web`. Açılışta `DbInitializer` bekleyen migration'ları uygular; `SeedInitialData=true` ise `AdminSeeder` çalışır (rol + departman + admin kullanıcı).
- Bağlantı seçimi: `Db:ActiveConnection` anahtarı `ConnectionStrings` içinden isimli bağlantıyı seçer (`ConnectionStringResolver`, eksikse açılışta fail-fast).

## Domain Events

Entity, `BaseEvent` türevi event'i kendine ekler (`AddDomainEvent`); `DispatchDomainEventsInterceptor` SaveChanges akışında `MediatrDomainEvent` köprüsüyle MediatR'a publish eder; handler `src/Application/Events/<EventAdı>/<EventAdı>EventHandler.cs`. Event'leri elle publish etme. Örnek: `SampleEntityCreated`.

## Caching

Output cache policy'leri `src/Web/Program.cs`'te tanımlıdır: `revalidate-24h`, `revalidate-2h`, `revalidate-1h`, `revalidate-15m`, `revalidate-5m`. Endpoint'e `[OutputCache(PolicyName = "...")]` ile uygulanır; tag bazlı geçersizleştirme için Application'daki `ICacheInvalidator` arayüzü kullanılır (impl: `src/Web/Services/OutputCacheInvalidator.cs`) — mutasyon handler'ında çağrılır.

## Konfigürasyon

- `src/Web/appsettings.json` tam iskelettir (development varsayılanları); `appsettings.Production.json` yalnızca farkları içerir ve `ASPNETCORE_ENVIRONMENT`'a göre **çalışma zamanında** üzerine bindirilir.
- Bölümler: `Db:ActiveConnection` + `ConnectionStrings`, `SeedInitialData`, `SmtpSettings` (→ `Infrastructure/Services/Models/SmtpSettings`), `FileStorage` (OS'e göre `WindowsRootPath`/`MacRootPath` → `FileStorageOptions`), `Licensing` (AutoMapper/MediatR anahtarları), `Serilog`, `DataProtection` (yalnızca Production; `KeysPath` zorunlu, yoksa açılış hata verir).
- CORS: yalnızca Development'ta `CleanApi FrontEnd` policy'si (`http://localhost:3000`, credentials) — origin listesi `src/Web/DependencyInjection.cs`.

## Loglama ve Audit

- Serilog: async console + günlük dönen dosya (`Logs/log-.txt`); ayarlar appsettings `Serilog` bölümünde.
- Audit hattı üç katmanlıdır, hepsi `dbo.AuditLog`'a yazar: `RequestAuditMiddleware` (HTTP istek izi), `AuditBehavior` (başarılı command/query kaydı), `AuditLogInterceptor` (entity değişim değerleri). Ek olarak `UnhandledExceptionBehaviour` hata payload'u yazar. Yeni bir loglama mekanizması eklemeden önce bu hattı kullan.

## Yeni Feature Checklist (sıralı)

1. **Domain** — entity (+ enum/sabitler) + repository arayüzü.
2. **Infrastructure** — EF configuration + repository impl + `DependencyInjection.cs`'e DI kaydı + `AppDbContext`'e DbSet.
3. **Application** — `Features/<Ad>/Commands|Queries/<UseCase>/`: `record <Ad>Command(...) : IRequest<Result<TResponse>>` + Handler + Validator (+ gerekiyorsa request'e `[Authorize(...)]`) + `Profiles`'a mapping.
4. **Web** — `Controllers/V1/`'e controller: `Mediator.Send` + `Match`.
5. **Migration** — komut yukarıda; migration'ı kullanıcı onayı olmadan üretme.
6. **Doğrulama** — `dotnet build` (0 warning hedefi).

## Kurallar (Do / Don't)

- Validasyon controller'da DEĞİL, Application'daki Validator'dadır.
- Audit alanlarını (CreatedBy vb.) elle set etme — interceptor doldurur.
- Domain event'leri elle publish etme — interceptor dispatch eder.
- SaveChanges'i repository'ye koyma — handler `IUnitOfWork` ile kaydeder.
- Soft-deletable sorgularda `IsDeleted` filtresini unutma (global filter yok).
- Kullanıcıya görünen her mesaj Türkçe ve `ErrorCodes` sabitinden gelir; handler'a string gömme.
- Yeni servis: arayüz Application'a, implementasyon Infrastructure'a, kayıt `Infrastructure/DependencyInjection.cs`'e.
- Tarih/saat için `IDateTimeProvider` / `TimeProvider` kullan; `DateTime.Now` yazma.

## Örnek Dilimi Silme Listesi

Gerçek feature'lara başlarken şu `Sample*` dosyaları ve placeholder'lar silinir/değiştirilir: `Domain/Entities/SchemaName/SampleEntity.cs`, `Domain/Enums/EnumSample.cs`, `Domain/Interfaces/Repositories/ISampleEntityRepository.cs`, `Domain/Constants/ValidationRules.cs` içindeki Sample sabitleri, `Policies.CanCreateSampleEntity` ve `ErrorCodes.AlreadyExists.SampleEntity` örnek sabitleri (kendi policy/hata sabitlerinle değiştir), `Application/GlobalUsings.cs` içindeki Sample namespace satırları, `Infrastructure/Persistence/Configurations/SchemaName/`, `Infrastructure/Persistence/Repositories/SampleEntityRepository.cs`, `Application/Features/SampleEntities/`, `Application/Events/SampleEntityCreated/`, `Web/Controllers/V1/SampleEntityController.cs`. `Department` ve `AuditLog` entity'leri kalıcıdır (seed ve audit altyapısı kullanır). `SchemaName` şema adı gerçek şema adınla değiştirilir.

## Komutlar

```bash
dotnet build                                   # doğrulama (test projesi yok)
dotnet run --project src/Web                   # çalıştır (Development)
dotnet ef migrations add <Ad> --project src/Infrastructure --startup-project src/Web
```

Swagger (yalnızca Development): `https://localhost:7249/swagger` — Health: `GET /health`.
