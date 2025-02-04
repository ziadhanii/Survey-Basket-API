namespace SurveyBasket.Api.Services;

public class AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider) : IAuthService
{
    private const int RefreshTokenExpiryDays = 14;

    public async Task<AuthResponse?> GetTokenAsync(string email, string password,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user == null)
            return null;

        var isValidPassword = await userManager.CheckPasswordAsync(user, password);

        if (!isValidPassword)
            return null;

        var (token, expiresIn) = jwtProvider.GenerateToken(user);

        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(RefreshTokenExpiryDays);

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            ExpiresOn = refreshTokenExpiration
        });

        await userManager.UpdateAsync(user);

        return new AuthResponse(
            Id: user.Id,
            Email: user.Email,
            FirstName: user.FirstName,
            LastName: user.LastName,
            Token: token,
            ExpiresIn: expiresIn,
            RefreshToken: refreshToken,
            RefreshTokenExpiration: refreshTokenExpiration
        );
    }

    public async Task<AuthResponse?> GetRefreshTokenAsync(string token, string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var userId = jwtProvider.ValidateToken(token);

        if (userId is null)
            return null;

        var user = await userManager.FindByIdAsync(userId);

        if (user is null)
            return null;

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(rt => rt.Token == refreshToken && rt.IsActive);

        if (userRefreshToken == null)
            return null;

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        var (newToken, expiresIn) = jwtProvider.GenerateToken(user);

        var newRefreshToken = GenerateRefreshToken();

        var newRefreshTokenExpiration = DateTime.UtcNow.AddDays(RefreshTokenExpiryDays);

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = newRefreshToken,
            ExpiresOn = newRefreshTokenExpiration
        });

        await userManager.UpdateAsync(user);

        return new AuthResponse(
            Id: user.Id,
            Email: user.Email,
            FirstName: user.FirstName,
            LastName: user.LastName,
            Token: newToken,
            ExpiresIn: expiresIn,
            RefreshToken: newRefreshToken,
            RefreshTokenExpiration: newRefreshTokenExpiration
        );
    }

    public async Task<bool> RevokeTokenAsync(string token, string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var userId = jwtProvider.ValidateToken(token);

        if (userId is null)
            return false;

        var user = await userManager.FindByIdAsync(userId);

        if (user is null)
            return false;

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(rt => rt.Token == refreshToken && rt.IsActive);

        if (userRefreshToken == null)
            return false;

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        await userManager.UpdateAsync(user);

        return true;
    }

    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}