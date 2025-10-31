using EventBusInterface;

namespace RankingService.Models;

public class RecipeLikeEvent: IntegrationEvent
{
    public string? RecipeId { get;  set; }


    // CAN PROBABLY DO WITHOUT THE UserId and TimeStamp fields...
    public string? UserId { get; set; }

    public DateTime? Timestamp { get; set; }

    // If true, this is a new like being added.
    // If false, this is an old like being deleted.
    public bool? IsLike { get; set; }

    public RecipeLikeEvent(){}

    public RecipeLikeEvent(string recipeId, string userId, DateTime timestamp, bool isLike)
    {
        RecipeId = recipeId;
        UserId = userId;
        Timestamp = timestamp;
        IsLike = isLike;
    }
}