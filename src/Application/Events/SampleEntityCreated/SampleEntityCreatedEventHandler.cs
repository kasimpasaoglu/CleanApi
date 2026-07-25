namespace Application.Events.SampleEntityCreated;

public class SampleEntityCreatedEventHandler(
    ILogger<SampleEntityCreatedEventHandler> logger,
    ICurrentUserService currentUserService) : INotificationHandler<SampleEntityCreatedDomainEvent>
{
    public Task Handle(SampleEntityCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Log From Domain Event - EntityCreatedBy : {createdBy}, EntityInfo : {name}", currentUserService.FullName, notification.Name);

        return Task.CompletedTask;
    }
}