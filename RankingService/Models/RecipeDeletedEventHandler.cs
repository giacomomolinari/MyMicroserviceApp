using System.Runtime.CompilerServices;
using EventBusInterface;
using RankingService.Services;

namespace RankingService.Models;

class RecipeDeletedEventHandler: IntegrationEventHandler<RecipeDeletedEvent>
{
    private RankingDBService _recipeCollection;

    public RecipeDeletedEventHandler(RankingDBService recipeCollection)
    {
        _recipeCollection = recipeCollection;
    }

    public async Task HandleAsync(RecipeDeletedEvent @event){
        // TEST ONLY: Console.WriteLine($"RecipeDeletedEvent for Recipe Id = {@event.RecipeId} is being handled by its custom handler.");
        await _recipeCollection.DeleteGivenRecipeIdAsync(@event.RecipeId);

    }
}