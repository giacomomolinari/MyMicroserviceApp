namespace RecipeCatalogue.Models;

// used to store the DB settings in the appsettings.json file
public class RecipeDBSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string RecipeCollectionName { get; set; } = null!;

    public string LikesCollectionName { get; set; } = null!;

    public string TagsCollectionName { get; set; } = null!;
}