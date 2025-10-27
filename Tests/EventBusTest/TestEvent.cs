using EventBusInterface;
using System;

namespace EventBusTest;

public record TestEvent: IntegrationEvent
{

    public string Message { get; set; }
    
    
    
    public TestEvent(string Message)
    {
        this.Id = Guid.NewGuid();
        this.CreationDate = DateTime.UtcNow;

        this.Message = Message;
    }
}