using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using RankingService;

namespace IntegrationTestCatalogueRanking;

public class CustomRankingServiceFactory : WebApplicationFactory<RankingService.Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(configBuilder =>
        {
            configBuilder.AddInMemoryCollection(
                new[] { new KeyValuePair<string, string?>("RabbitMQHost:ConnectionString", "localhost"),
                new KeyValuePair<string, string?>("RankingDatabase:ConnectionString", "mongodb://localhost:27017"),
                });
        });
    }
}