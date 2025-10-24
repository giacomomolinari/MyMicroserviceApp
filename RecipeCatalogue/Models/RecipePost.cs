using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace RecipeCatalogue.Models;

public class RecipePost
{
    // this field represents the id key in database.
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)] // converted automatically from ObjectId type to string
    public string? Id { get; set; }

    // other fields are automatically mapped to fields of the same name
    [BsonRepresentation(BsonType.ObjectId)] // converted automatically from ObjectId type to string
    public string? AuthorId { get; set; }
    public string? AuthorName { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public List<string>? Tags { get; set; }
    public List<CommentPost>? CommentList { get; set; }

}