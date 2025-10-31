using EventBusInterface;
using Microsoft.Extensions.Configuration.UserSecrets;
using NuGet.Packaging.Signing;

namespace RecipeCatalogue.Models;

public class RecipeLikeEvent : IntegrationEvent
{
    public string? RecipeId { get; private set; }

    public string? UserId { get; private set; }

    public DateTime? Timestamp { get; private set; }

    // If true, this is a new like being added.
    // If false, this is an old like being deleted.
    public bool? IsLike { get; private set; }

    public RecipeLikeEvent(string recipeId, string userId, DateTime timestamp, bool isLike)
    {
        RecipeId = recipeId;
        UserId = userId;
        Timestamp = timestamp;
        IsLike = isLike;
    }

    public RecipeLikeEvent(RecipeLike recipeLike, bool isLike)
    {
        RecipeId = recipeLike.RecipeId;
        UserId = recipeLike.UserId;
        Timestamp = recipeLike.Timestamp;
        IsLike = isLike;
    }


}