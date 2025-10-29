using Microsoft.AspNetCore.Mvc;
using RankingService.Models;
using RankingService.Services;

namespace RankingService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RankingServiceController
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
}
