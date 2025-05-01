using System;

namespace SurveyBasket.Api.Contracts.Polls;

public record PollResponse(
    int Id,
    string Title,
    string Summary,
    bool IsPublished,
    DateOnly StartsAt,
    DateOnly EndsAt);