using Microsoft.EntityFrameworkCore;
using RecipeCatalogue.Models;
using RecipeCatalogue.Services;

using SubsManagerInterface;
using SubsManagerImplementation;
using EventBusInterface;
using EventBusImplementation;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://*:80");

// Add controller as a Transient service to the DI
builder.Services.AddControllers()
    .AddJsonOptions( // makes it so property names in the web API's json
                     // match the names in the RecipePost class
        options => options.JsonSerializerOptions.PropertyNamingPolicy = null);


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add a unique instance of the DB connection service to the
// DI. Singleton means we create this once and use it for the
// entire lifespan of the application
builder.Services.AddSingleton<RecipeDBService>();

builder.Services.AddSingleton<LikesCollectionService>();

// In memory store, USED FOR TESTING
//builder.Services.AddDbContext<RecipeCatalogueContext>(opt => 
//   opt.UseInMemoryDatabase("RecipeList"));

// Adds the RecipeDBSettings to the DI so that we can access
// the settings from everywhere in the app
builder.Services.Configure<RecipeDBSettings>(
    builder.Configuration.GetSection("RecipesDatabase"));

// Adds the RabbitMQSettings to the DI as well.
// NOTE: Actually now I think this may not be needed, since I need to manually pass the arguments to the 
//       EventBusRabbitMQ constructor anyway...
builder.Services.Configure<RabbitMQSettings>(
    builder.Configuration.GetSection("RabbitMQHost")
);

// Add SubsManager to the DI
builder.Services.AddSingleton<ISubsManager, SubsManagerStub>();

// Add EventBus to the DI, passing the settings a
builder.Services.AddSingleton<IntegrationEventBus>(serviceProvider =>
{
    var rabbitMQSettings = serviceProvider.GetRequiredService<IOptions<RabbitMQSettings>>().Value;
    var subsManager = serviceProvider.GetRequiredService<ISubsManager>();

    return new IntegrationEventBusRMQ(
        connectionString: rabbitMQSettings.ConnectionString,
        brokerName: rabbitMQSettings.BrokerName,
        serviceName: rabbitMQSettings.ServiceName,
        subsManager: subsManager,
        serviceProvider: serviceProvider
        );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) 
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
