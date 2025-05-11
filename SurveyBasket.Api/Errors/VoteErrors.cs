namespace SurveyBasket.Api.Errors;

public static class VoteErrors
{
    public static readonly Error DuplicatedVote = new(
        "Vote.DuplicatedVote",
        "This user has already voted before for this poll",
        StatusCodes.Status409Conflict);


    public static readonly Error InvalidQuestion = new(
        "Vote.InvalidQuestion",
        "Invalid questions provided",
        StatusCodes.Status400BadRequest
    );
}