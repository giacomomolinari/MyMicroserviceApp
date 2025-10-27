using EventBusInterface;
using System;

namespace EventBusTest;

public class TestEventHandler : IntegrationEventHandler<TestEvent>
{
    public async Task HandleAsync(TestEvent @event){
        Console.WriteLine($"Event with Message = {@event.Message} is being handled by its custom handler.");
    }

}