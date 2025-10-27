using EventBusInterface;
using System;

namespace EventBusTest;

public class TestEventHandler : IntegrationEventHandler<TestEvent>
{

    private TaskCompletionSource<TestEvent> _tcs;

    public TestEventHandler(TaskCompletionSource<TestEvent> tcs)
    {
        _tcs = tcs;
    }

    public async Task HandleAsync(TestEvent @event){
        Console.WriteLine($"Event with Message = {@event.Message} is being handled by its custom handler.");
        _tcs.TrySetResult(@event);
    }

}