using EventBusInterface;
using RankingService.Services;

namespace RankingService.Models;

public class RecipeLikeEventHandler : IntegrationEventHandler<RecipeLikeEvent>
{
    private readonly RankingDBService _recipeCollection;

    public RecipeLikeEventHandler(RankingDBService recipeCollection)
    {
        _recipeCollection = recipeCollection;
    }

    public async Task HandleAsync(RecipeLikeEvent @event)
    {
        Console.WriteLine($"RecipeLikeEvent is being handled by its custom handler.");
        string action = @event.IsLike == true ? "LIKED" : "UNLIKED";
        Console.WriteLine($"Recipe with Id = {@event.RecipeId} was {action} by User with Id = {@event.UserId}");

        if (@event.IsLike == true)
        {
            await _recipeCollection.IncreaseLikesAsync(@event.RecipeId);
        }
        else
        {
            await _recipeCollection.DecreaseLikesAsync(@event.RecipeId);
        }
    }

}