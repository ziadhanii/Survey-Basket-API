using SurveyBasket.Api.Contracts.Questions;
using SurveyBasket.Api.Errors;

namespace SurveyBasket.Api.Services;

public class QuestionService(ApplicationDbContext context) : IQuestionService
{
    public async Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollId,
        CancellationToken cancellationToken = default)
    {
        var pollIsExisting = await context.Polls
            .AnyAsync(x => x.Id == pollId, cancellationToken);

        if (!pollIsExisting)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

        var questions = await context.Questions
            .Where(x => x.PollId == pollId)
            .Include(x => x.Answers)
            .ProjectToType<QuestionResponse>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<QuestionResponse>>(questions);
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

        return question is null
            ? Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound)
            : Result.Success(question);
    }

    public async Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest request,
        CancellationToken cancellationToken = default)
    {
        var pollIsExisting = await context.Polls
            .AnyAsync(x => x.Id == pollId, cancellationToken);

        if (!pollIsExisting)
            return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

        var questionIsExisting = await context.Questions
            .AnyAsync(x => x.Content == request.Content && x.PollId == pollId, cancellationToken);

        if (questionIsExisting)
            return Result.Failure<QuestionResponse>(QuestionErrors.DuplicatedQuestionContent);

        var question = request.Adapt<Question>();

        question.PollId = pollId;

        // Mapping done in MappingConfigurations with Mapster 
        // request.Answers.ForEach(answer => question.Answers.Add(new Answer { Content = answer }));

        await context.Questions.AddAsync(question, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

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

        return Result.Success();
    }

    public async Task<Result> ToggleStatusAsync(int pollId, int id, CancellationToken cancellationToken = default)
    {
        var question = await context.Questions.SingleOrDefaultAsync(x =>
            x.Id == id && x.PollId == pollId, cancellationToken);

        if (question is null)
            return Result.Failure(QuestionErrors.QuestionNotFound);

        question.IsActive = !question.IsActive;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}