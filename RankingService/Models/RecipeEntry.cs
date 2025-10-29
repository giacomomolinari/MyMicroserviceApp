using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RankingService.Models;

public class RecipeEntry
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? RecipeId { get; set; }

    public string? AuthorName { get; set; }

    public string? RecipeName { get; set; }

    public int? Likes { get; set; }
}