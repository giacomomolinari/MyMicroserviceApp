using RankingService.Models;
using RankingService.Services;

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
