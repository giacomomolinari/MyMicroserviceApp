using Microsoft.AspNetCore.Mvc;

namespace RankingService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RankingServiceController
{
    private readonly ILogger<RankingServiceController> _logger;

    public RankingServiceController(ILogger<RankingServiceController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public List<string> Get()
    {
        List<string> res = ["value1", "value2"];
        return res;
    }
}
