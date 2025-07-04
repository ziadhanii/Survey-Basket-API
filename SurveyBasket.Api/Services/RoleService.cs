namespace SurveyBasket.Api.Services;

public class RoleService(RoleManager<ApplicationRole> roleManager, ApplicationDbContext context) : IRoleService
{
    public async Task<IEnumerable<RoleResponse>> GetAllAsync(bool? includeDisabled = false,
        CancellationToken cancellationToken = default) =>
        await roleManager.Roles
            .Where(x => !x.IsDefault && (!x.IsDeleted || (includeDisabled.HasValue && includeDisabled.Value)))
            .ProjectToType<RoleResponse>()
            .ToListAsync(cancellationToken);

    public async Task<Result<RoleDetailResponse>> GetAsync(string id)
    {
        if (await roleManager.FindByIdAsync(id) is not { } role)
            return Result.Failure<RoleDetailResponse>(RoleErrors.RoleNotFound);

        var permissions = await roleManager.GetClaimsAsync(role);

        var response = new RoleDetailResponse(role.Id, role.Name!, role.IsDeleted, permissions.Select(x => x.Value));

        return Result.Success(response);
    }

    public async Task<Result<RoleDetailResponse>> AddAsync(RoleRequest request)
    {
        var roleIsExists = await roleManager.RoleExistsAsync(request.Name);

        if (roleIsExists)
            return Result.Failure<RoleDetailResponse>(RoleErrors.DuplicatedRole);

        var allowedPermissions = Permissions.GetAllPermissions();

        if (request.Permissions.Except(allowedPermissions).Any())
            return Result.Failure<RoleDetailResponse>(RoleErrors.InvalidPermissions);

        var role = new ApplicationRole
        {
            Name = request.Name,
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };

        var result = await roleManager.CreateAsync(role);

        if (result.Succeeded)
        {
            var permissions = request.Permissions
                .Select(x => new IdentityRoleClaim<string>
                {
                    ClaimType = Permissions.Type,
                    ClaimValue = x,
                    RoleId = role.Id
                });

            await context.AddRangeAsync(permissions);
            await context.SaveChangesAsync();

            var response = new RoleDetailResponse(role.Id, role.Name, role.IsDeleted, request.Permissions);

            return Result.Success(response);
        }

        var error = result.Errors.First();

        return Result.Failure<RoleDetailResponse>(new Error(error.Code, error.Description,
            StatusCodes.Status400BadRequest));
    }

    public async Task<Result> UpdateAsync(string id, RoleRequest request)
    {
        var roleIsExists = await roleManager.Roles.AnyAsync(x => x.Name == request.Name && x.Id != id);

        if (roleIsExists)
            return Result.Failure<RoleDetailResponse>(RoleErrors.DuplicatedRole);

        if (await roleManager.FindByIdAsync(id) is not { } role)
            return Result.Failure<RoleDetailResponse>(RoleErrors.RoleNotFound);

        var allowedPermissions = Permissions.GetAllPermissions();

        if (request.Permissions.Except(allowedPermissions).Any())
            return Result.Failure<RoleDetailResponse>(RoleErrors.InvalidPermissions);

        role.Name = request.Name;

        var result = await roleManager.UpdateAsync(role);

        if (result.Succeeded)
        {
            var currentPermissions = await context.RoleClaims
                .Where(x => x.RoleId == id && x.ClaimType == Permissions.Type)
                .Select(x => x.ClaimValue)
                .ToListAsync();

            var newPermissions = request.Permissions.Except(currentPermissions)
                .Select(x => new IdentityRoleClaim<string>
                {
                    ClaimType = Permissions.Type,
                    ClaimValue = x,
                    RoleId = role.Id
                });

            var removedPermissions = currentPermissions.Except(request.Permissions);

            await context.RoleClaims
                .Where(x => x.RoleId == id && removedPermissions.Contains(x.ClaimValue))
                .ExecuteDeleteAsync();


            await context.AddRangeAsync(newPermissions);
            await context.SaveChangesAsync();

            return Result.Success();
        }

        var error = result.Errors.First();

        return Result.Failure<RoleDetailResponse>(new Error(error.Code, error.Description,
            StatusCodes.Status400BadRequest));
    }

    public async Task<Result> ToggleStatusAsync(string id)
    {
        if (await roleManager.FindByIdAsync(id) is not { } role)
            return Result.Failure<RoleDetailResponse>(RoleErrors.RoleNotFound);

        role.IsDeleted = !role.IsDeleted;

        await roleManager.UpdateAsync(role);

        return Result.Success();
    }
}