using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using RecipeCatalogue.Models;
using Xunit;

// needed to avoid integration events from different tests interfering with each other
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace IntegrationTestCatalogueRanking;

public class RecipeCreationDeletionIntegrationTest : IClassFixture<CustomRecipeCatalogueFactory>, IClassFixture<CustomRankingServiceFactory>
{

    private readonly CustomRecipeCatalogueFactory _recipeCatalogueFactory;
    private readonly CustomRankingServiceFactory _rankingServiceFactory;

    private readonly HttpClient _recipeClient;
    private readonly HttpClient _rankingsClient;


    public RecipeCreationDeletionIntegrationTest(CustomRecipeCatalogueFactory recipeCatalogueFactory, CustomRankingServiceFactory rankingServiceFactory)
    {
        _recipeCatalogueFactory = recipeCatalogueFactory;
        _recipeClient = _recipeCatalogueFactory.CreateClient();

        _rankingServiceFactory = rankingServiceFactory;
        _rankingsClient = _rankingServiceFactory.CreateClient();
    }


    // First test that RecipeCatalogue is running and can access its database by sending a GET request
    // In effect this tests the CustomRecipeCatalogueFactory class
    [Fact]
    public async Task RecipeServiceWorking()
    {

        var response = await _recipeClient.GetAsync("api/RecipePosts");

        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());

    }


    // Test that RankingService is running and can access its database by sending a GET request
    [Fact]
    public async Task RankingServiceWorking()
    {

        var response = await _rankingsClient.GetAsync("api/RankingService");

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


    // Test publishing and reception of RecipeCreatedEvent and RecipeDeletedEvent events
    // between RecipeCatalogue (publisher) and RankingService (subscriber)
    [Fact]
    public async Task RecipeCreationDeletionIntegrationEventsTest()
    {
        // get and save initial count of recipe entries in Ranking database
        long initialCount = await getRankingEntriesCount();

        // Create and post new recipe to RecipeCatalogue database  
        RecipeInput testRecipe = new RecipeInput();

        using StringContent jsonContent = new(
            JsonSerializer.Serialize(testRecipe),
            Encoding.UTF8,
            "application/json");

        var postResponse = await _recipeClient.PostAsync("api/RecipePosts", jsonContent);
        postResponse.EnsureSuccessStatusCode();

        // get MongoDB Id assigned to new recipe (needed for deletion)
        string postResponseString = await postResponse.Content.ReadAsStringAsync();
        string newPostId = JsonSerializer.Deserialize<RecipePost>(postResponseString).Id;

        
        await Task.Delay(1000); // wait (to give Ranking service time to consume event) 

        // check updated count of recipe entries in Ranking database equals initial count + 1
        long updatedCount = await getRankingEntriesCount();
        Assert.Equal(initialCount + 1, updatedCount);

        // delete new recipe from RecipeCatalogue database
        var deleteResponse = await _recipeClient.DeleteAsync($"api/RecipePosts/{newPostId}");
        deleteResponse.EnsureSuccessStatusCode();

        
        await Task.Delay(1000); // wait (to give Ranking service time to consume event)

        // check final count of recipe entries in Ranking database equals initial count
        long finalCount = await getRankingEntriesCount();
        Assert.Equal(initialCount, finalCount);

    }

    private async Task<long> getRecipePostCount()
    {
        var getCountResponse = await _recipeClient.GetAsync("api/RecipePosts/count");
        getCountResponse.EnsureSuccessStatusCode();

        string getCountResponseString = await getCountResponse.Content.ReadAsStringAsync();
        return long.Parse(getCountResponseString);
    }

    private async Task<long> getRankingEntriesCount()
    {
        var getCountResponse = await _rankingsClient.GetAsync("api/RankingService/count");
        getCountResponse.EnsureSuccessStatusCode();

        string getCountResponseString = await getCountResponse.Content.ReadAsStringAsync();
        return long.Parse(getCountResponseString);
    }

}