using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeCatalogue.Models;
using RecipeCatalogue.Services;

namespace RecipeCatalogue.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RecipeLikesController : ControllerBase
{
    private readonly LikesCollectionService _likesCollectionService;

    public RecipeLikesController(LikesCollectionService likesCollectionService)
    {
        _likesCollectionService = likesCollectionService;
    }



    // GET: api/RecipeLikes 
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RecipeLike>>> GetLikesTable()
    {
        return await _likesCollectionService.GetAsync();
    }



    // GET: api/RecipeLikes/5
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

        return NoContent();
    }

    private async Task<bool> RecipeLikePairExists(string id)
    {
        var recipeLikeObj = await _likesCollectionService.GetAsync(id);
        return recipeLikeObj is not null;
    }



}