using EventBusInterface;
using System;

namespace RecipeCatalogue.Models;

public class RecipeCreatedEvent: IntegrationEvent
{

    public string? Message { get; private set; }

    public DateTime? RecipeCreationDate { get; private set; }
    
    public string? RecipeId { get; private set; }

    public string? RecipeName { get; private set; }

    public string? AuthorName { get; private set; }


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
    
    public RecipeCreatedEvent(RecipePost post)
    {
        this.Id = Guid.NewGuid();
        this.CreationDate = DateTime.UtcNow;

        this.RecipeId = post.Id;
        this.RecipeCreationDate = post.Timestamp;
        this.RecipeName = post.Title;
        this.AuthorName = post.AuthorName;
        this.Message = "";

    }
}