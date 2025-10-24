using Microsoft.EntityFrameworkCore;

namespace RecipeCatalogue.Models;
// Used for in-memory database only...
public class RecipeCatalogueContext : DbContext
{
    public RecipeCatalogueContext(DbContextOptions<RecipeCatalogueContext> options) : base(options)
    {       
    }

    public DbSet<RecipePost> RecipeTable {get; set;} = null!;
}