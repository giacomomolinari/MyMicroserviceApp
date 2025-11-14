using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RecipeCatalogue.Models;

public class AuthorPosts
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)] // converted automatically from ObjectId type to string
    public string? AuthorId { get; set; }
    public string? AuthorName {get; set;}
    public int TotalPosts { get; set; }
    public DateTime? LastPost { get; set; }
    public List<String?> PostTitles { get; set; } = default!;
}