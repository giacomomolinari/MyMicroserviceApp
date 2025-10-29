using EventBusInterface;
using System;

namespace EventListener;

public record RecipeCreatedEvent: IntegrationEvent
{

    public string? Message { get; private set; }
    
    public string? RecipeId { get; private set; }

    public string? RecipeName { get; private set; }

    public string? AuthorName { get; private set; }


    public RecipeCreatedEvent(string recipeId, string recipeName, string authorName, string message = "")
    {
        this.Id = Guid.NewGuid();
        this.CreationDate = DateTime.UtcNow;

        this.RecipeId = recipeId;
        this.RecipeName = recipeName;
        this.AuthorName = authorName;
        this.Message = message;
    }


}