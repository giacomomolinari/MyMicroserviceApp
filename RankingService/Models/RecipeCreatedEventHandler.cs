using System.Runtime.CompilerServices;
using EventBusInterface;
using RankingService.Services;

namespace RankingService.Models;

class RecipeCreatedEventHandler: IntegrationEventHandler<RecipeCreatedEvent>
{
    private RankingDBService _recipeCollection;


    public RecipeCreatedEventHandler(RankingDBService recipeCollection)
    {
        _recipeCollection = recipeCollection;
    }

    public async Task HandleAsync(RecipeCreatedEvent @event){

        // TEST ONLY: Console.WriteLine($"RecipeCreatedEvent for Recipe Id = {@event.RecipeId} is being handled by its custom handler.");
        // TEST ONLY: Console.WriteLine($"Title = {@event.RecipeName}, Author = {@event.AuthorName}");

        await _recipeCollection.CreateAsync(@event.ConvertToRecipeEntry());

    }
}