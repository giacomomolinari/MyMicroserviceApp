namespace RankingService.Models;

public class RankingDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string RecipeCollectionName { get; set; } = null!;
}