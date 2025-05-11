using SurveyBasket.Api.Contracts.Votes;
using SurveyBasket.Api.Errors;

namespace SurveyBasket.Api.Services;

public class VoteService(ApplicationDbContext context) : IVoteService
{
    public async Task<Result> AddVoteAsync(int pollId, string userId, VoteRequest request,
        CancellationToken cancellationToken = default)
    {
        var hasVote = await context.Votes.AnyAsync(x =>
            x.PollId == pollId && x.UserId == userId, cancellationToken);

        if (hasVote)
            return Result.Failure(VoteErrors.DuplicatedVote);

        var pollIsExisting = await context.Polls
            .AnyAsync(
                x => x.Id == pollId && x.IsPublished && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) &&
                     x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);

        if (!pollIsExisting)
            return Result.Failure(PollErrors.PollNotFound);

        var availableQuestions = await context.Questions
            .Where(x => x.PollId == pollId && x.IsActive)
            .Select(q => q.Id)
            .ToListAsync(cancellationToken);

        if (!request.Answers.Select(x => x.QuestionId).SequenceEqual(availableQuestions))
            return Result.Failure(VoteErrors.InvalidQuestion);


        var vote = new Vote
        {
            PollId = pollId,
            UserId = userId,
            VoteAnswers = request.Answers.Adapt<IEnumerable<VoteAnswer>>().ToList()
        };

        await context.AddAsync(vote, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}