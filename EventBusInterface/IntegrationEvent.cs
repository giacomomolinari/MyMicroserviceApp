namespace EventBusInterface;

public abstract record IntegrationEvent
{
    public Guid Id {get; init;}

    public DateTime CreationDate {get; init;}
}