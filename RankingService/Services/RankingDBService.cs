using RankingService.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace RankingService.Services;

public class RankingDBService
{
    private readonly IMongoCollection<RecipeEntry> _recipeCollection;

    public RankingDBService(IOptions<RankingDatabaseSettings> rankingDatabaseSettings)
    {
        var mongoClient = new MongoClient(rankingDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(rankingDatabaseSettings.Value.DatabaseName);

        _recipeCollection = mongoDatabase.GetCollection<RecipeEntry>(rankingDatabaseSettings.Value.RecipeCollectionName);
    }

    public async Task<List<RecipeEntry>> GetAsync()
    {
        List<RecipeEntry> res = await _recipeCollection.Find(_ => true).ToListAsync();
        return res;
    }

    public async Task<RecipeEntry> GetAsync(string id)
    {
        RecipeEntry res = await _recipeCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        return res;
    }

    public async Task CreateAsync(RecipeEntry entry)
    {
        await _recipeCollection.InsertOneAsync(entry);
    }

    public async Task UpdateAsync(string id, RecipeEntry entry)
    {
        await _recipeCollection.ReplaceOneAsync(x => x.Id == id, entry);
    }

    public async Task DeleteAsync(string id)
    {
        await _recipeCollection.DeleteOneAsync(x => x.Id == id);
    }
    
    public async Task DeleteGivenRecipeIdAsync(string id)
    {
        await _recipeCollection.DeleteOneAsync(x => x.RecipeId == id);
    }

}