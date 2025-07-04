using SurveyBasket.Api.Authentication.Filters;

namespace SurveyBasket.Api.Controllers;

[Route("api/polls/{pollId}/[controller]")]
[ApiController]
public class QuestionsController(IQuestionService questionService) : ControllerBase
{
    [HttpGet("")]
    [HasPermission(Permissions.GetQuestions)]
    public async Task<IActionResult> GetAll([FromRoute] int pollId, CancellationToken cancellationToken)
    {
        var result = await questionService.GetAllAsync(pollId, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    [HttpGet("{id:int}")]
    [HasPermission(Permissions.GetQuestions)]
    public async Task<IActionResult> Get([FromRoute] int pollId, [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var result = await questionService.GetAsync(pollId, id, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    [HttpPost("")]
    [HasPermission(Permissions.AddQuestions)]
    public async Task<IActionResult> Add([FromRoute] int pollId, [FromBody] QuestionRequest request,
        CancellationToken cancellationToken)
    {
        var result = await questionService.AddAsync(pollId, request, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(Get), new { pollId, result.Value.Id }, result.Value)
            : result.ToProblem();
    }

    [HttpPut("{id}")]
    [HasPermission(Permissions.UpdateQuestions)]
    public async Task<IActionResult> Update([FromRoute] int pollId, [FromRoute] int id,
        [FromBody] QuestionRequest request, CancellationToken cancellationToken)
    {
        var result = await questionService.UpdateAsync(pollId, id, request, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpPut("{id}/togglePublish")]
    [HasPermission(Permissions.UpdateQuestions)]
    public async Task<IActionResult> TogglePublish([FromRoute] int pollId, [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var result = await questionService.ToggleStatusAsync(pollId, id, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}