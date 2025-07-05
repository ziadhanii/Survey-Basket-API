using Microsoft.Extensions.Caching.Hybrid;
using SurveyBasket.Api.Contracts.Common;

namespace SurveyBasket.Api.Services;

public class QuestionService(
    ApplicationDbContext context,
    HybridCache hybridCache) : IQuestionService
{
    private const string CachePrefix = "availableQuestions";

    public async Task<Result<PaginatedList<QuestionResponse>>> GetAllAsync(int pollId,
        RequestFilters filters,
        CancellationToken cancellationToken = default)
    {
        var pollIsExists = await context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken: cancellationToken);

        if (!pollIsExists)
            return Result.Failure<PaginatedList<QuestionResponse>>(PollErrors.PollNotFound);

        var query = context.Questions
            .Where(
                x => x.PollId == pollId &&
                     (
                         string.IsNullOrEmpty(filters.SearchValue) ||
                         x.Content.Contains(filters.SearchValue)
                     )
            )
            .Include(x => x.Answers)
            .ProjectToType<QuestionResponse>()
            .AsNoTracking();

        var questions =
            await PaginatedList<QuestionResponse>.CreateAsync(query, filters.PageNumber, filters.PageSize,
                cancellationToken);

        return Result.Success(questions);
    }

    public async Task<Result<IEnumerable<QuestionResponse>>> GetAvailableAsync(int pollId, string userId,
        CancellationToken cancellationToken = default)
    {
        var hasVote = await context.Votes.AnyAsync(x => x.PollId == pollId && x.UserId == userId, cancellationToken);

        if (hasVote)
            return Result.Failure<IEnumerable<QuestionResponse>>(VoteErrors.DuplicatedVote);

        var pollIsExists = await context.Polls.AnyAsync(
            x => x.Id == pollId && x.IsPublished && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) &&
                 x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);

        if (!pollIsExists)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

        var cacheKey = $"{CachePrefix}-{pollId}";

        var questions = await hybridCache.GetOrCreateAsync<IEnumerable<QuestionResponse>>(cacheKey, async cacheEntry =>
                await context.Questions
                    .Where(x => x.PollId == pollId && x.IsActive)
                    .Include(x => x.Answers)
                    .Select(q => new QuestionResponse(
                        q.Id,
                        q.Content,
                        q.Answers.Where(a => a.IsActive).Select(a => new AnswerResponse(a.Id, a.Content))
                    ))
                    .AsNoTracking()
                    .ToListAsync(cancellationToken)
            , cancellationToken: cancellationToken);

        return Result.Success(questions);
    }

    public async Task<Result<QuestionResponse>> GetAsync(int pollId, int id,
        CancellationToken cancellationToken = default)
    {
        var question = await context.Questions
            .Where(x => x.PollId == pollId && x.Id == id)
            .Include(x => x.Answers)
            .ProjectToType<QuestionResponse>()
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);

        if (question is null)
            return Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound);

        return Result.Success(question);
    }

    public async Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest request,
        CancellationToken cancellationToken = default)
    {
        var pollIsExists = await context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken: cancellationToken);

        if (!pollIsExists)
            return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

        var questionIsExists =
            await context.Questions.AnyAsync(x => x.Content == request.Content && x.PollId == pollId,
                cancellationToken: cancellationToken);

        if (questionIsExists)
            return Result.Failure<QuestionResponse>(QuestionErrors.DuplicatedQuestionContent);

        var question = request.Adapt<Question>();
        question.PollId = pollId;

        await context.AddAsync(question, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        await hybridCache.RemoveAsync($"{CachePrefix}-{pollId}", cancellationToken);

        return Result.Success(question.Adapt<QuestionResponse>());
    }

    public async Task<Result> UpdateAsync(int pollId, int id, QuestionRequest request,
        CancellationToken cancellationToken = default)
    {
        var questionIsExists = await context.Questions
            .AnyAsync(x => x.PollId == pollId
                           && x.Id != id
                           && x.Content == request.Content,
                cancellationToken
            );

        if (questionIsExists)
            return Result.Failure(QuestionErrors.DuplicatedQuestionContent);

        var question = await context.Questions
            .Include(x => x.Answers)
            .SingleOrDefaultAsync(x => x.PollId == pollId && x.Id == id, cancellationToken);

        if (question is null)
            return Result.Failure(QuestionErrors.QuestionNotFound);

        question.Content = request.Content;

        //current answers
        var currentAnswers = question.Answers.Select(x => x.Content).ToList();

        //add new answer
        var newAnswers = request.Answers.Except(currentAnswers).ToList();

        newAnswers.ForEach(answer => { question.Answers.Add(new Answer { Content = answer }); });

        question.Answers.ToList().ForEach(answer => { answer.IsActive = request.Answers.Contains(answer.Content); });

        await context.SaveChangesAsync(cancellationToken);

        await hybridCache.RemoveAsync($"{CachePrefix}-{pollId}", cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ToggleStatusAsync(int pollId, int id, CancellationToken cancellationToken = default)
    {
        var question =
            await context.Questions.SingleOrDefaultAsync(x => x.PollId == pollId && x.Id == id, cancellationToken);

        if (question is null)
            return Result.Failure(QuestionErrors.QuestionNotFound);

        question.IsActive = !question.IsActive;

        await context.SaveChangesAsync(cancellationToken);

        await hybridCache.RemoveAsync($"{CachePrefix}-{pollId}", cancellationToken);

        return Result.Success();
    }
}