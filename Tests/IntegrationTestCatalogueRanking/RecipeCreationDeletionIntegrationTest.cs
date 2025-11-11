using System.Threading.Tasks;

namespace IntegrationTestCatalogueRanking;

public class RecipeCreationDeletionIntegratgionTest : IClassFixture<CustomRecipeCatalogueFactory>
{

    private readonly CustomRecipeCatalogueFactory _recipeCatalogueFactory;
    private readonly HttpClient _recipeClient;

    public RecipeCreationDeletionIntegratgionTest(CustomRecipeCatalogueFactory recipeCatalogueFactory)
    {
        _recipeCatalogueFactory = recipeCatalogueFactory;
        _recipeClient = _recipeCatalogueFactory.CreateClient();
    }



    // First test that service is running and can access its database by sending a GET request
    // In effect this tests the CustomRecipeCatalogueFactory class
    [Fact]
    public async Task RecipeServiceWorking()
    {

        var response = await _recipeClient.GetAsync("api/RecipePosts");

        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());

    }
    


}