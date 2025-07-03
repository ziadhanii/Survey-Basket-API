namespace SurveyBasket.Api.Abstractions.Consts;

public class RegexPatterns
{
    public const string Password =
        "(?=(.*[0-9]))(?=.*[\\!@#$%^&*()\\\\[\\]{}\\-_+=~`|:;\"'<>,./?])(?=.*[a-z])(?=(.*[A-Z]))(?=(.*)).{8,}";
}