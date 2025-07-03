namespace SurveyBasket.Api.Controllers;

[Route("me")]
[ApiController]
[Authorize]
public class AccountController(IUserService userService) : ControllerBase
{
    [HttpGet("")]
    public async Task<IActionResult> Info()
    {
        var result = await userService.GetProfileAsync(User.GetUserId()!);

        return Ok(result.Value);
    }
}