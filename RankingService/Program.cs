using RankingService.Models;
using RankingService.Services;

using SubsManagerInterface;
using SubsManagerImplementation;
using EventBusInterface;
using EventBusImplementation;
using EventBusInterface;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://*:80");
// Add services to the container.


// in Controllers, serialize/deserialize in case-insensitive way
builder.Services.AddControllers().AddJsonOptions(
        options => options.JsonSerializerOptions.PropertyNamingPolicy = null);



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// setup RankingDatabaseSettings object in the DI with the values taken from appsettings.json
builder.Services.Configure<RankingDatabaseSettings>(
    builder.Configuration.GetSection("RankingDatabase")
);

// add RankingDBService to the DI so that controller can use it to interact with the DB
builder.Services.AddSingleton<RankingDBService>();

// Add SubsManager to the DI
builder.Services.AddSingleton<ISubsManager, SubsManagerStub>();

// Add EventBus to the DI, passing the settings as arguments
builder.Services.AddSingleton<IntegrationEventBus>(serviceProvider =>
{
    var subsManager = serviceProvider.GetRequiredService<ISubsManager>();

    return new IntegrationEventBusRMQ(
        connectionString: builder.Configuration["RabbitMQHost:ConnectionString"],
        brokerName: builder.Configuration["RabbitMQHost:BrokerName"],
        serviceName: builder.Configuration["RabbitMQHost:ServiceName"],
        subsManager: subsManager,
        serviceProvider: serviceProvider
        );
});

// ADD EVENT HANDLERS TO DI
builder.Services.AddTransient<RecipeCreatedEventHandler>();

var app = builder.Build();

// subscribe to RecipeCreatedEvent events via the singleton IntegrationEventBus defined above
IntegrationEventBus myBus = app.Services.GetRequiredService<IntegrationEventBus>();

await myBus.EstablishConsumeConnection();
await myBus.Subscribe<RecipeCreatedEvent, RecipeCreatedEventHandler>();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run(); // blocks thread
