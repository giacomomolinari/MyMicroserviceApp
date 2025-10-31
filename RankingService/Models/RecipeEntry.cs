using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RankingService.Models;

public class RecipeEntry
{
    public RecipeEntry(string? recipeId, DateTime? creationDate, string? authorName, string? recipeName, int? likes)
    {
        RecipeId = recipeId;
        RecipeCreationDate = creationDate;
        AuthorName = authorName;
        RecipeName = recipeName;
        Likes = likes;
    }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? RecipeId { get; set; }

    public DateTime? RecipeCreationDate { get; set; }

    public string? AuthorName { get; set; }

    public string? RecipeName { get; set; }

    public int? Likes { get; set; }
}