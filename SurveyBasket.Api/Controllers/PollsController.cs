using SurveyBasket.Api.Contracts.Polls;

namespace SurveyBasket.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PollsController(IPollService pollService) : ControllerBase
{
    [HttpGet("")]
    [Authorize]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var polls = await pollService.GetAllAsync(cancellationToken);

        var response = polls.Adapt<IEnumerable<PollResponse>>();

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
    {
        var poll = await pollService.GetAsync(id, cancellationToken);

        if (poll is null)
            return NotFound();

        var response = poll.Adapt<PollResponse>();

        return Ok(response);
    }

    [HttpPost("")]
    public async Task<IActionResult> Add([FromBody] PollRequest request,
        CancellationToken cancellationToken)
    {
        var newPoll = await pollService.AddAsync(request.Adapt<Poll>(), cancellationToken);

        return CreatedAtAction(nameof(Get), new { id = newPoll.Id }, newPoll);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest request,
        CancellationToken cancellationToken)
    {
        var isUpdated = await pollService.UpdateAsync(id, request.Adapt<Poll>(), cancellationToken);

        if (!isUpdated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        var isDeleted = await pollService.DeleteAsync(id, cancellationToken);

        if (!isDeleted)
            return NotFound();

        return NoContent();
    }

    [HttpPut("{id}/togglePublish")]
    public async Task<IActionResult> TogglePublish([FromRoute] int id, CancellationToken cancellationToken)
    {
        var isUpdated = await pollService.TogglePublishStatusAsync(id, cancellationToken);

        if (!isUpdated)
            return NotFound();

        return NoContent();
    }
}