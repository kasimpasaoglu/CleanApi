using Microsoft.Data.SqlClient;

namespace Infrastructure.Services;

public class RequestCancellationClassifier : IRequestCancellationClassifier
{
    public bool IsCancelled(Exception exception, CancellationToken cancellationToken)
    {
        if (!cancellationToken.IsCancellationRequested)
            return false;

        return exception is OperationCanceledException
            or TaskCanceledException
            or SqlException;
    }
}