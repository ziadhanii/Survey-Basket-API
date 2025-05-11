using SurveyBasket.Api.Extensions;

namespace SurveyBasket.Api.Controllers;

[Route("api/polls/{pollId}/vote")]
[ApiController]
[Authorize]
public class VotesController(IQuestionService questionService) : ControllerBase
{
    [HttpGet("")]
    public async Task<IActionResult> Start([FromRoute] int pollId,
        CancellationToken cancellationToken = default)
    {
        var userId = User.GetUserId();

        var result = await questionService.GetAvailableAsync(pollId, userId!, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }
}