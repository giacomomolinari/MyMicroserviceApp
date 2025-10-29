using EventBusInterface;
using System;

namespace EventListener;

public class RecipeCreatedEventHandler : IntegrationEventHandler<RecipeCreatedEvent>
{
    public async Task HandleAsync(RecipeCreatedEvent @event){
        Console.WriteLine($"RecipeCreatedEvent for Reciepe Id = {@event.RecipeId} is being handled by its custom handler.");
        Console.WriteLine($"Title = {@event.RecipeName}, Author = {@event.AuthorName}, CreationDate={@event.CreationDate}");
    }

}