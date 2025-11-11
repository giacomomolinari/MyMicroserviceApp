using Xunit;
using Xunit.Abstractions;
using System.Data.Common;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System.Text.Json;
using EventBusInterface;
using EventBusImplementation;
using SubsManagerInterface;
using SubsManagerImplementation;
using System.Security.Authentication.ExtendedProtection;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;

namespace EventBusTest;

public class TestRabbitMQBus
{

    // used to hardcode connection details for testing
    private string connectionString = "localhost";
    private string brokerName = "testBroker";
    private string serviceName = "testService";

    private ISubsManager subsManager = new SubsManagerStub();

    private ITestOutputHelper _output;

    public TestRabbitMQBus(ITestOutputHelper output)
    {
        _output = output;
    }

    // Test that the local (testing) implementation of IntegrationEvent 
    // and IntegrationEventHandler compile without errors
    [Fact]
    public void TestEventClasses()
    {
        TestEvent testEvent = new TestEvent("TEST");
        var tcs = new TaskCompletionSource<TestEvent>();
        TestEventHandler testEventHandler = new TestEventHandler(tcs);
        Console.WriteLine("Hello World!");
        var res = testEventHandler.HandleAsync(testEvent);

        Assert.True(res.IsCompleted, "I should not be false");
    }



    // Test implementation of Publish method:
    /// <summary>
    ///  1. Create test exchange and queue bound to it
    ///  2. Start consuming on that queue
    ///  3. Publish TestEvent to the exchange, using EventBusImplementation in RabbitMQ
    ///  4. Await for callback to fire
    /// </summary>
    [Fact]
    public async void TestPublish()
    {
        // event to be published
        TestEvent testEvent = new TestEvent("TEST MESSAGE - TestPublish");

        // Use RabbitMQ directly to subscribe to test exchange and receive event 
        var eventName = testEvent.GetType().Name;
        var factory = new ConnectionFactory() { HostName = connectionString };

        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        // need to define exchange here in order to bind my queue to it!
        await channel.ExchangeDeclareAsync(exchange: brokerName, type: ExchangeType.Direct);

        // declares an auto-generated fresh queue and stores its name
        QueueDeclareOk queueDeclareResult = await channel.QueueDeclareAsync();
        string queueName = queueDeclareResult.QueueName;

        // bind it to testBroker exchange with routing key "TestEvent"
        // i.e. subscribe to events of type TestEvent
        await channel.QueueBindAsync(queue: queueName, exchange: brokerName, routingKey: eventName);

        // used to signal when message is received, i.e. when callback has been called
        var tcs = new TaskCompletionSource<string>();

        // create new consumer with an appropriate callback function
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            tcs.TrySetResult(message);

        };

        // have consumer consume from the queue created above
        await channel.BasicConsumeAsync(queueName, autoAck: true, consumer: consumer);

        // create service provider (needed to initialize IntegrationEventBusRMQ). No need to actually add services for this test
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();

        // Publish testEvent 
        IntegrationEventBus myBus = new IntegrationEventBusRMQ(connectionString, brokerName, serviceName, subsManager, serviceProvider);
        await myBus.Publish(testEvent);

        // wait 5 seconds, and if they pass, change the value of cts
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var receivedMessage = await tcs.Task.WaitAsync(cts.Token);
        var receivedEvent = JsonSerializer.Deserialize<TestEvent>(receivedMessage);

        // deals with the fact that TestEvent is a class, not a record, so Assert.Equal fails
        receivedEvent.Should().BeEquivalentTo(receivedEvent);
    }

    // Test implementation of Subscribe method:
    /// <summary>
    ///  1. Subscribe to TestEvent type events using Subscribe method
    ///  2. Publish a TestEvent 
    /// </summary>
    [Fact]
    public async void TestSubscribe()
    {
        // create service provider (needed to initialize IntegrationEventBusRMQ). 
        var services = new ServiceCollection();
        // add event handler
        services.AddTransient<TestEventHandler>();
        var tcs = new TaskCompletionSource<TestEvent>();
        services.AddSingleton(tcs);

        var serviceProvider = services.BuildServiceProvider();

        // create eventBus and start up consumer connection
        IntegrationEventBusRMQ myBus = new IntegrationEventBusRMQ(connectionString, brokerName, serviceName, subsManager, serviceProvider);
        string consumerTag = await myBus.EstablishConsumeConnection();

        // subscribe to TestEvent events
        await myBus.Subscribe<TestEvent, TestEventHandler>();

        // event to be published
        TestEvent testEvent = new TestEvent("TEST MESSAGE - Test Subscribe");
        await myBus.Publish(testEvent);

        var received = await tcs.Task.WaitAsync(TimeSpan.FromSeconds(5));

        received.Should().BeEquivalentTo(testEvent);
    }

}