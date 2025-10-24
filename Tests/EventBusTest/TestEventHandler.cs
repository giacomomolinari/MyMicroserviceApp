using EventBusInterface;
using System;

namespace EventBusTest;

public class TestEventHandler : IntegrationEventHandler<TestEvent>
{
    public Task HandleAsync(TestEvent @event){
        Console.WriteLine($"Event with Message = {@event.Message} is being handled.");
        return(Task.CompletedTask);
    }

}