namespace Application.Common.Interfaces;

/// <summary>
/// exception'a ve cancellationToken'a bakarak, exceptiona sebep olan hatanin istegin iptal edilmesinden kaynakli olup olmadigini kontrol eder,
/// Hem Sql hem de Operation hatalarini yaklamak icin Infra katmaninda somutlanir
/// </summary>
public interface IRequestCancellationClassifier
{
    bool IsCancelled(Exception exception, CancellationToken cancellationToken);
}