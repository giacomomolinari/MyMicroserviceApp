using EventBusInterface;

namespace RankingService.Models;

public class RecipeCreatedEvent: IntegrationEvent
{

    public string? Message { get;  set; }

    public DateTime? RecipeCreationDate { get;  set; }  

    public string? RecipeId { get;  set; }

    public string? RecipeName { get;  set; }

    public string? AuthorName { get;  set; }


    public RecipeCreatedEvent() { }
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


    // create a new recipe entry with 0 likes
    public RecipeEntry ConvertToRecipeEntry()
    {
        return new RecipeEntry(creationDate: CreationDate, recipeId: RecipeId, recipeName: RecipeName, authorName: AuthorName, likes: 0);
    }


}