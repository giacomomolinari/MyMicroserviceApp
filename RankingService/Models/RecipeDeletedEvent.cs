using EventBusInterface;

namespace RankingService.Models;

public class RecipeDeletedEvent: IntegrationEvent
{
    public string? RecipeId { get; set; }
    
    public RecipeDeletedEvent() { }
    public RecipeDeletedEvent(string recipeId)
    {
        RecipeId = recipeId;
    }

}