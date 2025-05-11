using SurveyBasket.Api.Contracts.Votes;

namespace SurveyBasket.Api.Services;

public interface IVoteService
{
    Task<Result> AddVoteAsync(int pollId, string userId, VoteRequest request,
        CancellationToken cancellationToken = default);
}