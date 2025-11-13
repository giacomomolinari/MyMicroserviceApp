using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using NuGet.Protocol.Plugins;

namespace RecipeCatalogue.Models;

public class RecipeTag
{
    // lowercase name
    public string Name { get; set; } = default!;

    public RecipeTag(string name)
    {
        Name = name.ToLower();
    }
}