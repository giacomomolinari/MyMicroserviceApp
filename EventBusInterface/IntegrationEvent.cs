namespace EventBusInterface;

public abstract class IntegrationEvent
{
    public Guid Id {get; init;}

    public DateTime CreationDate {get; init;}
}