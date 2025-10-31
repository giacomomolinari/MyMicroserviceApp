using EventBusInterface;

namespace RankingService.Models;

class RecipeCreatedEventHandler: IntegrationEventHandler<RecipeCreatedEvent>
{
    public async Task HandleAsync(RecipeCreatedEvent @event){
        
        Console.WriteLine($"RecipeCreatedEvent for Recipe Id = {@event.RecipeId} is being handled by its custom handler.");
        Console.WriteLine($"Title = {@event.RecipeName}, Author = {@event.AuthorName}");
    }
}