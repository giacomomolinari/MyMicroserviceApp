using EventBusInterface;

namespace RankingService.Models;

public class RecipeLikeEventHandler : IntegrationEventHandler<RecipeLikeEvent>
{
    public async Task HandleAsync(RecipeLikeEvent @event)
    {
        Console.WriteLine($"RecipeLikeEvent is being handled by its custom handler.");
        string action = @event.IsLike == true ? "LIKED" : "UNLIKED";
        Console.WriteLine($"Recipe with Id = {@event.RecipeId} was {action} by User with Id = {@event.UserId}");
    }

}