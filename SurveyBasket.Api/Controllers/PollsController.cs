namespace SurveyBasket.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PollsController(IPoolService poolService) : ControllerBase
{
    [HttpGet("")]
    public IActionResult GetAll()
    {
        return Ok(poolService.GetAll());
    }

    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
        var poll = poolService.Get(id);

        return poll is null ? NotFound() : Ok(poll);
    }


    [HttpPost("")]
    public IActionResult Add(Poll request)
    {
        var newPoll = poolService.Add(request);
        return CreatedAtAction(nameof(Get), new { id = newPoll.Id }, newPoll);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, Poll request)
    {
        var poll = poolService.Get(id);

        var isUpdated = poolService.Update(id, request);

        if (!isUpdated)
            return NotFound();

        return NoContent();
    }
    
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var poll = poolService.Get(id);
        
        if (poll is null)
            return NotFound();
        
        poolService.Delete(id);
        return NoContent();
    }
}