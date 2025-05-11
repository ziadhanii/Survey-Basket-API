using SurveyBasket.Api.Contracts.Results;

namespace SurveyBasket.Api.Services;

public interface IResultService
{
    Task<Result<PollVotesResponse>> GetPollVotesAsync(int pollId,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<VotesPerDayResponse>>> GetVotesPerDayAsync(int pollId,
        CancellationToken cancellationToken = default);
}