namespace RecipeCatalogue.Models;

public class CommentPost
{
    public int Id {get; set;}
    public string? AuthorId {get; set;}
    public string? RecipeId {get; set;}
    public string? content {get; set;}
}