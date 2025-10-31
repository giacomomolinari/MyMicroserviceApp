using EventBusInterface;

namespace RecipeCatalogue.Models;

public class RecipeDeletedEvent : IntegrationEvent
{
    public string? RecipeId { get; private set; }

    public RecipeDeletedEvent(string recipeId)
    {
        RecipeId = recipeId;
    }
}