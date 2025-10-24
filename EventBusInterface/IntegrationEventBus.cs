namespace EventBusInterface;

public interface IntegrationEventBus
{
    void Publish(IntegrationEvent @event);

    void Subscribe<T, TH>()
    where T: IntegrationEvent
    where TH: IntegrationEventHandler<T>;

    void Unsubscribe<T,TH>()
    where T: IntegrationEvent
    where TH: IntegrationEventHandler<T>;
}
