namespace Domain.Constants;

public abstract class Policies
{
    // Örnek policy: rol eşlemesi Infrastructure/DependencyInjection.cs içinde yapılır,
    // request record'una [Authorize(Policy = Policies.CanCreateSampleEntity)] ile uygulanır.
    public const string CanCreateSampleEntity = nameof(CanCreateSampleEntity);
}
