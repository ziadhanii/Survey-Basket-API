namespace SurveyBasket.Api.Errors;

public static class RoleErrors
{
    public static readonly Error RoleNotFound =
        new("Role.NotFound", "Role not found", StatusCodes.Status404NotFound);

    public static readonly Error DuplicatedRole =
        new("Role.DuplicatedRole", "Another role with the same name is already exists", StatusCodes.Status409Conflict);

    public static readonly Error InvalidPermissions =
        new("Role.InvalidPermissions", "Invalid permissions", StatusCodes.Status400BadRequest);
}