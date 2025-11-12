using RecipeCatalogue.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace RecipeCatalogue.Services;

public class LikesCollectionService
{
    private readonly IMongoCollection<RecipeLike> _likesCollection;

    // gets the recipeDBSettings object from DI via constructor injection
    // then uses it to retrieve tha Recipe table in DB and assign it to _recipeCollection
    public LikesCollectionService(IOptions<RecipeDBSettings> recipeDBSettings)
    {
        var mongoClient = new MongoClient(recipeDBSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(recipeDBSettings.Value.DatabaseName);

        _likesCollection = mongoDatabase.GetCollection<RecipeLike>(recipeDBSettings.Value.LikesCollectionName);
    }

    public async Task<List<RecipeLike>> GetAsync() =>
        await _likesCollection.Find(_ => true).ToListAsync();

    public async Task<RecipeLike?> GetAsync(string id) =>
        await _likesCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(RecipeLike newRecipeLike) =>
        await _likesCollection.InsertOneAsync(newRecipeLike);

    public async Task UpdateAsync(string id, RecipeLike updatedRecipeLike) =>
        await _likesCollection.ReplaceOneAsync(x => x.Id == id, updatedRecipeLike);

    public async Task RemoveAsync(string id) =>
        await _likesCollection.DeleteOneAsync(x => x.Id == id);

    
    public async Task<List<RecipeLike>> GetAllLikesForRecipe (string recipeId) =>
        await _likesCollection.Find(x => x.RecipeId == recipeId).ToListAsync();
        
    public async Task<long> CountEntries()
    {
        var res = await _likesCollection.Find(_ => true).CountDocumentsAsync();
        return res;
    } 
}