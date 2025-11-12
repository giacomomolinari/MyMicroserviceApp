using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using RecipeCatalogue.Models;

namespace IntegrationTestCatalogueRanking;

public class RecipeCreationDeletionIntegrationTest : IClassFixture<CustomRecipeCatalogueFactory>
{

    private readonly CustomRecipeCatalogueFactory _recipeCatalogueFactory;
    private readonly HttpClient _recipeClient;
   

    public RecipeCreationDeletionIntegrationTest(CustomRecipeCatalogueFactory recipeCatalogueFactory)
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

    // Test creation and deletion of recipes in RecipeCatalogue
    [Fact]
    public async Task RecipeCreationDeletion()
    {
        // get initial number of recipe posts
        long initialCount = await getRecipePostCount();

        // Create and post new recipe   
        RecipeInput testRecipe = new RecipeInput();

        using StringContent jsonContent = new(
            JsonSerializer.Serialize(testRecipe),
            Encoding.UTF8,
            "application/json");

        var postResponse = await _recipeClient.PostAsync("api/RecipePosts", jsonContent);
        postResponse.EnsureSuccessStatusCode();

        // get MongoDB Id assigned to new recipe
        string postResponseString = await postResponse.Content.ReadAsStringAsync();
        string newPostId = JsonSerializer.Deserialize<RecipePost>(postResponseString).Id;


        // get updated number of recipe posts, and check it is initial number + 1
        long updatedCount = await getRecipePostCount();
        Assert.Equal(initialCount + 1, updatedCount);

        // Delete new recipe
        var deleteResponse = await _recipeClient.DeleteAsync($"api/RecipePosts/{newPostId}");
        deleteResponse.EnsureSuccessStatusCode();

        // check final number of recipes is same as initial
        long finalCount = await getRecipePostCount();
        Assert.Equal(initialCount, finalCount);
    }


    public async Task<long> getRecipePostCount()
    {
        var getCountResponse = await _recipeClient.GetAsync("api/RecipePosts/count");
        getCountResponse.EnsureSuccessStatusCode();

        string getCountResponseString = await getCountResponse.Content.ReadAsStringAsync();
        return long.Parse(getCountResponseString);
    }

}