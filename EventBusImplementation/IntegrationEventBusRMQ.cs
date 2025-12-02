using RabbitMQ.Client;
using EventBusInterface;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;
using System.Collections.ObjectModel;
using System.Threading.Channels;
using RabbitMQ.Client.Events;
using System.Globalization;
using SubsManagerInterface;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace EventBusImplementation;


public class IntegrationEventBusRMQ : IntegrationEventBus
{
    private string _connectionString;
    private string _brokerName;
    private string _serviceName;
    private IConnection? _persistentConsumeConnection; // supposed to be persistent, used for consumption
    private IChannel? _consumeChannel;          // supposed to be persistent, used for consumption

    private ISubsManager _subsManager;

    private IServiceProvider _serviceProvider; 

    // inject both parameters from config files in appsettings.json, or compose.yml
    // For testing, hardcode them and pass them to constructor
    // NOTE: persistentConnection should be a wrapper around IConnection that actually makes it
    //       persistent! For now use normal connection...
    public IntegrationEventBusRMQ(string connectionString, string brokerName, string serviceName, ISubsManager subsManager, IServiceProvider serviceProvider)
    {
        _connectionString = connectionString;
        _brokerName = brokerName;
        _serviceName = serviceName;
        _subsManager = subsManager;
        _serviceProvider = serviceProvider;
    }

    // this deals with the RabbitMQ setup logic that in the original eShop is done by the
    // constructor. Done to simplify things after channel creation became Async in RabbitMQ.
    // This does nothing on multiple calls (should allow it to re-establish connection if closed...)
    public async Task<string> EstablishConsumeConnection()
    {
        if (_persistentConsumeConnection == null)
        {
            // create connection (should use a wrapper to make it actually persistent...)
            var factory = new ConnectionFactory { HostName = _connectionString };
            _persistentConsumeConnection = await factory.CreateConnectionAsync();

            // create channel
            _consumeChannel = await _persistentConsumeConnection.CreateChannelAsync();

            // declare echange (app-level)
            await _consumeChannel.ExchangeDeclareAsync(exchange: _brokerName, type: ExchangeType.Direct);

            // declare queue (one for each service)
            await _consumeChannel.QueueDeclareAsync(queue: _serviceName, durable: true, exclusive: false, autoDelete: false);

            // create consumer
            var consumer = new AsyncEventingBasicConsumer(_consumeChannel);

            // Add generic callback method
            // Currently just a stub, need to add an effective generic callback that retrieves a handler
            // from a _subsManager
            consumer.ReceivedAsync += GenericHandler;

            // start consuming the queue 
            string res = await _consumeChannel.BasicConsumeAsync(queue: _serviceName, autoAck: true, consumer: consumer);

            return res;
        }
        else return "Connection already established";
    }


    private async Task GenericHandler(object? model, BasicDeliverEventArgs ea)
    {
        string eventName = ea.RoutingKey;
        var body = ea.Body.ToArray();
        string serializedEvent = Encoding.UTF8.GetString(body);
        // get the concrete event type of the event received
        Type eventType = _subsManager.getEventType(eventName);
        var @event = JsonSerializer.Deserialize(serializedEvent, eventType);

          
        // Ask _subsManager for its handler type
        List<Type>? handlerList = _subsManager.getHandlerTypeIfSubscribed(eventName);

        if (handlerList == null)
        {
            throw new NullReferenceException("No handler available for subscribed event.");
        } else {
    
            foreach (Type handlerType in handlerList)
            {
                // Request handler from DI and use it to invoke HandleAsync method
                object concreteHandler = _serviceProvider.GetRequiredService(handlerType);
                MethodInfo? concreteMethod = handlerType.GetMethod("HandleAsync");
                if (concreteMethod == null)
                {
                    throw new NullReferenceException($"Could not retrieve HandleAsync method for halderType = {handlerType}");
                }
                await (Task) concreteMethod.Invoke(concreteHandler, new object[] { @event});
            }
        }
        
        

    }

    public async Task Publish(IntegrationEvent @event)
    {
        var eventName = @event.GetType().Name;
        var factory = new ConnectionFactory() { HostName = _connectionString };

        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        // define a direct exchange, so messages are sent to queues whose routingkey 
        // exactly matches that of the message
        await channel.ExchangeDeclareAsync(exchange: _brokerName, type: ExchangeType.Direct);

        // IMPORTANT NOTE: JsonSerializer does not see the runtime type of the object!
        // So when this executes on a concrete event type, it ignores all information
        // beyond the Id and CreationDate which are included in the interface. That's
        // why I have to pass it the type of @event explicitly!
        string message = JsonSerializer.Serialize(@event, @event.GetType()); // message is string containing json object @event
        var body = Encoding.UTF8.GetBytes(message); // encode that string into bytes as body

        await channel.BasicPublishAsync(
            exchange: _brokerName,
            routingKey: eventName,
            body: body
            );

    }

    public async Task Subscribe<T, TH>()
    where T : IntegrationEvent
    where TH : IntegrationEventHandler<T>
    {
        if (_consumeChannel == null)
        {
            throw new InvalidOperationException("Consume connection not established yet.");
        }

        string eventName = typeof(T).Name;
        // bind service queue to the app-level exchange, using event type as routing key
        await _consumeChannel.QueueBindAsync(queue: _serviceName, exchange: _brokerName, routingKey: eventName);

        // register subscription in a subsManager tasked with retrieving the handler upon request
        _subsManager.addSubscription<T, TH>();
    }

    public async Task Unsubscribe<T, TH>()
    where T : IntegrationEvent
    where TH : IntegrationEventHandler<T>
    {
        // implement w/  RabbitMQ
    }


}
