namespace SurveyBasket.Api.Services;

public class PollService(ApplicationDbContext context) : IPollService
{
    public async Task<IEnumerable<Poll>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await context.Polls.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<Poll?> GetAsync(int id, CancellationToken cancellationToken = default) =>
        await context.Polls.FindAsync(id, cancellationToken);

    public async Task<Poll> AddAsync(Poll poll, CancellationToken cancellationToken = default)
    {
        await context.AddAsync(poll, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return poll;
    }

    public async Task<bool> UpdateAsync(int id, Poll poll, CancellationToken cancellationToken = default)
    {
        var currentPoll = await GetAsync(id, cancellationToken);

        if (currentPoll is null)
            return false;

        currentPoll.Title = poll.Title;
        currentPoll.Summary = poll.Summary;
        currentPoll.StartsAt = poll.StartsAt;
        currentPoll.EndsAt = poll.EndsAt;

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await GetAsync(id, cancellationToken);

        if (poll is null)
            return false;

        context.Remove(poll);

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> TogglePublishStatusAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await GetAsync(id, cancellationToken);

        if (poll is null)
            return false;

        poll.IsPublished = !poll.IsPublished;
        
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}