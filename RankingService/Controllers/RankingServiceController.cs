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
    public async Task<List<RecipeEntry>> Get()
    {
        List<RecipeEntry> res = await _recipeCollection.GetAsync();
        return res;
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
