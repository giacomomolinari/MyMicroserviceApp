using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace RecipeCatalogue.Models;

public class RecipeLike
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? RecipeId { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? UserId { get; set; }

    public string? UserName { get; set; }

    public DateTime? Timestamp { get; set; } = DateTime.UtcNow; // Automatically set Timestamp to current time when creating a RecipeLike object

}