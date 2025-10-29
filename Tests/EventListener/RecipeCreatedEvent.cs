using EventBusInterface;
using System;

namespace EventListener;

public class RecipeCreatedEvent: IntegrationEvent
{

    public string? Message { get;  set; }

    public DateTime? RecipeCreationDate { get;  set; }  

    public string? RecipeId { get;  set; }

    public string? RecipeName { get;  set; }

    public string? AuthorName { get;  set; }


    public RecipeCreatedEvent() { } // this is supposed to fix the deserialization problem?

    public RecipeCreatedEvent(DateTime recipeCreationDate, string recipeId, string recipeName, string authorName, string message = "")
    {
        this.Id = Guid.NewGuid();
        this.CreationDate = DateTime.UtcNow;

        this.RecipeCreationDate = recipeCreationDate;
        this.RecipeId = recipeId;
        this.RecipeName = recipeName;
        this.AuthorName = authorName;
        this.Message = message;
    }


}