using SurveyBasket.Api.Contracts.Results;
using SurveyBasket.Api.Errors;

namespace SurveyBasket.Api.Services;

public class ResultService(ApplicationDbContext context) : IResultService
{
    public async Task<Result<PollVotesResponse>> GetPollVotesAsync(int pollId,
        CancellationToken cancellationToken = default)
    {
        var pollVotes = await context.Polls
            .Where(x => x.Id == pollId)
            .Select(x => new PollVotesResponse(
                x.Title,
                x.Votes.Select(v => new VoteResponse(
                    $" {v.User.FirstName} {v.User.LastName}",
                    v.SubmittedOn,
                    v.VoteAnswers.Select(answer => new QuestionAnswerResponse(
                        answer.Question.Content, answer.Answer.Content))
                ))
            ))
            .SingleOrDefaultAsync(cancellationToken);

        return pollVotes is null
            ? Result.Failure<PollVotesResponse>(PollErrors.PollNotFound)
            : Result.Success(pollVotes);
    }


    public async Task<Result<IEnumerable<VotesPerDayResponse>>> GetVotesPerDayAsync(int pollId,
        CancellationToken cancellationToken = default)
    {
        var pollIsExisting = await context.Polls
            .AnyAsync(x => x.Id == pollId, cancellationToken);

        if (!pollIsExisting)
            return Result.Failure<IEnumerable<VotesPerDayResponse>>(PollErrors.PollNotFound);

        var votesPerDay = await context.Votes
            .Where(x => x.PollId == pollId)
            .GroupBy(x => new { Date = DateOnly.FromDateTime(x.SubmittedOn) })
            .Select(g => new VotesPerDayResponse(
                g.Key.Date,
                g.Count()))
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<VotesPerDayResponse>>(votesPerDay);
    }
}