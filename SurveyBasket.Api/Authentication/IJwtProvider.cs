using SurveyBasket.Api.Entities;

namespace SurveyBasket.Api.Authentication;

public interface IJwtProvider
{
    (string token, int expiresIn ) GenerateToken(ApplicationUser user);
    string? ValidateToken(string token);
}