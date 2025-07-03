using SurveyBasket.Api.Contracts.Users;

namespace SurveyBasket.Api.Services;

public class UserService(UserManager<ApplicationUser> userManager) : IUserService
{
    public async Task<Result<UserProfileResponse>> GetProfileAsync(string userId)
    {
        var user = await userManager.Users
            .Where(x => x.Id == userId)
            .ProjectToType<UserProfileResponse>()
            .SingleAsync();

        return Result.Success(user);
    }
}