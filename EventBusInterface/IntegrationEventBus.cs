namespace EventBusInterface;

public interface IntegrationEventBus
{
    Task Publish(IntegrationEvent @event);

    Task Subscribe<T, TH>()
    where T: IntegrationEvent
    where TH: IntegrationEventHandler<T>;

    Task Unsubscribe<T, TH>()
    where T : IntegrationEvent
    where TH : IntegrationEventHandler<T>;

    Task<string> EstablishConsumeConnection();
}
