using RecipeCatalogue.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration.UserSecrets;
using MongoDB.Driver.Linq;
using MongoDB.Bson;

namespace RecipeCatalogue.Services;

public class RecipeDBService
{
    private readonly IMongoCollection<RecipePost> _recipeCollection;

    private readonly IMongoCollection<RecipeTag> _tagsCollection;

    private readonly IMongoCollection<RecipeLike> _likesCollection;

    private readonly List<string> _tagsInitList = ["vegan", "vegetarian", "high-fibre", "high-protein", "quick", "easy"];

    // gets the recipeDBSettings object from DI via constructor injection
    // then uses it to retrieve tha Recipe table in DB and assign it to _recipeCollection
    public RecipeDBService(IOptions<RecipeDBSettings> recipeDBSettings)
    {
        var mongoClient = new MongoClient(recipeDBSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(recipeDBSettings.Value.DatabaseName);

        _recipeCollection = mongoDatabase.GetCollection<RecipePost>(recipeDBSettings.Value.RecipeCollectionName);

        _tagsCollection = mongoDatabase.GetCollection<RecipeTag>(recipeDBSettings.Value.TagsCollectionName);

        _likesCollection = mongoDatabase.GetCollection<RecipeLike>(recipeDBSettings.Value.LikesCollectionName);
    }

    public async Task TagsCollectionInit()
    {
        long tagsNumber = await _tagsCollection.CountDocumentsAsync(_ => true);
        if (tagsNumber == 0)
        {
            await _tagsCollection.InsertManyAsync(_tagsInitList.Select(x => new RecipeTag(x)));
        }
    }

    public async Task<List<RecipePost>> GetAsync() =>
        await _recipeCollection.Find(_ => true).ToListAsync();

    public async Task<RecipePost?> GetAsync(string id) =>
        await _recipeCollection.Find(x => x.Id == id).FirstOrDefaultAsync();


    public async Task<List<AuthorPosts>> GetPostsByAuthorAsync()
    {
        var result = await _recipeCollection.Aggregate()
            .Group(x => x.AuthorId, g => new AuthorPosts
            { 
                AuthorId = g.Key,
                AuthorName  = g.First().AuthorName,
                TotalPosts = g.Count(), 
                LastPost = g.Max(x => x.Timestamp),
                PostTitles = g.Select(x => x.Title).ToList()
            })
            .SortByDescending(x => x.TotalPosts)
            .ToListAsync();
        
        return result;
    }


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


    /* Likes Collection Service */
    public async Task<List<RecipeLike>> GetLikesAsync() =>
        await _likesCollection.Find(_ => true).ToListAsync();

    public async Task<RecipeLike?> GetLikesAsync(string id) =>
        await _likesCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateLikeAsync(RecipeLike newRecipeLike) =>
        await _likesCollection.InsertOneAsync(newRecipeLike);

    public async Task UpdateLikeAsync(string id, RecipeLike updatedRecipeLike) =>
        await _likesCollection.ReplaceOneAsync(x => x.Id == id, updatedRecipeLike);

    public async Task RemoveLikeAsync(string id) =>
        await _likesCollection.DeleteOneAsync(x => x.Id == id);

    
    public async Task<List<RecipeLike>> GetAllLikesForRecipe (string recipeId) =>
        await _likesCollection.Find(x => x.RecipeId == recipeId).ToListAsync();
        
    public async Task<long> CountLikes()
    {
        var res = await _likesCollection.Find(_ => true).CountDocumentsAsync();
        return res;
    } 


}