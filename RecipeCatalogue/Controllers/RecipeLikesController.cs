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
public class RecipeLikesController : ControllerBase
{
    private readonly LikesCollectionService _likesCollectionService;

    private readonly IntegrationEventBus _eventBus;

    public RecipeLikesController(LikesCollectionService likesCollectionService, IntegrationEventBus eventBus)
    {
        _likesCollectionService = likesCollectionService;
        _eventBus = eventBus;
    }



    // GET: api/RecipeLikes 
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RecipeLike>>> GetLikesTable()
    {
        return await _likesCollectionService.GetAsync();
    }



    // GET: api/RecipeLikes/5
    // NEED TO CHECK THE USER DOES NOT ALREADY LIKE RECIPE? ALTHOUGH MIGHT BE BEST TO 
    // DO THAT FRONTENT MAYBE, SINCE NEED TO SHOW USER THAT HE ALREADY LIKES RECIPE
    // ANYWAY...
    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<RecipeLike>> GetRecipeLikePair(string id)
    {
        var recipeLikeObj = await _likesCollectionService.GetAsync(id);

        if (recipeLikeObj == null)
        {
            return NotFound();
        }

        return recipeLikeObj;
    }



    // PUT: api/RecipeLikes/5
    // SHOULD REMOVE THIS...
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutRecipeLikePair(string id, RecipeLike recipeLikeObj)
    {
        if (id != recipeLikeObj.Id)
        {
            return BadRequest();
        }

        var recipe = await _likesCollectionService.GetAsync(id);
        if (recipe == null)
        {
            return NotFound();
        }

        await _likesCollectionService.UpdateAsync(id, recipeLikeObj);

        return NoContent();
    }

    // POST: api/RecipeLikes
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<RecipeLike>> PostRecipeLikePair(RecipeLike recipeLikeObj)
    {
        await _likesCollectionService.CreateAsync(recipeLikeObj);

        // post event that new Like object was created
        RecipeLikeEvent likeCreatedEvent = new RecipeLikeEvent(recipeLikeObj, isLike: true);
        await _eventBus.Publish(likeCreatedEvent);

        return CreatedAtAction(nameof(GetRecipeLikePair), new { id = recipeLikeObj.Id }, recipeLikeObj);
    }

    // DELETE: api/RecipePosts/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRecipLikePair(string id)
    {
        var recipeLikeObj = await _likesCollectionService.GetAsync(id);
        if (recipeLikeObj is null)
        {
            return NotFound();
        }

        await _likesCollectionService.RemoveAsync(id);

        RecipeLikeEvent likeDeletedEvent = new RecipeLikeEvent(recipeLikeObj, isLike: false);
        await _eventBus.Publish(likeDeletedEvent);

        return NoContent();
    }

    private async Task<bool> RecipeLikePairExists(string id)
    {
        var recipeLikeObj = await _likesCollectionService.GetAsync(id);
        return recipeLikeObj is not null;
    }



}