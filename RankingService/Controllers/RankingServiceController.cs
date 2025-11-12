using Microsoft.AspNetCore.Mvc;
using RankingService.Models;
using RankingService.Services;

namespace RankingService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RankingServiceController: ControllerBase
{
    private readonly ILogger<RankingServiceController> _logger;
    private readonly RankingDBService _recipeCollection;

    public RankingServiceController(ILogger<RankingServiceController> logger, RankingDBService dbService)
    {
        _logger = logger;
        _recipeCollection = dbService;
    }

    [HttpGet]
    public async Task<ActionResult<List<RecipeEntry>>> Get()
    {
        List<RecipeEntry> res = await _recipeCollection.GetAsync();
        return res;
    }

    [HttpGet("top")]
    public async Task<ActionResult<List<RecipeEntry>>> GetRanked([FromQuery] int num, [FromQuery] int hours)
    {
        // Should really be caching these in Redis, with some restrictions on the accepted values
        // of num and hours (e.g. num=10 or 100, hours= 24 or 24*7 or 24*31 or 24*365 or -1 (for all time))
        List<RecipeEntry> res = await _recipeCollection.GetRanked(num, hours);
        return res;
    }

    [HttpGet("count")]
    public async Task<ActionResult<long>> GetRecipeEntriesCount()
    {
        return await _recipeCollection.CountEntries();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRecipeEntry(string id)
    {
        var recipeEntry = await _recipeCollection.GetAsync(id);
        if (recipeEntry is null)
        {
            return NotFound();
        }

        await _recipeCollection.DeleteAsync(id);
        return NoContent();
    }
}
