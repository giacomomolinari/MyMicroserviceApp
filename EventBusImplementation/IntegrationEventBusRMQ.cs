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

namespace EventBusImplementation;


public class IntegrationEventBusRMQ : IntegrationEventBus
{
    private string _connectionString;
    private string _brokerName;
    private string _serviceName;
    private IConnection? _persistentConsumeConnection; // supposed to be persistent, used for consumption
    private IChannel? _consumeChannel;          // supposed to be persistent, used for consumption

    // inject both parameters from config files in appsettings.json, or compose.yml
    // For testing, hardcode them and pass them to constructor
    // NOTE: persistentConnection should be a wrapper around IConnection that actually makes it
    //       persistent! For now use normal connection...
    public IntegrationEventBusRMQ(string connectionString, string brokerName, string serviceName)
    {
        _connectionString = connectionString;
        _brokerName = brokerName;
        _serviceName = serviceName;
    }

    // this deals with the RabbitMQ setup logic that in the original eShop is done by the
    // constructor. Done to simplify things after channel creation became Async in RabbitMQ.
    // does nothing on multiple calls (should allow to re-establish connection if closed...)
    public async void EstablishConsumeConnection()
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

            // add generic callback method
            consumer.ReceivedAsync += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"{_serviceName} received message: {message}");
                return Task.CompletedTask;
            };

            // start consuming the queue
           
        }
    }

    public async void Publish(IntegrationEvent @event)
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

    public void Subscribe<T, TH>()
    where T : IntegrationEvent
    where TH : IntegrationEventHandler<T>
    {
        // implement w/ RabbitMQ
    }

    public void Unsubscribe<T, TH>()
    where T : IntegrationEvent
    where TH : IntegrationEventHandler<T>
    {
        // implement w/  RabbitMQ
    }


}
