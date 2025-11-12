using RecipeCatalogue.Models;

namespace IntegrationTestCatalogueRanking;

// Stores content inputted by user to post a recipe
// I.e. does not include an Id or a Datetime
public class RecipeInput
{
    public string? AuthorId { get; set; } = "68de74df222ff19ad1cebea4";
    public string? AuthorName { get; set; } = "Giacomo Molinari";
    public string? Title { get; set; } = "Test Recipe";
    public string? Content { get; set; } = "Content of Test Recipe";
    public List<string>? Tags { get; set; } = null;
    public List<CommentPost>? CommentList { get; set; } = null;

}