namespace SurveyBasket.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PollsController(IPollService pollService) : ControllerBase
{
    /// <summary>
    /// Get all polls.
    /// </summary>
    [HttpGet("")]
    public IActionResult GetAll()
    {
        var polls = pollService.GetAll();
        var response = polls.Adapt<List<PollResponse>>();

        return Ok(response);
    }

    /// <summary>
    /// Get a specific poll by ID.
    /// </summary>
    [HttpGet("{id:int}")]
    public IActionResult Get([FromRoute] int id)
    {
        var poll = pollService.Get(id);

        if (poll is null)
            return NotFound();

        var response = poll.Adapt<PollResponse>();

        return Ok(response);
    }

    /// <summary>
    /// Add a new poll.
    /// </summary>
    [HttpPost("")]
    public IActionResult Add([FromBody] Poll request)
    {
        var newPoll = pollService.Add(request.Adapt<Poll>());

        return CreatedAtAction(nameof(Get), new { id = newPoll.Id }, newPoll);
    }

    /// <summary>
    /// Update an existing poll.
    /// </summary>
    [HttpPut("{id:int}")]
    public IActionResult Update([FromRoute] int id, [FromBody] Poll request)
    {
        var poll = pollService.Get(id);

        if (poll is null)
            return NotFound();

        var isUpdated = pollService.Update(id, request.Adapt<Poll>());

        if (!isUpdated)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Delete a poll by ID.
    /// </summary>
    [HttpDelete("{id:int}")]
    public IActionResult Delete([FromRoute] int id)
    {
        var poll = pollService.Get(id);

        if (poll is null)
            return NotFound();

        pollService.Delete(id);
        return NoContent();
    }
}