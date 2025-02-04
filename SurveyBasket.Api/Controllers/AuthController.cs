namespace SurveyBasket.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var authResult = await authService.GetTokenAsync(request.Email, request.Password, cancellationToken);

        return authResult is null ? BadRequest("Invalid email or password") : Ok(authResult);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var authResult =
            await authService.GetRefreshTokenAsync(token: request.Token, request.RefreshToken, cancellationToken);

        return authResult is null ? BadRequest("Invalid token") : Ok(authResult);
    }

    [HttpPost("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshTokenAsync([FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var isRevoked =
            await authService.RevokeTokenAsync(token: request.Token, request.RefreshToken, cancellationToken);

        return isRevoked ? Ok(isRevoked) : BadRequest("Operation failed");
    }
}