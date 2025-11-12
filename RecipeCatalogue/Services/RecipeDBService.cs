using RecipeCatalogue.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace RecipeCatalogue.Services;

public class RecipeDBService
{
    private readonly IMongoCollection<RecipePost> _recipeCollection;

    // gets the recipeDBSettings object from DI via constructor injection
    // then uses it to retrieve tha Recipe table in DB and assign it to _recipeCollection
    public RecipeDBService(IOptions<RecipeDBSettings> recipeDBSettings)
    {
        var mongoClient = new MongoClient(recipeDBSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(recipeDBSettings.Value.DatabaseName);

        _recipeCollection = mongoDatabase.GetCollection<RecipePost>(recipeDBSettings.Value.RecipeCollectionName);
    }

    public async Task<List<RecipePost>> GetAsync() =>
        await _recipeCollection.Find(_ => true).ToListAsync();

    public async Task<RecipePost?> GetAsync(string id) =>
        await _recipeCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(RecipePost newRecipePost) =>
        await _recipeCollection.InsertOneAsync(newRecipePost);

    public async Task UpdateAsync(string id, RecipePost updatedRecipePost) =>
        await _recipeCollection.ReplaceOneAsync(x => x.Id == id, updatedRecipePost);

    public async Task RemoveAsync(string id) =>
        await _recipeCollection.DeleteOneAsync(x => x.Id == id);

    public async Task<long> CountEntries()
    {
        var res = await _recipeCollection.Find(_ => true).CountDocumentsAsync();
        return res;
    } 

}