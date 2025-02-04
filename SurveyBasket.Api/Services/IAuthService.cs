namespace SurveyBasket.Api.Services;

public interface IAuthService
{
    Task<AuthResponse?> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default);
}