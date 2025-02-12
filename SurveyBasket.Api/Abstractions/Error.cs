namespace SurveyBasket.Api.Abstractions;

public record Error(
    string Code,
    string Description
)
{
    public static readonly Error None = new(string.Empty, string.Empty);
}