using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using RecipeCatalogue;

namespace IntegrationTestCatalogueRanking;

// Intellisys suggests removing RecipeCatalogue dependency and using Program type
// But this is wrong! Because conflict between Program class in RecipeCatalogue and Program class in RankingService!
public class CustomRecipeCatalogueFactory : WebApplicationFactory<RecipeCatalogue.Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(configBuilder =>
        {
            configBuilder.AddInMemoryCollection(
                new[] { new KeyValuePair<string, string?>("RabbitMQHost:ConnectionString", "localhost"),
                new KeyValuePair<string, string?>("RecipesDatabase:ConnectionString", "mongodb://localhost:27017"),
                });
        });
    }
}