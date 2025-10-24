namespace EventBusInterface;
public interface IntegrationEventHandler<EventT>
where EventT: IntegrationEvent
{
    // INTERESTING NOTE: No need to tag this as "async" because it's an interface!
    // The async tag is only used at implementation level. As far as the interface is concerned,
    // all that matters is that this method returns a Task!
    Task HandleAsync(EventT @event);
}