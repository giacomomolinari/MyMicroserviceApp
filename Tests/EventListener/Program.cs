using SubsManagerInterface;
using SubsManagerImplementation;
using EventBusInterface;
using EventBusImplementation;
using Microsoft.Extensions.DependencyInjection;
using EventListener;

// unlike recipes, which runs in the same Docker virtual network as the rabbitmq 
// host, this listener app runs in localhost and so should access rabbitmq
// via its localhost port
string connectionString = "localhost";

string brokerName = "MyMicroserviceApp";
string serviceName = "eventListener";
ISubsManager subsManager = new SubsManagerStub();

// create service provider (needed to initialize IntegrationEventBusRMQ)
var services = new ServiceCollection();
// add event handler
services.AddTransient<RecipeCreatedEventHandler>();

var serviceProvider = services.BuildServiceProvider();

// create bus and establish consume connection
IntegrationEventBus myBus = new IntegrationEventBusRMQ(connectionString, brokerName, serviceName, subsManager, serviceProvider);
await myBus.EstablishConsumeConnection();

// subscribe to RecipeCreatedEvent events
await myBus.Subscribe<RecipeCreatedEvent, RecipeCreatedEventHandler>();

Console.WriteLine("Press [Enter] to exit");
Console.ReadLine();