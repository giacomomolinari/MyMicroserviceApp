using Microsoft.EntityFrameworkCore;
using RecipeCatalogue.Models;
using RecipeCatalogue.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://*:80");

// Add services to the container.
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

// In memory store
//builder.Services.AddDbContext<RecipeCatalogueContext>(opt => 
//   opt.UseInMemoryDatabase("RecipeList"));

// Adds the RecipeDBSettings to the DI so that we can access
// the settings from everywhere in the app
builder.Services.Configure<RecipeDBSettings>(
    builder.Configuration.GetSection("RecipesDatabase"));

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
