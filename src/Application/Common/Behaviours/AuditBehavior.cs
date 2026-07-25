#pragma warning disable CA1873

namespace Application.Common.Behaviours;

public class AuditBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<AuditBehavior<TRequest, TResponse>> _logger;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditLogWriter _auditLogWriter;

    /// <summary>
    /// MediatR pipeline'inda requestin basariyla tamamlanmasindan sonra
    /// AuditLog tablosuna (payload/values olmadan) bir basari kaydi ekler.
    /// Serilog'a basari INF satiri YAZMAZ — duplicate log'u onlemek icin
    /// HTTP istek izi RequestAuditMiddleware'de tek noktada toplanir;
    /// "kim hangi sorguyu cekti / komutu calistirdi" izi tum IRequest'ler
    /// (Command + Query) icin dbo.AuditLog tablosundan uretilir (KVKK / ic denetim).
    /// Failure yolunda (Result.IsFailure) Serilog'a WRN dusurulur — read sorgu
    /// hatasi dahil tani degeri tasidigi icin Command/Query farki gozetmez.
    /// Thrown exception'lar UnhandledExceptionBehaviour'da handle edilir.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="currentUser"></param>
    /// <param name="auditLogWriter"></param>
    public AuditBehavior(
        ILogger<AuditBehavior<TRequest, TResponse>> logger,
        ICurrentUserService currentUser, IAuditLogWriter auditLogWriter)
    {
        _logger = logger;
        _currentUser = currentUser;
        _auditLogWriter = auditLogWriter;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _currentUser.UserId ?? "anonymous";
        var userName = _currentUser.FullName ?? "anonymous";
        var ipAddress = _currentUser.IpAddress ?? "unknown";
        var timestamp = DateTimeOffset.UtcNow;

        var response = await next(cancellationToken);

        if (response is Result result && result.IsFailure)
        {
            _logger.LogWarning("{Request} failed for {UserName} at {Timestamp}: {ErrorDesc}", requestName, userName, timestamp, result.Error.Description);
            return response;
        }

        await _auditLogWriter.WriteAsync(
            tableName: "Success",
            action: requestName,
            performedById: userId,
            perfomedByName: userName,
            perfomedByIp: ipAddress,
            newValues: null,
            oldValues: null,
            keyValues: null);


        return response;
    }
}