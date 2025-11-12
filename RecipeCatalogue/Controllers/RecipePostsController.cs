using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeCatalogue.Models;
using RecipeCatalogue.Services;
using EventBusInterface;


namespace RecipeCatalogue.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RecipePostsController : ControllerBase
{
    private readonly RecipeDBService _recipeDBService;
    private readonly IntegrationEventBus _eventBus;

    public RecipePostsController(RecipeDBService recipeDBService, IntegrationEventBus eventBus)
    {
        _recipeDBService = recipeDBService;
        _eventBus = eventBus;
    }



    // GET: api/RecipePosts
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RecipePost>>> GetRecipeTable()
    {
        return await _recipeDBService.GetAsync();
    }



    // GET: api/RecipePosts/5
    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<RecipePost>> GetRecipePost(string id)
    {
        var recipePost = await _recipeDBService.GetAsync(id);

        if (recipePost == null)
        {
            return NotFound();
        }

        return recipePost;
    }


    // GET: api/RecipePosts/count
    [HttpGet("count")]
    public async Task<ActionResult<long>> GetRecipePostsCount()
    {
        return await _recipeDBService.CountEntries();
    }

    // PUT: api/RecipePosts/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutRecipePost(string id, RecipePost recipePost)
    {
        if (id != recipePost.Id)
        {
            return BadRequest();
        }

        var recipe = await _recipeDBService.GetAsync(id);
        if (recipe == null)
        {
            return NotFound();
        }

        await _recipeDBService.UpdateAsync(id, recipePost);

        return NoContent();
    }



    // POST: api/RecipePosts
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<RecipePost>> PostRecipePost(RecipePost recipePost)
    {
        await _recipeDBService.CreateAsync(recipePost);

        // create new RecipeCreatedEvent containig new recipe post information
        RecipeCreatedEvent @event = new RecipeCreatedEvent(recipePost);

        // send a new RecipeCreatedEvent
        await _eventBus.Publish(@event);

        return CreatedAtAction(nameof(GetRecipePost), new { id = recipePost.Id }, recipePost);
    }

    // DELETE: api/RecipePosts/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRecipePost(string id)
    {
        var recipePost = await _recipeDBService.GetAsync(id);
        if (recipePost is null)
        {
            return NotFound();
        }

        await _recipeDBService.RemoveAsync(id);

        // publish event for recipe deletion
        RecipeDeletedEvent @event = new RecipeDeletedEvent(id);
        await _eventBus.Publish(@event);

        return NoContent();
    }

    private async Task<bool> RecipePostExists(string id)
    {
        var recipePost = await _recipeDBService.GetAsync(id);
        return recipePost is not null;
    }
}
