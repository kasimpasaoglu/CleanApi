#pragma warning disable CA1873

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application.Common.Behaviours;

public class UnhandledExceptionBehaviour<TRequest, TResponse>(
    ILogger<UnhandledExceptionBehaviour<TRequest, TResponse>> logger,
    ICurrentUserService currentUser,
    IAuditLogWriter auditLogWriter,
    IRequestCancellationClassifier cancellationClassifier
    ) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next(cancellationToken);
        }
        catch (Exception ex) when (cancellationClassifier.IsCancelled(ex, cancellationToken))
        {
            throw new OperationCanceledException(cancellationToken);
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;
            var userId = currentUser.UserId ?? "anonymous";
            var userName = currentUser.FullName ?? "anonymous";
            var ipAddress = currentUser.IpAddress ?? "unknown";

            var safePayload = BuildSafeErrorPayload(request, ex);

            logger.LogError(ex,
                "Unhandled Exception for Request {RequestName} | User: {UserName} ({UserId}) | Payload: {Payload}",
                requestName, userName, userId, safePayload);

            await auditLogWriter.WriteAsync(
                tableName: "UnhandledExceptions",
                action: requestName,
                performedById: userId,
                perfomedByName: userName,
                perfomedByIp: ipAddress,
                newValues: safePayload,
                oldValues: null,
                keyValues: null);

            throw;
        }
    }
    
    private static string BuildSafeErrorPayload(TRequest request, Exception ex)
    {
        // Burada amaç: full dump değil, güvenli + kısa özet
        // FileContent/base64/token/password vs. redacted + truncate

        var node = JsonSerializer.SerializeToNode(request, new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });

        // SerializeToNode başarısız olursa bile exception payload’u kaybetmeyelim
        var payloadObj = new
        {
            RequestType = typeof(TRequest).Name,
            ExceptionType = ex.GetType().Name,
            ExceptionMessage = ex.Message,
            Request = node
        };

        var json = JsonSerializer.Serialize(payloadObj);

        // global truncate
        const int maxLen = 4000;
        if (json.Length > maxLen) json = json[..maxLen] + "...(truncated)";

        // hızlı redaction (string içinde) – kaba ama etkili
        json = json.Replace("FileContent", "FileContent<redacted>", StringComparison.OrdinalIgnoreCase);

        return json;
    }
}