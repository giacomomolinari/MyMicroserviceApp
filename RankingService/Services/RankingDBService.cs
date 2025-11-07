using RankingService.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RabbitMQ.Client.Exceptions;

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

    public async Task DeleteGivenRecipeIdAsync(string recipeId)
    {
        await _recipeCollection.DeleteOneAsync(x => x.RecipeId == recipeId);
    }

    public async Task IncreaseLikesAsync(string recipeId)
    {
        await _recipeCollection.UpdateOneAsync(x => x.RecipeId == recipeId, Builders<RecipeEntry>.Update.Inc(x => x.Likes, 1));
    }

    public async Task DecreaseLikesAsync(string recipeId)
    {
        await _recipeCollection.UpdateOneAsync(x => x.RecipeId == recipeId, Builders<RecipeEntry>.Update.Inc(x => x.Likes, -1));
    }
    
    public async Task<List<RecipeEntry>> GetRanked(int num, int hours)
    {
        DateTime cutoffTime = DateTime.UtcNow.AddHours(-hours);
        
        List<RecipeEntry> res = await _recipeCollection
        .Find(Builders<RecipeEntry>.Filter.Gte(x => x.RecipeCreationDate, cutoffTime))
        .Sort(Builders<RecipeEntry>.Sort.Descending(x => x.Likes))
        .Limit(num)
        .ToListAsync();

        return res;
    }

}