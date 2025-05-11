using SurveyBasket.Api.Errors;


namespace SurveyBasket.Api.Services;

public class PollService(ApplicationDbContext context) : IPollService
{
    public async Task<IEnumerable<PollResponse>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await context.Polls
            .AsNoTracking()
            .ProjectToType<PollResponse>()
            .ToListAsync(cancellationToken);


    public async Task<IEnumerable<PollResponse>> GetCurrentAsync(CancellationToken cancellationToken = default) =>
        await context.Polls
            .Where(x => x.IsPublished && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) &&
                        x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow))
            .AsNoTracking()
            .ProjectToType<PollResponse>()
            .ToListAsync(cancellationToken);


    public async Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await context.Polls.FindAsync(id, cancellationToken);

        return poll is not null
            ? Result.Success(poll.Adapt<PollResponse>())
            : Result.Failure<PollResponse>(PollErrors.PollNotFound);
    }

    public async Task<Result<PollResponse>> AddAsync(PollRequest request, CancellationToken cancellationToken = default)
    {
        var isExistingTitle = await context.Polls.AnyAsync(p => p.Title == request.Title, cancellationToken);

        if (isExistingTitle)
            return Result.Failure<PollResponse>(PollErrors.DuplicatedPollTitle);

        var poll = request.Adapt<Poll>();

        await context.AddAsync(poll, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(poll.Adapt<PollResponse>());
    }

    public async Task<Result> UpdateAsync(int id, PollRequest request, CancellationToken cancellationToken = default)
    {
        var currentPoll = await context.Polls.FindAsync(id, cancellationToken);


        var isExistingTitle =
            await context.Polls.AnyAsync(p => p.Title == request.Title && p.Id != id, cancellationToken);

        if (isExistingTitle)
            return Result.Failure<PollResponse>(PollErrors.DuplicatedPollTitle);

        if (currentPoll is null)
            return Result.Failure(PollErrors.PollNotFound);

        currentPoll.Title = request.Title;
        currentPoll.Summary = request.Summary;
        currentPoll.StartsAt = request.StartsAt;
        currentPoll.EndsAt = request.EndsAt;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await context.Polls.FindAsync(id, cancellationToken);

        if (poll is null)
            return Result.Failure(PollErrors.PollNotFound);

        context.Remove(poll);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> TogglePublishStatusAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await context.Polls.FindAsync(id, cancellationToken);

        if (poll is null)
            return Result.Failure(PollErrors.PollNotFound);

        poll.IsPublished = !poll.IsPublished;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}